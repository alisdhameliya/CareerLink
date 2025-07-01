using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Implementations;
using Repositories.Interfaces;
using Repositories.Model;


namespace API.Controllers
{
    [ApiController]
    [Route("api/jobposts")]
    public class JobPostApiController : ControllerBase
    {
        private readonly IJobPostInterface _jobRepository;
        private readonly INotificationService _notificationService;
        public JobPostApiController(IJobPostInterface repository, INotificationService notificationService)
        {
            _jobRepository = repository;
            _notificationService = notificationService;
        }

        [HttpGet("getAllDepartments")]
        public async Task<IActionResult> getAllDepartments()
        {
            try
            {
                List<t_department> departments = await _jobRepository.getAllDepartments();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error in getAllDepartments method in JobPostApiController {ex.Message}");
                return BadRequest(new { message = $"{ex.Message}" });
            }
        }

        [HttpGet("getAllSkills")]
        public async Task<IActionResult> getAllSkills()
        {
            try
            {
                List<t_skills> skills = await _jobRepository.getAllSkills();
                return Ok(skills);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error in getAllSkills method in JobPostApiController {ex.Message}");
                return BadRequest(new { message = $"{ex.Message}" });
            }
        }
        // [HttpPost("create")]
        // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JobPostResponse))]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> Create([FromForm] Job_Post job)
        // {
        //     try
        //     {
        //         if (!ModelState.IsValid)
        //             return BadRequest(new { message = "All Fields are Required" });

        //         int result = await _jobRepository.CreateJob(job);
        //         if (result == 0)
        //             return Ok(new { success = false, message = "Error while Posting Job" });

        //         var userId = User.FindFirst("uid")?.Value;
        //         var userIdInt = userId != null ? Convert.ToInt32(userId) : 0;

        //         var notificationMessage = $"{job.company.c_company_name} posted a '{job.c_job_title}' job hiring";
        //         await _notificationService.SendUserNotificationAsync(
        //             userIdInt,
        //             notificationMessage,
        //             "job posted");

        //         return Ok(new JobPostResponse
        //         {
        //             Success = true,
        //             Message = "Job Posted Successfully",
        //             Notification = new NotificationResponse
        //             {
        //                 Message = notificationMessage,
        //                 Type = "job_posting",
        //                 Timestamp = DateTime.UtcNow
        //             }
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error in Create method: {ex.Message}");
        //         return BadRequest(new { message = $"{ex.Message}" });
        //     }
        // }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JobPostResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Job_Post job)
        {
            try
            {
                if (job == null)
                    return BadRequest(new { message = "Job object is null." });

                if (!ModelState.IsValid)
                    return BadRequest(new { message = "All Fields are Required" });

                if (job.c_company_id == 0)
                    return BadRequest(new { message = "Company ID is required." });

                int result = await _jobRepository.CreateJob(job);

                if (result == -1)
                {
                    return Ok(new { success = false, message = "Your company is not approved yet. Job posting is not allowed." });
                }
                if (result == 0)
                {
                    return Ok(new { success = false, message = "Error while Posting Job" });
                }

                // **Fix for job.company null issue**
                // string companyName = job.company?.c_company_name ?? "Your company";

                // var userId = User.FindFirst("uid")?.Value;
                // var userIdInt = userId != null ? Convert.ToInt32(userId) : 0;

                var notificationMessage = $"{job.c_company_name} posted a '{job.c_job_title}' job hiring";
                await _notificationService.SendNotificationAsync(
                    notificationMessage,
                    "job posted");

                return Ok(new JobPostResponse
                {
                    Success = true,
                    Message = "Job Posted Successfully",
                    Notification = new NotificationResponse
                    {
                        Message = notificationMessage,
                        Type = "job_posting",
                        Timestamp = DateTime.UtcNow
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create method: {ex.Message}");
                return BadRequest(new { message = $"{ex.Message}" });
            }
        }




        [HttpPut("edit")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JobPostResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromQuery] int id, [FromBody] Job_Post post)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                var userIdInt = userId != null ? Convert.ToInt32(userId) : 0;

                int result = await _jobRepository.EditJob(id, post);
                if (result == 0)
                    return Ok(new { success = false, message = "Error while Editing Job" });

                var userNotificationMessage = $"{post.c_company_name} updated their {post.c_job_title} job hiring";
                await _notificationService.SendNotificationAsync(
                    userNotificationMessage,
                    "job_update");

                // var broadcastMessage = $"{post.company.c_company_name} posted a '{post.c_job_title}' job";
                // await _notificationService.SendNotificationAsync(
                //     broadcastMessage,
                //     "job_update_alert");

                return Ok(new JobPostResponse
                {
                    Success = true,
                    Message = "Job Edited Successfully",
                    Notification = new NotificationResponse
                    {
                        Message = userNotificationMessage,
                        Type = "job_update",
                        Timestamp = DateTime.UtcNow
                    }
                });
            }
            catch (Exception ex)
            {
                var errorMessage = $"Job update failed: {ex.Message}";
                await _notificationService.SendNotificationAsync(
                    errorMessage,
                    "system_error");

                Console.WriteLine($"Error in Edit method: {ex.Message}");
                return BadRequest(new { message = errorMessage });
            }
        }

        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JobPostResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteJob(
        [FromQuery] int id,
        [FromQuery] string jobTitle,
        [FromQuery] string companyName)
        {
            try
            {
                Console.WriteLine("Company Name:" + companyName);
                Console.WriteLine("Job Title:" + jobTitle);
                Console.WriteLine("Job ID:" + id);

                var userId = User.FindFirst("uid")?.Value;
                var userIdInt = userId != null ? Convert.ToInt32(userId) : 0;

                int result = await _jobRepository.DeleteJob(id);
                if (result == 0)
                    return Ok(new { success = false, message = "Error while Deleting Job" });

                var notificationMessage = $"{companyName} closed the hiring for {jobTitle} job";
                await _notificationService.SendNotificationAsync(
                    notificationMessage,
                    "job_deletion");

                return Ok(new JobPostResponse
                {
                    Success = true,
                    Message = "Job Deleted Successfully",
                    Notification = new NotificationResponse
                    {
                        Message = notificationMessage,
                        Type = "job_deletion",
                        Timestamp = DateTime.UtcNow
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete method: {ex.Message}");
                return BadRequest(new { message = $"{ex.Message}" });
            }
        }
        // Add these response models to your project
        public class JobPostResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public NotificationResponse Notification { get; set; }
            public NotificationResponse BroadcastNotification { get; set; } // For edit action
        }

        public class NotificationResponse
        {
            public string Message { get; set; }
            public string Type { get; set; }
            public DateTime Timestamp { get; set; }
        }

        [HttpGet("getJobs")]
        public async Task<IActionResult> getjobs(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "All Fields are Required" });
                }

                List<Job_Post> jobs = await _jobRepository.GetJobDetails(id);

                return Ok(jobs);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error in getAllJobs method in JobPostApiController {ex.Message}");
                return BadRequest(new { message = $"{ex.Message}" });
            }
        }

        [HttpGet("getjobsuser")]
        public async Task<List<t_ViewJobs>> GetJobsUser()
        {
            var res = await _jobRepository.GetAllJobs();
            return res;
        }

        [HttpGet("getonejob")]
        public async Task<Job_Post> GetOneJob([FromQuery] int id)
        {
            Job_Post jb = await _jobRepository.GetOneJob(id);
            return jb;
        }
    }
}
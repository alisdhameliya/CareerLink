using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Model;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]


    public class ApplyJobController : ControllerBase
    {
        private readonly IApplyjobInterface _applyjobInterface;
        private readonly EmailService _emailService;
        public ApplyJobController(IApplyjobInterface applyjobInterface, EmailService emailService)
        {
            _applyjobInterface = applyjobInterface;
            _emailService = emailService;
        }

        [HttpGet("CheckFields/{id}")]
        public async Task<IActionResult> CheckFields(int id)
        {
            var res = await _applyjobInterface.CheckFields(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Field check successful",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "User not found or required fields missing",
                    data = res
                });
            }
        }



        [HttpPost("ApplyJob")]
        public async Task<IActionResult> ApplyJob([FromForm] t_apply_job apply_Job)
        {
            var res = await _applyjobInterface.ApplyJob(apply_Job);

            if (res == 1)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Job applied successfully.",
                    data = res
                });
            }
            else if (res == 2)
            {
                return Ok(new
                {
                    success = false,
                    msg = "You have already applied for this job.",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Failed to apply for the job due to an error.",
                    data = res
                });
            }
        }


        // [HttpDelete("DeleteAppliedJobs/{id}")]
        // public async Task<IActionResult> DeleteAppliedJobs(int id){
        //     var res = await _applyjobInterface.DeleteJob(id);
        //     if(res != null){
        //         return Ok(new {
        //             success = true,
        //             msg = "Deleted applied job",
        //             data = res
        //         });
        //     }
        //     else{
        //         return BadRequest(new {
        //             success = true,
        //             msg = "Deletion failed for applied job",
        //             data = res
        //         });
        //     }
        // }

        [HttpGet("GetApplyJobApplication/{id}")]
        public async Task<List<t_apply_job>> GetApplyJobApplication(int id, string? jobTitle = null, string? status = null)
        {
            List<t_apply_job> tt = await _applyjobInterface.GetApplyJobApplication(id, jobTitle, status);
            return tt;
        }

        [HttpPut("UpdateStatusOfJobApplication")]
        public async Task<IActionResult> UpdateStatusOfJobApplication([FromForm] t_apply_job apply_Job)
        {
            var res = await _applyjobInterface.UpdateStatusOfJobApplication(apply_Job);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success update status of job application",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Failed to update status of job application",
                    data = res
                });
            }
        }

        [HttpGet("GetUserAppliedJobApplication/{id}")]
        public async Task<List<t_apply_job>> GetUserAppliedJobApplication(int id)
        {
            List<t_apply_job> tt = await _applyjobInterface.GetUserAppliedJobApplication(id);
            return tt;
        }

        [HttpGet("GetJobTitle/{id}")]
        public async Task<List<Job_Post>> GetJobTitle(int id)
        {
            List<Job_Post> tt = await _applyjobInterface.GetJobTitles(id);
            return tt;
        }

        [HttpPost("InterviewSchedule")]
        public async Task<IActionResult> InterviewSchedule([FromForm] t_interview_schedule interview_Schedule)
        {
            var res = await _applyjobInterface.InterviewSchedule(interview_Schedule);

            if (res != null)
            {
                string body = $@"
                <table style='max-width: 600px; margin: auto; font-family: Arial, sans-serif; border: 1px solid #e0e0e0; border-radius: 10px; overflow: hidden;'>
                <tr style='background-color: #0d6efd; color: white; text-align: center;'>
                    <td style='padding: 20px; font-size: 24px; font-weight: bold;'>Interview Scheduled</td>
                </tr>
                <tr>
                    <td style='padding: 30px; font-size: 16px; color: #333;'>
                    <p>Dear <strong>{interview_Schedule.c_fullName}</strong>,</p>
                    <p>We are pleased to inform you that your interview has been scheduled as follows:</p>
                    <ul style='list-style: none; padding-left: 0;'>
                        <li><strong>Date:</strong> {interview_Schedule.c_interview_date}</li>
                        <li><strong>Time:</strong> {interview_Schedule.c_interview_time}</li>
                        <li><strong>Meeting Link:</strong> <a href='{interview_Schedule.c_meeting_url}' style='color: #0d6efd;'>{interview_Schedule.c_meeting_url}</a></li>
                    </ul>
                    <p style='margin-top: 20px;'>Please be on time and ensure you have all the required documents ready.</p>
                    <p style='margin-top: 30px;'>Best of luck with your interview!</p>
                    </td>
                </tr>
                <tr style='background-color: #f8f9fa; text-align: center; font-size: 12px; color: #666;'>
                    <td style='padding: 15px;'>This is an automated email, please do not reply to it.</td>
                </tr>
                </table>";

                await _emailService.SendEmailAsync(
                    interview_Schedule.c_email,
                    "Interview Scheduled",
                    body
                );

                return Ok(new
                {
                    success = true,
                    msg = "Successfully interview scheduled and email sent",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Failed to schedule interview",
                    data = res
                });
            }
        }

        [HttpGet("GetInterviews_ScheduleByCompany/{id}")]
        public async Task<List<t_interview_schedule>> GetInterviews_ScheduleByCompany(int id)
        {
            List<t_interview_schedule> tt = await _applyjobInterface.GetInterviews_ScheduleByCompany(id);
            return tt;
        }

        [HttpPut("UpdateInterviewSchedule")]
        public async Task<IActionResult> UpdateInterviewSchedule([FromForm] t_interview_schedule interview_Schedule)
        {
            var res = await _applyjobInterface.UpdateInterviewSchedule(interview_Schedule);
            if (res != null)
            {
                string body = $@"
                <table style='max-width: 600px; margin: auto; font-family: Arial, sans-serif; border: 1px solid #e0e0e0; border-radius: 10px; overflow: hidden;'>
                <tr style='background-color: #0d6efd; color: white; text-align: center;'>
                    <td style='padding: 20px; font-size: 24px; font-weight: bold;'>Interview Schedule Updated</td>
                </tr>
                <tr>
                    <td style='padding: 30px; font-size: 16px; color: #333;'>
                    <p>Dear <strong>{interview_Schedule.c_fullName}</strong>,</p>
                    <p>We would like to inform you that your interview schedule has been <strong>updated</strong>. Please find the new details below:</p>
                    <ul style='list-style: none; padding-left: 0;'>
                        <li><strong>Updated Date:</strong> {interview_Schedule.c_interview_date}</li>
                        <li><strong>Updated Time:</strong> {interview_Schedule.c_interview_time}</li>
                        <li><strong>Meeting Link:</strong> <a href='{interview_Schedule.c_meeting_url}' style='color: #0d6efd;'>{interview_Schedule.c_meeting_url}</a></li>
                    </ul>
                    <p style='margin-top: 20px;'>Kindly make a note of the updated schedule and ensure your availability accordingly.</p>
                    <p style='margin-top: 30px;'>We wish you all the best for your interview!</p>
                    </td>
                </tr>
                <tr style='background-color: #f8f9fa; text-align: center; font-size: 12px; color: #666;'>
                    <td style='padding: 15px;'>This is an automated email, please do not reply to it.</td>
                </tr>
                </table>";

                await _emailService.SendEmailAsync(
                    interview_Schedule.c_email,
                    "Interview Schedule Updated",
                    body
                );
                return Ok(new
                {
                    success = true,
                    msg = "Successfully updated interview schedule",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Failed to update interview schedule",
                    data = res
                });
            }
        }

    
        [HttpGet("UserAppliedStatus")]
        public async Task<IActionResult> UserAppliedStatus(int userid,int jobid){
            var res = await _applyjobInterface.UserAppliedStatus(userid,jobid);
            if(res!=null){
                return Ok(new {
                    success = true,
                    msg = "Successfully getting user applied for job",
                    data = res
                });
            }
            else{
                return BadRequest(new {
                    success = false,
                    msg = "Failed to getting user applied for job",
                    data = res
                });
            }
        }
    }
}

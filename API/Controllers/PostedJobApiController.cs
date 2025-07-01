using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;
using Repositories.Model;
using Repositories.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/postedjob")]
    public class PostedJobApiController : ControllerBase
    {
        private readonly IPostedJobInterface _jobRepository;
        private readonly JobSearchServices _jobSearchService;
        public PostedJobApiController(IPostedJobInterface repository, JobSearchServices jobSearchService)
        {
            _jobRepository = repository;
            _jobSearchService = jobSearchService;
        }
        [HttpGet("getJobs")]
        public async Task<IActionResult> getjobs()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "All Fields are Required" });
                }

                List<Job_Post> jobs = await _jobRepository.GetJobDetails();

                return Ok(new { success = true, totalCount = jobs.Count, data = jobs });
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error in getAllJobs method in PostedJobApiController {ex.Message}");
                return BadRequest(new { message = $"{ex.Message}" });
            }
        }

        [HttpGet("getjobsuser")]
        public async Task<List<t_ViewJobs>> GetJobsUser()
        {
            var res = await _jobRepository.GetAllJobs();
            return res;
        }

        [HttpGet("getJobDescription")]
        public async Task<IActionResult> getJobDescription([FromQuery] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "All Fields are Required" });
                }

                Job_Post jobs = await _jobRepository.GetJobDescription(id);

                return Ok(jobs);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error in getJobDescription method in PostedJobApiController {ex.Message}");
                return BadRequest(new { message = $"{ex.Message}" });
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchJobs(
            [FromQuery] string? searchText,
            [FromQuery] string? jobTitle,
            [FromQuery] string? location,
            [FromQuery] string? jobType,
            [FromQuery] string? salaryRange,
            [FromQuery] decimal? searchSalary,
            [FromQuery] int? vacancy,
            [FromQuery] int? departmentId,
            [FromQuery] string? qualification,
            [FromQuery] string? skills,
            [FromQuery] int? companyId,
            [FromQuery] string? companyName,
            [FromQuery] string? departmentName,
            [FromQuery] string? companyLogo)

        {
            var results = await _jobSearchService.SearchJobs(
                searchText, jobTitle, location, jobType,
                salaryRange, searchSalary, vacancy, departmentId,
                qualification, skills, companyId, companyName, departmentName, companyLogo);
            // qualification, skills, companyId);
            var uniqueResults = results.Distinct().ToList();


            return Ok(new { success = true, totalCount = uniqueResults.Count, data = uniqueResults });
        }
    }
}
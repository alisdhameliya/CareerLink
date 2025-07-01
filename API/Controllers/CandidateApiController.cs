using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Services;


namespace API.Controllers
{
    public class CandidateApiController : ControllerBase
    {
        private readonly ICandidateInterface _candidateRepo;
        private readonly JobSearchServices _jobSearchService;

        public CandidateApiController(ICandidateInterface candidateRepo, JobSearchServices jobSearchService)
        {
            _candidateRepo = candidateRepo;
            _jobSearchService = jobSearchService;
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


            return Ok(uniqueResults);
        }

        // [HttpGet("searchKeyword")]
        // public async Task<IActionResult> SearchJobs([FromQuery] string? searchText)
        // {
        //     var results = await _jobSearchService.SearchJobs(searchText);

        //     return Ok(results.Distinct().ToList());
        // }

        [HttpGet("GetAllJobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            try
            {
                // Get all jobs from the database
                var jobs = await _candidateRepo.GetJobs();

                // Create or update the Elasticsearch index
                await _jobSearchService.CreateOrUpdateIndex(jobs);

                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to create/update index: {ex.Message}");
            }
        }
    }
}
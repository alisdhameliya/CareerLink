using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminApiController : ControllerBase
    {

        private readonly IAdminInterface _adminRepository;

        public AdminApiController(IAdminInterface adminRepository)
        {
            _adminRepository = adminRepository;
            
        }

        // [HttpPost("send-email")]
        // public IActionResult SendEmail([FromBody] EmailRequest request)
        // {
        //     _emailService.SendEmail(request.ToEmail, request.UserName, request.Link);
        //     return Ok("Email sent successfully.");
        // }

        [HttpGet("get-user")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _adminRepository.GetUsers();
            return Ok(user);
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _adminRepository.DeleteUser(id);
            return Ok(result);
        }
        [HttpGet("get-user-count")]
        public async Task<IActionResult> GetUserCount()
        {
            var count = await _adminRepository.GetUsersCount();
            return Ok(new { totalUsers = count });
        }


        [HttpGet("get-recruiter-count")]
        public async Task<IActionResult> GetRecruiterCount()
        {
            var count = await _adminRepository.GetRecruitersCount();
            return Ok(new { totalUsers = count });
        }

        [HttpGet("getrecruiter")]
        public async Task<IActionResult> GetRecruiter()
        {
            var recruiter = await _adminRepository.GetRecruiters();
            return Ok(recruiter);
        }

        [HttpPut("updaterecruiterstatus/{companyId}")]
        public async Task<IActionResult> UpdateRecruiterStatus(int companyId, bool approved)
        {
            var result = await _adminRepository.UpdateRecruiterStatus(companyId, approved);
            if (result)
                return Ok(new { message = "Recruiter status updated successfully" });
            return NotFound(new { message = "Recruiter not found" });
        }
        [HttpPut("updaterecruiterstatusreject/{companyId}")]
        public async Task<IActionResult> UpdateRecruiterStatusReject(int companyId,string reason, bool approved)
        {
            var result = await _adminRepository.UpdateRecruiterstatusReject(companyId,reason, approved);
            if (result)
                return Ok(new { message = "Recruiter status updated successfully" });
            return NotFound(new { message = "Recruiter not found" });
        }
        [HttpPut("bulkupdaterecruiterstatus")]
        public async Task<IActionResult> BulkUpdateRecruiterStatus(List<int> companyIds, bool approved)
        {
            var result = await _adminRepository.BulkUpdateRecruiterStatus(companyIds, approved);
            if (result)
                return Ok(new { message = "Recruiter status updated successfully" });
            return NotFound(new { message = "Recruiter not found" });
        }

       

        [HttpPut("bulkupdaterecruiterstatusreject")]
        public async Task<IActionResult> BulkUpdateRecruiterStatusReject(List<int> companyIds, string reason,bool approved)
        {
            var result = await _adminRepository.BulkUpdateRecruiterStatusReject(companyIds,reason, approved);
            if (result)
                return Ok(new { message = "Recruiter status updated successfully" });
            return NotFound(new { message = "Recruiter not found" });
        }

        [HttpDelete("deleterecruiter/{companyId}")]
        public async Task<IActionResult> DeleteRecruiter(int companyId)
        {
            var result = await _adminRepository.DeleteRecruiter(companyId);
            return Ok(result);
        }

        [HttpGet("pendingapproval")]
        public async Task<IActionResult> PendingApproval()
        {
            var pending = await _adminRepository.PendingApproval();
            return Ok(pending);
        }

        [HttpGet("get-jobpost-count")]
        public async Task<IActionResult> GetJobPostCount()
        {
            var count = await _adminRepository.GetJobPostCount();
            return Ok(new { totalJobs = count });
        }

        [HttpGet("get-registration-stats")]
        public async Task<IActionResult> GetRegistrationStats([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var stats = await _adminRepository.GetRegistrationStats(startDate, endDate);
            return Ok(stats);
        }

        [HttpGet("get-user-distribution")]
        public async Task<IActionResult> GetUserDistribution()
        {
            var distribution = await _adminRepository.GetUserDistribution();
            return Ok(distribution);
        }

        [HttpGet("getjob")]
        public async Task<IActionResult> GetJobs()
        {
            var job = await _adminRepository.GetJobs();
            return Ok(job);
        }
        public class EmailRequest
        {
            public string ToEmail { get; set; }
            public string UserName { get; set; }
            public string Link { get; set; }
        }
    }
}
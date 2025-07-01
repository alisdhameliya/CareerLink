using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Models;
using Repositories.Implementations;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndustryApiController : ControllerBase
    {
        private readonly IIndustryInterface _industryRepository;

        public IndustryApiController(IIndustryInterface industryRepository)
        {
            _industryRepository = industryRepository;
        }

        [HttpGet("industries")]
        public async Task<IActionResult> GetIndustries()
        {
            try
            {
                var industries = await _industryRepository.GetIndustriesAsync();
                return Ok(industries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
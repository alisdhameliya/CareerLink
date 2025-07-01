using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyApiController : ControllerBase
    {
        private readonly ICompanyInterface _companyRepository;
        private readonly ILogger<CompanyApiController> _logger;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IConfiguration _configuration;

        public CompanyApiController(ICompanyInterface companyRepository,
                                    ILogger<CompanyApiController> logger,
                                    CloudinaryService cloudinaryService,
                                    IConfiguration configuration)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cloudinaryService = cloudinaryService ?? throw new ArgumentNullException(nameof(cloudinaryService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet("getCompanyName/{id}")]
        public async Task<IActionResult> GetCompanyName(int id)
        {
            var company = await _companyRepository.GetCompanyName(id);
            return Ok(company);
        }


        [HttpGet("getCompanyId/{id}")]
        public Task<int> GetCompanyId(int id)
        {
            var res = _companyRepository.GetCompanyId(id);
            return res;
        }

        /// <summary>
        /// Registers a new company.
        /// </summary>
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] t_companies company, IFormFile companyLogo)
        {
            try
            {
                if (company == null) return BadRequest("Invalid company data.");
                if (company.LegalDocuments == null || company.LegalDocuments.Count == 0)
                    return BadRequest("Legal documents are required.");

                var uploadedFiles = await UploadLegalDocuments(company.LegalDocuments);
                if (uploadedFiles == null) return StatusCode(500, "Failed to upload legal documents.");

                company.c_legal_documents = uploadedFiles.ToArray();
                company.LegalDocuments = null; // Prevent sending in response

                if (companyLogo != null)
                {
                    string logoUrl = await UploadCompanyLogo(companyLogo);
                    if (logoUrl == null) return BadRequest("Only PNG and JPG formats are allowed for the logo.");
                    company.c_company_logo = logoUrl;
                }

                int result = await _companyRepository.RegisterCompany(company);

                return result switch
                {
                    0 => Ok(new { success = false, message = "Company is already registered" }),
                    1 => Ok(new { success = true, message = "Company registered successfully" }),
                    _ => BadRequest(new { success = false, message = "Something went wrong" })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering company: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Gets recruiter details by ID.
        /// </summary>
        [HttpGet("getRecruiter")]
        public async Task<IActionResult> GetRecruiter(int id)
        {
            try
            {
                var result = await _companyRepository.GetRecruiterById(id);
                if (result == null) return NotFound("Recruiter not found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching recruiter: {ex.Message}");
                return StatusCode(500, "Error retrieving recruiter details.");
            }
        }

        /// <summary>
        /// Uploads legal documents to Cloudinary.
        /// </summary>
        private async Task<List<string>> UploadLegalDocuments(List<IFormFile> legalDocuments)
        {
            var uploadedFiles = new List<string>();

            foreach (var file in legalDocuments)
            {
                using var stream = file.OpenReadStream();
                string fileUrl = await _cloudinaryService.UploadImageAsync(stream, file.FileName, "Documents");
                if (string.IsNullOrEmpty(fileUrl)) return null;
                uploadedFiles.Add(fileUrl);
            }

            return uploadedFiles;
        }

        /// <summary>
        /// Uploads a company logo to Cloudinary.
        /// </summary>
        private async Task<string> UploadCompanyLogo(IFormFile companyLogo)
        {
            string logoExtension = Path.GetExtension(companyLogo.FileName).ToLower();
            if (logoExtension != ".png" && logoExtension != ".jpg" && logoExtension != ".jpeg")
                return null;

            using var stream = companyLogo.OpenReadStream();
            return await _cloudinaryService.UploadImageAsync(stream, companyLogo.FileName, "Logos");
        }

        [HttpGet("getMandatoryFields")]
        public async Task<List<t_companies>> GetMandatoryFields()
        {
            var mandatoryFields = await _companyRepository.GetMandatoryFields();
            return mandatoryFields;
        }

        [HttpGet("getCompanyStatus/{id}")]
        public async Task<IActionResult> GetCompanyStatus(int id)
        {
            var result = await _companyRepository.GetCompanyStatus(id);
            return Ok(result);
        }
    }
}
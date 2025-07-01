using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Web;
using System.Net;

namespace MVC.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _uploadPath;

        public AdminController(ILogger<AdminController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _uploadPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult User()
        {
            return View();
        }
         public IActionResult Recruiter()
        {
            return View();
        }
         public IActionResult RecruiterManagement()
        {
            return View();
        }
         public IActionResult UserManagement()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult R()
        {
           return View();
        }

        [HttpGet("preview-document/{filename}")]
        public IActionResult PreviewDocument(string filename)
        {
            try
            {
                _logger.LogInformation($"Attempting to preview document: {filename}");
                
                // Check if the URL is a Cloudinary URL
                if (filename.StartsWith("http"))
                {
                    _logger.LogInformation($"Document is a Cloudinary URL: {filename}");
                    
                    // For PDF files from Cloudinary, we need to ensure proper content type
                    if (filename.ToLower().EndsWith(".pdf"))
                    {
                        // Add proper headers for PDF viewing
                        Response.Headers.Add("Content-Type", "application/pdf");
                        Response.Headers.Add("Content-Disposition", "inline; filename=document.pdf");
                        Response.Headers.Add("Access-Control-Allow-Origin", "*");
                        
                        // For PDFs, we'll use a proxy approach to handle CORS
                        using (var client = new System.Net.Http.HttpClient())
                        {
                            var response = client.GetAsync(filename).Result;
                            var content = response.Content.ReadAsByteArrayAsync().Result;
                            return File(content, "application/pdf");
                        }
                    }
                    
                    return Redirect(filename);
                }

                // If not a Cloudinary URL, handle as local file
                var decodedFilename = WebUtility.UrlDecode(filename);
                var fullPath = Path.Combine(_uploadPath, decodedFilename);
                
                _logger.LogInformation($"Full file path: {fullPath}");
                
                if (!System.IO.File.Exists(fullPath))
                {
                    _logger.LogWarning($"File not found at path: {fullPath}");
                    return NotFound("File not found");
                }

                var fileBytes = System.IO.File.ReadAllBytes(fullPath);
                var contentType = GetContentType(decodedFilename);
                
                _logger.LogInformation($"Returning file with content type: {contentType}");
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error previewing document: {ex.Message}");
                return StatusCode(500, "Error previewing document");
            }
        }

        private string GetContentType(string filename)
        {
            var extension = Path.GetExtension(filename).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
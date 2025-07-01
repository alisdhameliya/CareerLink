using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MVC.Controllers
{
    public class UserDetailsController : Controller
    {
        private readonly ILogger<UserDetailsController> _logger;

        public UserDetailsController(ILogger<UserDetailsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddUserDetails()
        {
            return View();
        }

         public IActionResult PersonalDetail()
        {
            return View();
        }

        public IActionResult UsersDetail(){
            return View();
        }

        public IActionResult Education(){
            return View();
        }

        public IActionResult Experience(){
            return View();
        }

        public IActionResult Projects(){
            return View();
        }

        public IActionResult Certificates(){
            return View();
        }

        public IActionResult Skills(){
            return View();
        }

         public IActionResult JobPreference(){
            return View();
        }

        public IActionResult UploadResume(){
            return View();
        }

        public IActionResult PersonalInfo(){
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
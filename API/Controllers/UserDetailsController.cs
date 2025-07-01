using System.IO.Pipelines;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController : ControllerBase
    {
        private readonly IEducation_Details _education;
        private readonly IWorkExperience _experience;
        private readonly IUserSkills _userskills;
        private readonly IUserResume _resume;
        private readonly IUserCertificate _certificate;
        private readonly IUserProjects _projects;
        private readonly IJobPreference _preference;

        private readonly IUserProfileDetail _profile;
        private readonly CloudinaryService _cloudinaryService;

        public UserDetailsController(IEducation_Details education, IWorkExperience experience, IUserSkills userskills, IUserResume resume, IUserCertificate certificate, IUserProjects projects, IJobPreference preference,IUserProfileDetail profile,CloudinaryService cloudinaryService)
        {
            _education = education;
            _experience = experience;
            _userskills = userskills;
            _resume = resume;
            _certificate = certificate;
            _projects = projects;
            _preference = preference;
            _profile = profile;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost]
        [Route("AddEducation")]
        public async Task<IActionResult> AddEducationDetails([FromForm] t_Education_Details t_Education)
        {
            var res = await _education.AddEducationDetails(t_Education);

            if (res == 1)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Education details added successfully",
                    data = res
                });
            }
            else
            {
                return StatusCode(500, new
                {
                    success = false,
                    msg = "Failed to add education details",
                    data = res
                });
            }
        }

        [Route("GetEducation/{id}")]
        [HttpGet]
        public async Task<List<t_Education_Details>> GetEducation_Details(int id)
        {
            List<t_Education_Details> tt = await _education.GetEducation_Details(id);
            return tt;
        }

        [HttpPut("UpdateEducationDetails")]
        public async Task<IActionResult> UpdateEducationDetails([FromForm] t_Education_Details t_Education)
        {
            var res = await _education.UpdateEducationDetails(t_Education);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success update education details",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Error while update education details",
                    data = res
                });
            }
        }

        [HttpDelete("DeleteEducationDetails/{id}")]
        public async Task<IActionResult> DeleteEducationDetails(int id)
        {
            var res = await _education.DeleteEducationDetails(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success delete education details",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Error while delete education details",
                    data = res
                });
            }
        }

        [HttpGet("GetOneEducation/{id}")]
        public async Task<IActionResult> GetOneEducation(int id)
        {
            var res = await _education.GetOneEducation(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "success get one education",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "failed to get one education",
                    data = res
                });
            }
        }

        [HttpPost]
        [Route("AddWorkExperience")]
        public async Task<IActionResult> AddWorkExperience([FromForm] t_Work_Experience experience)
        {
            var res = await _experience.AddWorkExperience(experience);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success add work experience details",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Error while add work experience details",
                    data = res
                });
            }
        }

        [HttpGet("GetWorkExperience/{id}")]
        public async Task<List<t_Work_Experience>> GetWorkExperience(int id)
        {
            var res = await _experience.GetWorkExperience(id);
            return res;
        }

        [HttpPut("UpdateWorkDetail")]
        public async Task<IActionResult> UpdateWork([FromForm] t_Work_Experience experience)
        {
            var res = await _experience.UpdateWorkExperience(experience);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success update work experience details",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Error while update work experience details",
                    data = res
                });
            }
        }

        [HttpDelete("DeleteWorkExperience/{id}")]
        public async Task<IActionResult> DeleteWorkExperience(int id)
        {
            var res = await _experience.DeleteWorkExperience(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success delete work experience details",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Error while delete work experience details",
                    data = res
                });
            }
        }

        [HttpGet("GetOneWorkExperience/{id}")]
        public async Task<IActionResult> GetOneWorkExperience(int id)
        {
            var res = await _experience.GetOneWorkExperience(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "success get one work experience",
                    data = res
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    msg = "failed get one work experience",
                    data = res
                });
            }
        }

        [HttpPost("AddUserSkills")]
        public async Task<IActionResult> AddUserSkills([FromForm] t_UserSkills skills)
        {
            var res = await _userskills.AddUserSkills(skills);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success add user skills details",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Error while add user skills details",
                    data = res
                });
            }
        }

        [HttpGet("GetUserSkills")]
        public async Task<List<t_UserSkills>> GetUserSkills(int id)
        {
            List<t_UserSkills> res = await _userskills.GetUserSkills(id);
            return res;
        }

        [HttpPut("UpdateUserSkills")]
        public async Task<IActionResult> UpdateUserSkills([FromForm] t_UserSkills t_UserSkills)
        {
            var res = await _userskills.UpdateUserSkills(t_UserSkills);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success update user skills details",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Error while update user skills details",
                    data = res
                });
            }
        }

        [HttpDelete("DeleteUserSkills/{id}")]
        public async Task<IActionResult> DeleteUserSkills(int id)
        {
            var res = await _userskills.DeleteUserSkills(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success delete user skills details",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Error while delete user skills details",
                    data = res
                });
            }
        }

        [HttpGet("GetOneSkill/{id}")]
        public async Task<IActionResult> GetOneSkill(int id)
        {
            var res = await _userskills.GetOneSkill(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "success get one user skill",
                    data = res
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    msg = "failed get one user skill",
                    data = res
                });
            }
        }

        [HttpPost("AddUserResume")]
        public async Task<IActionResult> AddUserResume([FromForm] t_UserResume resume)
        {
            if (resume.c_ResumeFile == null || resume.c_ResumeFile.Length == 0)
            {
                return BadRequest(new { success = false, msg = "File upload error" });
            }

            try
            {
                  var fileName = resume.c_user_id+ resume.c_ResumeID + Path.GetExtension(resume.c_ResumeFile.FileName);
                var filePath = Path.Combine("../MVC/wwwroot/user_resume", fileName);
                Directory.CreateDirectory(Path.Combine("../MVC/wwwroot/profile_images"));
                resume.c_ResumeFilePath = fileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    resume.c_ResumeFile.CopyTo(stream);
                }

                // Insert into the database
                var res = await _resume.AddUserResume(resume);

                if (res == 1)
                {
                    return Ok(new { success = true, data = res, msg = "Resume uploaded successfully" });
                }
                else if (res == -1)
                {
                    return Conflict(new { success = false, msg = "User resume already exists" });
                }
                else
                {
                    return BadRequest(new { success = false, msg = "Failed to upload resume" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, msg = "Internal Server Error", error = ex.Message });
            }
        }



        [HttpPut("UpdateUserResume")]
        public async Task<IActionResult> UpdateUserResume(t_UserResume resume)
        {

            if (resume.c_ResumeFile != null && resume.c_ResumeFile.Length > 0)
            {
                var fileName = resume.c_ResumeID + resume.c_user_id + Path.GetExtension(
                 resume.c_ResumeFile.FileName);
                var filePath = Path.Combine("../MVC/wwwroot/user_resume", fileName);
                resume.c_ResumeFilePath = fileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    resume.c_ResumeFile.CopyTo(stream);
                }
                var res = await _resume.UpdateUserResume(resume);
                if (res != null)
                {
                    return Ok(new
                    {
                        success = true,
                        data = res,
                        msg = "Success upload resume"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        data = res,
                        msg = "Failed upload resume"
                    });
                }
            }
            else
            {
                System.Console.WriteLine("Error while resume upload");
                return BadRequest(new
                {
                    msg = "file upload error",
                });
            }
        }

        [HttpDelete("DeleteUserResume/{id}")]
        public async Task<IActionResult> DeleteUserResume(int id)
        {
            var res = await _resume.DeleteUserResume(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    data = res,
                    msg = "success deleted user resume"
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    data = res,
                    msg = "failed deleted user resume"
                });
            }
        }

        [HttpGet("GetUserResume")]
        public async Task<IActionResult> GetUserResume(int id)
        {
            t_UserResume tr = await _resume.GetUserResume(id);
            if (tr != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success getting user resume",
                    data = tr
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "failed getting user resume",
                    data = tr
                });
            }
        }

        [HttpGet("GetOneResume/{id}")]
        public async Task<IActionResult> GetOneResume(int id)
        {
            var res = await _resume.GetOneResume(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "success get one resume",
                    data = res
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    msg = "failed get one resume",
                    data = res
                });
            }
        }

        [HttpPost("AddUserCertificate")]
        public async Task<IActionResult> AddUserCertificate(t_User_Certificate certificate)
        {
            if (certificate.c_certificate != null && certificate.c_certificate.Length > 0)
            {
                var fileName = certificate.c_CertificationID + certificate.c_user_id + Path.GetExtension(
                 certificate.c_certificate.FileName);
                var filePath = Path.Combine("../MVC/wwwroot/user_certificate", fileName);
                certificate.c_CertificateFilePath = fileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    certificate.c_certificate.CopyTo(stream);
                }
                var res = await _certificate.AddUserCertificate(certificate);
                if (res != null)
                {
                    return Ok(new
                    {
                        success = true,
                        msg = "Success add user certificate",
                        data = res
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        msg = "failed add user certificate",
                        data = res
                    });
                }
            }
            else
            {
                System.Console.WriteLine("Error while resume upload");
                return BadRequest(new
                {
                    msg = "file upload error",
                });
            }
        }

        [HttpGet("GetUserCertificate")]
        public async Task<List<t_User_Certificate>> GetUserCertificate(int id)
        {
            List<t_User_Certificate> tt = await _certificate.GetAllUserCertificate(id);
            return tt;
        }

        [HttpPut("UpdateUserCertificate")]
        public async Task<IActionResult> UpdateUserCertificate(t_User_Certificate certificate)
        {
            if (certificate.c_certificate != null && certificate.c_certificate.Length > 0)
            {
                var fileName = certificate.c_CertificationID + certificate.c_user_id + Path.GetExtension(
                 certificate.c_certificate.FileName);
                var filePath = Path.Combine("../MVC/wwwroot/user_certificate", fileName);
                certificate.c_CertificateFilePath = fileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    certificate.c_certificate.CopyTo(stream);
                }
                var res = await _certificate.UpdateUserCertificate(certificate);
                if (res != null)
                {
                    return Ok(new
                    {
                        success = true,
                        msg = "Success update user certificate",
                        data = res
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        msg = "failed update user certificate",
                        data = res
                    });
                }
            }
            else
            {
                System.Console.WriteLine("Error while resume upload");
                return BadRequest(new
                {
                    msg = "file upload error",
                });
            }

        }

        [HttpDelete("DeleteUserCertificate/{id}")]
        public async Task<IActionResult> DeleteUserCertificate(int id)
        {
            var res = await _certificate.DeleteUserCertificate(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    data = res,
                    msg = "success deleted user certificate"
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    data = res,
                    msg = "failed deleted user certificate"
                });
            }
        }

        [HttpGet("GetOneCertificate/{id}")]
        public async Task<IActionResult> GetOneCertificate(int id)
        {
            var res = await _certificate.GetOneCertificate(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "success get one job certificate",
                    data = res
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    msg = "failed get one job certificate",
                    data = res
                });
            }
        }

        [HttpPost("AddUserProjects")]
        public async Task<IActionResult> AddUserProjects([FromForm] t_UserProjects projects)
        {
            var res = await _projects.AddUserProjects(projects);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success add user projects",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "failed add user projects",
                    data = res
                });
            }
        }

        [HttpGet("GetUserProject")]
        public async Task<List<t_UserProjects>> GetUserProjects(int id)
        {
            List<t_UserProjects> tt = await _projects.GetUserProjects(id);
            return tt;
        }

        [HttpPut("UpdateUserProject")]
        public async Task<IActionResult> UpdateUserProject([FromForm] t_UserProjects projects)
        {
            var res = await _projects.UpdateUserProjects(projects);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success update user projects",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "failed update user projects",
                    data = res
                });
            }
        }


        [HttpDelete("DeleteUserProject/{id}")]
        public async Task<IActionResult> DeleteUserProject(int id)
        {
            var res = await _projects.DeleteUserProjects(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success delete user projects",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "failed delete user projects",
                    data = res
                });
            }
        }


        [HttpGet("GetOneProject/{id}")]
        public async Task<IActionResult> GetOneProject(int id)
        {
            var res = await _projects.GetOneProject(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "success get one project",
                    data = res
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    msg = "failed get one project",
                    data = res
                });
            }
        }

        [HttpPost("AddJobPreference")]
        public async Task<IActionResult> AddJobPrefernce([FromForm] t_JobPreference preference)
        {
            Console.WriteLine("Received Data: " + JsonSerializer.Serialize(preference));

            var res = await _preference.AddJobPreference(preference);
            if (res == 1)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Job preference added successfully",
                    data = res
                });
            }
            else if (res == -1)  // Duplicate entry case
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "User already has a job preference record"
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Failed to add job preference"
                });
            }
        }


        [HttpGet("GetJobPreference")]
        public async Task<t_JobPreference> GetJobPreference(int id)
        {
            t_JobPreference tt = await _preference.GetJobPreference(id);
            return tt;
        }

        [HttpPut("UpdateJobPreference")]
        public async Task<IActionResult> UpdateJobPreference([FromForm] t_JobPreference preference)
        {
            var res = await _preference.UpdateJobPreference(preference);
            if (res == 1)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Job preference update successfully",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Failed to update job preference"
                });
            }
        }

        [HttpDelete("DeleteJobPreference/{id}")]
        public async Task<IActionResult> DeleteJobPreference(int id)
        {
            var res = await _preference.DeleteJobPreference(id);
            if (res == 1)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Job preference delete successfully",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Failed to delete job preference"
                });
            }
        }

        [HttpGet("GetOneJobPreference/{id}")]
        public async Task<IActionResult> GetOneJobPreference(int id)
        {
            var res = await _preference.GetOneJobPreference(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "success get one job preference",
                    data = res
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    msg = "failed get one job preference",
                    data = res
                });
            }
        }
    
        [HttpGet("GetUserProfileDetail/{id}")]
        public async Task<t_UserProfileDetail> GetUserProfileDetail(int id){
            t_UserProfileDetail t = await _profile.GetUserProfileDetail(id);
            return t;
        }

        [HttpGet("GetProfilePercentage/{id}")]
        public async Task<t_UserProfileDetail> GetProfilePercentage(int id){
            t_UserProfileDetail t = await _profile.GetUserProfilePercentage(id);
            return t;
        } 

        [HttpPut("Updatepassword")]
        public async Task<IActionResult> Updatepassword([FromForm]t_UpdatePassword updatePassword){
            var res = await _profile.UpdatePassword(updatePassword);
            if(res!=null){
                return Ok(new {
                    success = true,
                    msg = "Success update user password",
                    data = res
                });
            }
            else{   
                return BadRequest(new {
                    success = false,
                    msg = "Failed to update user password",
                    data = res
                });
            }
        } 
    
       [HttpPut("UpdatePersonalDetail")]
        public async Task<IActionResult> UpdatePersonalDetail([FromForm]t_user user)
        {
            if (user.c_image != null && user.c_image.Length > 0)
            {

                if (user.c_image != null && user.c_image.Length > 0)
                {
                    string logoUrl = await UploadProfilePic(user.c_image);
                    if (logoUrl == null) return BadRequest("Only PNG and JPG formats are allowed for the logo.");
                    user.c_profileImage = logoUrl;
                }

                var res = await _profile.UpdatePersonalDetail(user);
                if (res != null)
                {
                    return Ok(new
                    {
                        success = true,
                        msg = "Success update user personal detail",
                        data = res
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        msg = "failed update user personal detail",
                        data = res
                    });
                }
            }
            else
            {
                System.Console.WriteLine("Error while profile image upload");
                return BadRequest(new
                {
                    msg = "file upload error",
                });
            }
        }

         private async Task<string> UploadProfilePic(IFormFile c_image)
        {
            string logoExtension = Path.GetExtension(c_image.FileName).ToLower();
            if (logoExtension != ".png" && logoExtension != ".jpg" && logoExtension != ".jpeg")
                return null;

            using var stream = c_image.OpenReadStream();
            return await _cloudinaryService.UploadImageAsync(stream, c_image.FileName, "ProfileImage");
        }
    
        [HttpGet("GetUserProfile/{id}")]
        public async Task<IActionResult> GetUserProfile(int id){
            var res = await _profile.GetUserProfile(id);
            if(res != null){
                return Ok(new {
                    success = true,
                    msg = "success getting user profile data",
                    data = res
                });

            }
            else{
                return BadRequest(new {
                    success = false,
                    msg = "failed getting user profile data",
                    data = res
                });
            }
        } 
    }
}
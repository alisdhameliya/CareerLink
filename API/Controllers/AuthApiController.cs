using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Repositories.Model;
using StackExchange.Redis;
using RabbitMQ.Client;
using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MailKit.Net.Smtp;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthInterface _authInterface;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthApiController> _logger;
        private readonly IDatabase _redis;
        private readonly IModel _channel;
        private readonly CloudinaryService _cloudinaryService;

        public AuthApiController(IAuthInterface authInterface, ILogger<AuthApiController> logger, IConnectionMultiplexer redis, IModel channel, IConfiguration configuration, CloudinaryService cloudinaryService)
        {
            _authInterface = authInterface ?? throw new ArgumentNullException(nameof(authInterface));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _redis = redis.GetDatabase();
            _channel = channel;
            _cloudinaryService = cloudinaryService;
        }

        //Forgot password changes made here...
        [HttpPost("SendOtp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { success = false, message = "Email is required." });
            }
            int result = await _authInterface.GetUserEmail(request.Email);
            if (result == 0)
            {
                return BadRequest(new { success = false, message = "Email not registered." });
            }
            if (result == -1)
            {
                return BadRequest(new { success = false, message = "Error in fetching the registered email" });
            }
            string otp = new Random().Next(100000, 999999).ToString();
            bool isSet = await _redis.StringSetAsync(request.Email, otp, TimeSpan.FromMinutes(5)); // Store OTP for 5 minutes

            if (!isSet)
            {
                return BadRequest(new { success = false, message = "Failed to store OTP." });
            }

            bool emailSent = await SendOtpEmailAsync(request.Email, otp);
            if (!emailSent)
            {
                return StatusCode(500, new { success = false, message = "Failed to send OTP email." });
            }

            return Ok(new { success = true, message = "OTP sent to your email." });
        }

        private async Task<bool> SendOtpEmailAsync(string email, string otp)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderPassword = _configuration["EmailSettings:SenderPassword"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("CareerLink", senderEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Your OTP Code";

                string emailBody = $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f9f9f9;
                            padding: 0;
                            margin: 0;
                        }}
                        .container {{
                            max-width: 480px;
                            margin: 40px auto;
                            background: #ffffff;
                            padding: 30px;
                            border-radius: 10px;
                            box-shadow: 0px 4px 12px rgba(0, 0, 0, 0.1);
                            text-align: center;
                        }}
                        .header {{
                            font-size: 22px;
                            font-weight: bold;
                            color: #333;
                            margin-bottom: 20px;
                        }}
                        .otp-code {{
                            font-size: 24px;
                            font-weight: bold;
                            color: #007bff;
                            background: #e7f3ff;
                            display: inline-block;
                            padding: 12px 24px;
                            border-radius: 8px;
                            margin: 15px 0;
                        }}
                        .message {{
                            font-size: 16px;
                            color: #555;
                            margin-bottom: 20px;
                        }}
                        .footer {{
                            font-size: 12px;
                            color: #888;
                            border-top: 1px solid #ddd;
                            padding-top: 15px;
                            margin-top: 20px;
                        }}
                        .logo {{
                            width: 200px; /* Adjust width as needed */
                            height: 50px; /* Maintain aspect ratio */
                            margin-bottom: 5px;
                            object-fit:contain;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <img src=""https://res.cloudinary.com/dhruvil20/image/upload/v1743946773/2removebg-preview_x58xrm.png"" alt=""Company Logo"" class=""logo"">
                        <div class='header'>CareerLink OTP Verification</div>
                        <p class='message'>Your One-Time Password (OTP) is:</p>
                        <p class='otp-code'>{otp}</p>
                        <p class='message'>Please enter this code to complete your verification. This OTP is valid for <b>5 minutes</b>.</p>
                        <div class='footer'>
                            <p>If you did not request this, please ignore this email.</p>
                            <p>Best Regards, <br> <b>CareerLink Team</b></p>
                        </div>
                    </div>
                </body>
                </html>";

                message.Body = new TextPart("html")
                {
                    Text = emailBody
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(smtpServer, smtpPort, false);
                    await client.AuthenticateAsync(senderEmail, senderPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }


        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromForm] OtpVerificationRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp))
            {
                return BadRequest(new { success = false, message = "Email and OTP are required." });
            }

            string storedOtp = await _redis.StringGetAsync(request.Email);
            if (storedOtp == request.Otp)
            {
                return Ok(new { success = true, message = "OTP verified." });
            }
            return BadRequest(new { success = false, message = "Invalid or expired OTP." });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordRequest request)
        {
            Console.WriteLine($"Received Email: {request.Email}, NewPassword: {request.NewPassword}");

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest(new { success = false, message = "Email and new password are required." });
            }

            bool updated = await _authInterface.ChangePassword(request.Email, request.NewPassword);
            if (updated)
            {
                return Ok(new { success = true, message = "Password reset successful." });
            }
            return StatusCode(500, new { success = false, message = "Failed to reset password." });
        }
        [HttpGet("GetUserProfile")]
        public IActionResult GetUserProfile()
        {
            var claims = User.Claims.ToList();
            foreach (var claim in claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            var userId = User.FindFirst("uid")?.Value; // "sub" claim holds user ID in JWT

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { success = false, message = "User ID not found in token." });
            }
            return Ok(new { success = true, userId });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] vm_Login login)
        {
            try
            {
                if (login == null || string.IsNullOrEmpty(login.c_email) || string.IsNullOrEmpty(login.c_password) || string.IsNullOrEmpty(login.c_userRole))
                {
                    return BadRequest(new { success = false, message = "Invalid login data." });
                }

                _logger.LogInformation("Login Attempt: {Email}", login.c_email);
                // if(login.c_email=="ritadehrawala3@gmail.com" && login.c_password=="Krishna!1" && login.c_userRole=="Admin")

                t_user user = await _authInterface.Login(login);

                if (user == null)
                {
                    _logger.LogError("Login failed: No user found for email {Email}", login.c_email);
                    return Ok(new { success = false, message = "Invalid email or password, or an incorrect role selection." });
                }

                // if (user.c_userRole != login.c_userRole)
                // {
                //     return Ok(new { success = false, message = $"You are not registered as {login.c_userRole}." });
                // }

                // if (string.IsNullOrEmpty(user.c_password) || !BCrypt.Net.BCrypt.Verify(login.c_password, user.c_password))
                // {
                //     return Ok(new { success = false, message = "Wrong Email or Password." });
                // }

                var token = GenerateJwtToken(user);
                // var userIdClaim = User.FindFirst("UserId")?.Value;// this is just for the person who see the code dont uncomment this
                // Console.WriteLine("UserId in claim: " + userIdClaim);
                return Ok(new { success = true, message = "User logged in successfully.", userId = user.c_userId, userrole = user.c_userRole, token });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Login: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An error occurred while processing your request." });
            }
        }

        private string GenerateJwtToken(t_user user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var claims = new[]
            {

                new Claim("uid", user.c_userId.ToString()),
                new Claim("role", user.c_userRole.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.c_email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string> UploadProfilePic(IFormFile ProfilePic)
        {
            string logoExtension = Path.GetExtension(ProfilePic.FileName).ToLower();
            if (logoExtension != ".png" && logoExtension != ".jpg" && logoExtension != ".jpeg")
                return null;

            using var stream = ProfilePic.OpenReadStream();
            return await _cloudinaryService.UploadImageAsync(stream, ProfilePic.FileName, "ProfileImage");
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] t_user1 user)
        {
            try
            {
                if (user == null) return BadRequest("Invalid user data.");
                _logger.LogInformation("User Registration Attempt: {Email}", user.c_email);

                if (user.ProfilePic != null && user.ProfilePic.Length > 0)
                {
                    string logoUrl = await UploadProfilePic(user.ProfilePic);
                    if (logoUrl == null) return BadRequest("Only PNG and JPG formats are allowed for the logo.");
                    user.c_profileImage = logoUrl;
                }
                var status = await _authInterface.Register(user);
                if (status == 1)
                {
                    var userData = new { Email = user.c_email, Name = user.c_username, Role = user.c_userRole };
                    _redis.StringSet($"User:{user.c_email}", JsonSerializer.Serialize(userData));

                    _channel.QueueDeclare(queue: "UserRegistrationQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    var message = JsonSerializer.Serialize(userData);
                    var body = Encoding.UTF8.GetBytes(message);
                    _channel.BasicPublish(exchange: "", routingKey: "UserRegistrationQueue", basicProperties: null, body: body);

                    _logger.LogInformation("User registration event published to RabbitMQ: {Email}", user.c_email);
                    return Ok(new { success = true, message = "User Registered successfully. Admin will be notified." });
                }
                else if (status == 0)
                {
                    return Ok(new { success = false, message = "User Already Exists" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "An error occurred during registration." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Register: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
    public class OtpRequest
    {
        public string Email { get; set; }
    }

    public class OtpVerificationRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
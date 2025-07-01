using System;
using System.IO;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using Repositories.Interfaces;
using System.Net;
using System.Net.Mail;
using MailKit;


public class EmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUser;
    private readonly string _smtpPass;

    public EmailService()
    {
        _smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? "careerlink2025@gmail.com";
        _smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? "nayv ftgb lewn ylzf";
    }

    public void SendApprovalEmail(string recruiterEmail, string companyName)
    {
        try
        {
            string emailBody = LoadEmailTemplate("ApprovalTemplate.cshtml", companyName, "", "");

            using (var smtpClient = new System.Net.Mail.SmtpClient(_smtpServer, _smtpPort))
            using (var mailMessage = new MailMessage())
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                mailMessage.From = new MailAddress(_smtpUser, "CareerLink");
                mailMessage.To.Add(recruiterEmail);
                mailMessage.Subject = "Recruiter Approved - You Can Now Post Jobs";
                mailMessage.Body = emailBody;
                mailMessage.IsBodyHtml = true;

                smtpClient.Send(mailMessage);
                Console.WriteLine($"✅ Approval email sent to {recruiterEmail}");
            }
        }
        catch (SmtpException smtpEx)
        {
            Console.WriteLine($"❌ SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
        }
    }

    public void SendRejectionEmail(string recruiterEmail, string companyName, string recruiterName, string reason)
    {
        try
        {
            string emailBody = LoadEmailTemplate("RejectionTemplate.cshtml", companyName, recruiterName, reason);

            using (var smtpClient = new System.Net.Mail.SmtpClient(_smtpServer, _smtpPort))
            using (var mailMessage = new MailMessage())
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                mailMessage.From = new MailAddress(_smtpUser, "CareerLink");
                mailMessage.To.Add(recruiterEmail);
                mailMessage.Subject = "Recruiter Application Rejected";
                mailMessage.Body = emailBody;
                mailMessage.IsBodyHtml = true;

                smtpClient.Send(mailMessage);
                Console.WriteLine($"✅ Rejection email sent to {recruiterEmail}");
            }
        }
        catch (SmtpException smtpEx)
        {
            Console.WriteLine($"❌ SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
        }
    }

    private string LoadEmailTemplate(string templateFile, string companyName, string recruiterName, string reason)
    {
        if (!File.Exists(templateFile))
        {
            Console.WriteLine("❌ Email template file not found.");
            return "Email template missing.";
        }

        string templateContent = File.ReadAllText(templateFile);
        templateContent = templateContent.Replace("{{CompanyName}}", companyName)
                                         .Replace("{{RecruiterName}}", recruiterName)
                                         .Replace("{{Reason}}", reason);

        return templateContent;
    }
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using (var smtpClient = new System.Net.Mail.SmtpClient(_smtpServer)
        {
            Port = _smtpPort,
            Credentials = new NetworkCredential(_smtpUser, _smtpPass),
            EnableSsl = true,
        })
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }

    public static implicit operator EmailService(MailService v)
    {
        throw new NotImplementedException();
    }
}

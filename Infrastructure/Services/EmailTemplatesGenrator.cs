using Microsoft.Extensions.Configuration;
using Application.Write.Contracts;

namespace Application.Services
{
    public class EmailTemplateGenerator : IEmailTemplateService
    {
        private readonly IConfiguration _config;

        public EmailTemplateGenerator(IConfiguration config)
        {
            _config = config;
        }

        public string GetForgotPasswordTemplate(string userName, string token)
        {
            var linkTemplate = _config["EmailLinks:PasswordReset"];
            var finalLink = string.Format(linkTemplate!, token);

            return $@"
            <div style='font-family: sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px;'>
                <h2 style='color: #2d3436; text-align: center;'>HR System</h2>
                <h3>Hello {userName},</h3>
                <p>Click the button below to reset your password:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{finalLink}' style='background-color: #0984e3; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px;'>Reset Password</a>
                </div>
                <p style='color: #636e72; font-size: 12px;'>If you didn't request this, ignore this email.</p>
                <p style='border-top: 1px solid #eee; padding-top: 20px; text-align: center;'>© {DateTime.Now.Year} HR System</p>
            </div>";
        }

        public string GetWelcomeTemplate(string userName, string token)
        {
            var linkTemplate = _config["EmailLinks:AccountActivation"];
            var fullLink = string.Format(linkTemplate!, token);
        
            return $@"
            <div style='font-family: sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px;'>
                <h2 style='color: #00b894; text-align: center;'>Welcome to HR System</h2>
                <p>Hi {userName},</p>
                <p>Your account has been created successfully. You can now log in:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{fullLink}' style='background-color: #00b894; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px;'>Login Now</a>
                </div>
            </div>";
        }
        
            
        public string GetFreezeAdminRequestTemplate(string adminName, string targetName, string? reason, string requesterName, int actionId)
        {
            var linkTemplate = _config["EmailLinks:PendingActionDetails"];
            var fullLink = string.Format(linkTemplate!, actionId);

            return $@"
            <div style='font-family: sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px;'>
                <h2 style='color: #d63031; text-align: center;'>Freeze Admin Request</h2>
                <p>Hi {adminName},</p>
                <p><strong>{requesterName}</strong> has requested to freeze the admin account of <strong>{targetName}</strong> for the following reason:</p>
                <blockquote style='background-color: #f1f1f1; padding: 15px; border-left: 5px solid #d63031;'>{reason}</blockquote>
                <p>Click the button below to review and take action:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{fullLink}' style='background-color: #d63031; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px;'>Review Request</a>
                </div>
            </div>";
        }

        public string GetReavtivateAdminRequestTemplate(string adminName, string targetName, string? reason, string requesterName, int actionId)
        {
            var linkTemplate = _config["EmailLinks:PendingActionDetails"];
            var fullLink = string.Format(linkTemplate!, actionId);

            return $@"
            <div style='font-family: sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px;'>
                <h2 style='color: #00b894; text-align: center;'>Reactivate Admin Request</h2>
                <p>Hi {adminName},</p>
                <p><strong>{requesterName}</strong> has requested to reactivate the admin account of <strong>{targetName}</strong> for the following reason:</p>
                <blockquote style='background-color: #f1f1f1; padding: 15px; border-left: 5px solid #00b894;'>{reason}</blockquote>
                <p>Click the button below to review and take action:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{fullLink}' style='background-color: #00b894; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px;'>Review Request</a>
                </div>
            </div>";
        }
    }
}
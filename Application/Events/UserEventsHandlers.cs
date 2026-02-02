using Application.Write.CommandHandlers;
using Application.Write.Contracts;
using Domain.Entites;
using Application.Write.Commands;

namespace Application.Events
{
    public class CrateNewUserEventHandler
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateGenerator;

        public CrateNewUserEventHandler(IEmailService emailService, IEmailTemplateService emailTemplateGenerator)
        {
            _emailService = emailService;
            _emailTemplateGenerator = emailTemplateGenerator;
        }


        private async Task SendEmail(string userName, string email, string token)
        {
             if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userName)) return;
            
            var body = _emailTemplateGenerator.GetWelcomeTemplate(userName, token);
            await _emailService.SendEmailAsync(email, "Welcome to HR System", body);
        }


        public async Task Invoke(string userName, string email, string token)
        {
           await SendEmail(userName, email, token);
        }
    }



    public class ResetPasswordEventHandler
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateGenerator;

        public ResetPasswordEventHandler(IEmailService emailService, IEmailTemplateService emailTemplateGenerator)
        {
            _emailService = emailService;
            _emailTemplateGenerator = emailTemplateGenerator;
        }


        private async Task SendEmail(string email, string token, string userName)
        {
             if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userName)) return;
            
            var body = _emailTemplateGenerator.GetForgotPasswordTemplate(userName, token);
            await _emailService.SendEmailAsync(email, "Reset Password in HR System", body);
        }


        public async Task Invoke(string userName, string email, string token) => await SendEmail(userName, email, token);
    }


    public class NewPendingAdminActionCreatedEventhandler
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateGenerator;

        public NewPendingAdminActionCreatedEventhandler(IEmailService emailService, IEmailTemplateService emailTemplateGenerator)
        {
            _emailService = emailService;
            _emailTemplateGenerator = emailTemplateGenerator;
        }

        private async Task SendEmail(string adminEmail, string adminName, string targetName, string? reason, string requesterName, int actionId, enAdminActionType actionType)
        {
            if(string.IsNullOrEmpty(adminName) || string.IsNullOrEmpty(adminEmail) || actionId <= 0 || string.IsNullOrEmpty(requesterName)) return;
            
            string body;
            string subject;

            switch (actionType)
            {
                case enAdminActionType.FreezeAdmin:
                    body = _emailTemplateGenerator.GetFreezeAdminRequestTemplate(adminName, targetName, reason, requesterName, actionId);
                    subject = "Freeze Admin Request in HR System";
                    break;

                case enAdminActionType.ReactivateAdmin:
                    body = _emailTemplateGenerator.GetReavtivateAdminRequestTemplate(adminName, targetName, reason, requesterName, actionId);
                    subject = "Re-activate Admin Request in HR System";
                    break;
                    
                default:
                    return;
            }

            await _emailService.SendEmailAsync(adminEmail,subject, body);
        }

        
        public async Task Invoke(IEnumerable<User> admins, string targetName, string? reason, string requesterName, int actionId, enAdminActionType actionType)
        {
            foreach (var admin in admins)
            {
                string email = admin.Employee!.Emails.FirstOrDefault(e => e.IsPrimary)!.Value.Value;
                await SendEmail(email, admin.UserName, targetName, reason, requesterName, actionId, actionType);
            }
        }   
    }




    public class ApproveFreezAdminEventHandler
    {
        private readonly UserHandler _userHandler;

        public ApproveFreezAdminEventHandler(UserHandler userHandler)
        {
            _userHandler = userHandler;
        }

        public async Task Invoke(int adminId, string approvedBy)
        {
            await _userHandler.FreezeHandle(adminId);
        }
    }


    public class ApproveReactivateAdminEventHandler
    {
        private readonly UserHandler _userHandler;

        public ApproveReactivateAdminEventHandler(UserHandler userHandler)
        {
            _userHandler = userHandler;
        }

        public async Task Invoke(int adminId, string approvedBy)
        {
            await _userHandler.ActivateHandle(adminId);
        }
    }
}

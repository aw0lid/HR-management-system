using Domain.Entites;

namespace Application.Write.Contracts
{
    public interface ITokenService
    {
        public string GenerateJwtToken(User user);
        public (string PlainToken, string HashedToken) CreateSecureToken();
        public string HashToken(string plainToken);
    }

    public interface IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public interface IEmailTemplateService
    {
        public string GetForgotPasswordTemplate(string userName, string token);
        public string GetWelcomeTemplate(string userName, string token);
        public string GetFreezeAdminRequestTemplate(string adminName, string targetName, string? reason, string requesterName, int actionId);
        public string GetReavtivateAdminRequestTemplate(string adminName, string targetName, string? reason, string requesterName, int actionId);
    }
}
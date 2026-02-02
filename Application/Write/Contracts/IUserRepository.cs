using Domain.Entites;

namespace Application.Write.Contracts
{

    public class EmployeeValidationResult
    {
        public int Id { get; set; }
        public string JobCode { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsUser { get; set; }
        public bool UserNameExists { get; set; }
    }


    public interface IUserRepository : IGenericRepository<User>
    {
        Task<bool> IsUserNameExistsAsync(string Username);
        Task<EmployeeValidationResult?> GetEmployeeValidationStatus(int id, string username);
        Task<User?> GetUserByUserNameAsync(string userName);
        Task<(string? Email, User? user)> GetUserWithEmailByUserNameAsync(string userName);
        Task<Token?> GetToken(string refToken);
    }
}
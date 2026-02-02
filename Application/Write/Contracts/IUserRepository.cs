using Domain.Entites;
using Domain.ValueObjects;

namespace Application.Write.Contracts
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<bool> IsUserNameExistsAsync(string Username);
        Task<(bool Exists, bool IsHr, bool IsUser, bool UserNameExists)> GetEmployeeValidationStatus(int id);
        Task<User> GetUserByUserName(string userName);
        Task<bool> AddNewLogAsync(UserLog log);
    }
}
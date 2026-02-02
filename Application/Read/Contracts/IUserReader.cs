using Domain.Entites;
using Read.ViewModels;

namespace Read.Contracts
{
    public interface IUserReader
    {
        Task<UserView?> GetUserByIdAsync(int id);
        Task<UserView?> GetUserByCodeAsync(string code);
        Task<UserView?> GetUserByUserNameAsync(string UserName);
        Task<UserView?> GetUserByNationalNumberAsync(string nationalNumber);
        Task<IEnumerable<UserView>> GetAllUsersAsync(int PageNumber, int Size);
        Task<IEnumerable<PendingAdminAction>> GetPendingAdminsActions();
    }
}
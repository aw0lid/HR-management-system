using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Read.Contracts;
using Read.ViewModels;
using System.Data;

namespace Infrastructure.Queries
{
    public class UserQuerier : IUserReader
    {

        private readonly HRDBContext _context;
        public UserQuerier(HRDBContext context) => _context = context;

        

        public async Task<UserView?> GetUserByIdAsync(int id)
        {
            return _context.Database.SqlQueryRaw<UserView>("EXEC sp_GetUser @Id = {0}", id)
                        .AsEnumerable().FirstOrDefault();
        }

        public async Task<UserView?> GetUserByCodeAsync(string code)
        {
            return _context.Database
                .SqlQueryRaw<UserView>("EXEC sp_GetUser @Code = {0}", code)
                .AsEnumerable()
                .FirstOrDefault();
        }

        public async Task<UserView?> GetUserByUserNameAsync(string UserName)
        {
            return _context.Database
                .SqlQueryRaw<UserView>("EXEC sp_GetUser @UserName = {0}", UserName)
                .AsEnumerable()
                .FirstOrDefault();
        }

        public async Task<UserView?> GetUserByNationalNumberAsync(string nationalNumber)
        {
            return _context.Database
                .SqlQueryRaw<UserView>("EXEC sp_GetUser @NationalNumber = {0}", nationalNumber)
                .AsEnumerable()
                .FirstOrDefault();
        }

        public async Task<IEnumerable<UserView>> GetAllUsersAsync(int PageNumber, int Size)
        {
            return await _context.Database
                .SqlQueryRaw<UserView>("EXEC sp_GetUserPaged @PageNumber = {0}, @PageSize = {1}", PageNumber, Size)
                .ToListAsync();
        }

        public async Task<IEnumerable<PendingAdminAction>> GetPendingAdminsActions()
        {
            return await _context.PendingAdminActions
                .Where(p => p.Status == enAdminActionStatus.Pending)
                .ToListAsync();
        }
    }
}
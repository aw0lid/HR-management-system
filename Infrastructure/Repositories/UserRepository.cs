using Application.Write.Contracts;
using Domain.Entites;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HRDBContext _context;

        public UserRepository(HRDBContext context)
        {
            _context = context;
        }



        public async Task<bool> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetByIdAsync(int Id)
        {
            return await _context.Users.FindAsync(Id);
        }

        public async Task<bool> IsUserNameExistsAsync(string Username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == Username);
        }



        public async Task<(bool Exists, bool IsHr, bool IsUser, bool UserNameExists)> GetEmployeeValidationStatus(int id)
        {
            var status = await _context.Employees
                .Where(e => e.EmployeeId == id)
                .Select(e => new
                {
                    Exists = true,
                    IsHr = e.WorkInfoHistory.Any(ew => ew.IsCurrent && ew.Department.DepartmentCode == "HR"),
                    IsUser = _context.Users.Any(u => u.EmployeeId == id)
                })
                .FirstOrDefaultAsync();

            if (status == null)
                return (false, false, false, false);

            return (status.Exists, status.IsHr, status.IsUser, false); 
        }

       public async Task<User?> GetUserByUserName(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> AddNewLogAsync(UserLog log)
        {
            await _context.UserLogs.AddAsync(log);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
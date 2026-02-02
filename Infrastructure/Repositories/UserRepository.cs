using Application.Write.Contracts;
using Domain.Entites;
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

       

        public async Task<Token?> GetToken(string refToken)
        {
            return await _context.Tokens.Include(t => t.User.Tokens).FirstOrDefaultAsync(t => t.value == refToken);
        }

        public async Task<User?> GetByIdAsync(int Id)
        {
            return await _context.Users.Include(u => u.Tokens).FirstOrDefaultAsync(u => u.UserId == Id);
        }

        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users.Include(u => u.Tokens).FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<(string? Email, User? user)> GetUserWithEmailByUserNameAsync(string userName)
        {
            var result = await _context.Users
                .Where(u => u.UserName == userName).Include(u => u.Tokens)
                .Select(u => new 
                {
                    UserObject = u,
                    PrimaryEmailValue = u.Employee.Emails
                                        .Where(e => e.IsPrimary)
                                        .Select(e => e.Value.Value)
                                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            return (result?.PrimaryEmailValue, result?.UserObject);
        }


        public async Task<bool> IsUserNameExistsAsync(string Username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == Username);
        }



       public async Task<EmployeeValidationResult?> GetEmployeeValidationStatus(int id, string username)
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.EmployeeId == id)
                .Select(e => new EmployeeValidationResult
                {
                    Id = e.EmployeeId,
                    JobCode = e.WorkInfoHistory.Where(w => w.IsCurrent).Select(w => w.JobTitleLevel.JobTitle.JobTitleCode).FirstOrDefault()!,
                    Email = e.Emails.Where(em => em.IsPrimary).Select(em => em.Value.Value).FirstOrDefault()!,
                    IsUser = _context.Users.Any(u => u.EmployeeId == id),
                    UserNameExists = _context.Users.Any(u => u.UserName == username)
                })
                .FirstOrDefaultAsync();
        }
    }
}
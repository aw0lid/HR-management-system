using Application.Write.Contracts;
using Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly HRDBContext _context;
        public EmployeeRepository(HRDBContext context) => _context = context;

      


        public async Task<bool> AddAsync(Employee entity)
        {
            _context.Employees.Add(entity);
            await _context.SaveChangesAsync();
            return true; 
        }

      
        public Task<bool> UpdateAsync(Employee entity)
        {
            _context.Employees.Update(entity);
            _context.SaveChanges();
            return Task.FromResult(true);
        }




        public async Task<(bool nationalIdExists, bool phoneExists, bool emailExists)> CheckConstraintsAsync(string nationalId, string phone, string email)
        {
            var nationalIdExists = await _context.Employees.AnyAsync(e => e.EmployeeNationalNumber.Value == nationalId);
            var phoneExists = await _context.EmployeePhones.AnyAsync(p => p.Value.Value == phone);
            var emailExists = await _context.EmployeeEmails.AnyAsync(e => e.Value.Value == email);

            return (nationalIdExists, phoneExists, emailExists);
        }



        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.WorkInfoHistory.Where(w => w.IsCurrent))
                    .ThenInclude(w => w.Manager)
                        .ThenInclude(m => m.WorkInfoHistory.Where(mw => mw.IsCurrent))
                .Include(e => e.WorkInfoHistory.Where(w => w.IsCurrent))
                    .ThenInclude(w => w.Department)
                .Include(e => e.WorkInfoHistory.Where(w => w.IsCurrent))
                    .ThenInclude(w => w.JobTitleLevel)
                        .ThenInclude(j => j.JobGrade)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

    


        public async Task<Employee?> GetWithEmails(int Id)
        {
            return await _context.Employees
                .Include(e => e.Emails)
                .Include(e => e.WorkInfoHistory.Where(w => w.IsCurrent))
                .FirstOrDefaultAsync(e => e.EmployeeId == Id)
                ?? null;
        }

       
        public async Task<Employee?> GetWithPhones(int Id)
        {
            return await _context.Employees
                .Include(e => e.Phones)
                .Include(e => e.WorkInfoHistory.Where(w => w.IsCurrent))
                .FirstOrDefaultAsync(e => e.EmployeeId == Id)
                ?? null;
        }

       

        public async Task<IEnumerable<Employee>> GetAllManagersAsync(int Id)
        {
          
            return await _context.EmployeeWorkInfos
                .Where(w => w.IsCurrent && w.ManagerId != null)
                .Select(w => w.Manager!)
                .Distinct()
                .AsNoTracking()
                .ToListAsync();
        }

      
        public async Task<bool> IsEmailExistsAsync(string email) => await _context.EmployeeEmails.AnyAsync(e => e.Value.Value == email);
        public async Task<bool> IsNationalNumberExistsAsync(string nationalNumber) => await _context.Employees.AnyAsync(e => e.EmployeeNationalNumber.Value == nationalNumber);
        public async Task<bool> IsPhoneNumberExistsAsync(string phoneNumber) => await _context.EmployeePhones.AnyAsync(p => p.Value.Value == phoneNumber);
    }
}
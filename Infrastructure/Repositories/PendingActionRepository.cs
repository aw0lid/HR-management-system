using Application.Write.Contracts;
using Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PendingActionRepository : IPendingActionsRepository
    {
        private readonly HRDBContext _context;

        public PendingActionRepository(HRDBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(PendingAdminAction pendingAction)
        {
            await _context.PendingAdminActions.AddAsync(pendingAction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(PendingAdminAction pendingAction)
        {
            _context.PendingAdminActions.Update(pendingAction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PendingAdminAction?> GetByIdAsync(int id)
        {
            return await _context.PendingAdminActions.FindAsync(id);
        }

        public async Task<(IEnumerable<User> admins, bool RequestIsValid)> GetAllAdminsWithEmailsAndCheckIfValidRequest(int targetId)
        {
            var data = await _context.Users
                .Select(_ => new
                {
                    Admins = _context.Users
                        .Include(u => u.Employee!.Emails)
                        .Where(u => u.Role == enRole.SystemAdmin)
                        .ToList(),

                    HasPendingAction = _context.PendingAdminActions
                        .Any(pa => pa.TargetUserId == targetId && pa.Status == enAdminActionStatus.Pending)
                })
                .FirstOrDefaultAsync();

            if (data == null) return (Enumerable.Empty<User>(), true);
            return (data.Admins, !data.HasPendingAction);
        }

       public async Task<(PendingAdminAction? action, User? procceser)> GetActionAndUserProcceser(int actionId, int userId)
        {
            var result = await _context.PendingAdminActions
                .Select(_ => new
                {
                    Action = _context.PendingAdminActions.FirstOrDefault(a => a.ActionId == actionId),
                    User = _context.Users.FirstOrDefault(u => u.UserId == userId)
                })
                .FirstOrDefaultAsync();

            return (result?.Action, result?.User);
        }
    }
}
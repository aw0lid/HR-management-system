using Domain.Entites;

namespace Application.Write.Contracts
{
    public interface IPendingActionsRepository : IGenericRepository<PendingAdminAction>
    {
        Task<(IEnumerable<User> admins, bool RequestIsValid)> GetAllAdminsWithEmailsAndCheckIfValidRequest(int targetId);
        Task<(PendingAdminAction? action, User? procceser)> GetActionAndUserProcceser(int actionId, int userId);
    }
}
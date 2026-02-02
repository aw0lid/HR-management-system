using SharedKernal;
using static Domain.ErrorsMenu.ErrorsMenu.User;


namespace Domain.Entites
{
    public enum enAdminActionType : byte {FreezeAdmin = 1, ReactivateAdmin = 2}
    public enum enAdminActionStatus : byte {Pending = 1, Approved = 2, Canceled = 3}



    public class PendingAdminAction
    {
        public int ActionId { get; private set; }
        public enAdminActionType ActionType { get; private set; }
        
        public int RequestedBy { get; private set; }
        public virtual User Requestor { get; private set; } = null!;

        public int? ProcessedBy { get; private set; }
        public User? Processor { get; private set; }

        public int TargetUserId { get; private set; }
        public User? TargetUser { get; private set; }

        public DateTime RequestedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; private set; }
        
        public enAdminActionStatus Status { get; private set; }
        public string? RequestReason { get; private set; }
        public string? ProcessNotes { get; private set; }




        private PendingAdminAction() { }

        private PendingAdminAction(enAdminActionType actionType, int requestedBy, int targetUserId, string? reason = null)
        {
            if(requestedBy <= 0) throw new ArgumentException(nameof(requestedBy));
            if(targetUserId <= 0) throw new ArgumentException(nameof(targetUserId));

            ActionType = actionType;
            RequestedAt = DateTime.UtcNow;
            RequestedBy = requestedBy;
            TargetUserId = targetUserId;
            RequestReason = reason;
            Status = enAdminActionStatus.Pending;
        }


        public static PendingAdminAction CreateFreezeAdminAction(int requestedBy, int targetUserId, string? reason = null)
            => new PendingAdminAction(enAdminActionType.FreezeAdmin, requestedBy, targetUserId, reason);

        public static PendingAdminAction CreateReactivateAdminAction(int requestedBy, int targetUserId, string? reason = null)
            => new PendingAdminAction(enAdminActionType.ReactivateAdmin, requestedBy, targetUserId, reason);



        public Result<PendingAdminAction> Approve(int ProcessedById)
        {
            if (Status != enAdminActionStatus.Pending) return Result<PendingAdminAction>.Failure(ActionNotPending);
            if (RequestedBy == ProcessedById) return Result<PendingAdminAction>.Failure(CannotApproveOwnAction);
            if (ProcessedById <= 0) return Result<PendingAdminAction>.Failure(InvalidProcessorId);
            
            Status = enAdminActionStatus.Approved;
            ProcessedBy = ProcessedById;
            ProcessedAt = DateTime.UtcNow;

            return Result<PendingAdminAction>.Successful(this);
        }


        public Result<PendingAdminAction> Cancel()
        {
            if (Status != enAdminActionStatus.Pending)
                return Result<PendingAdminAction>.Failure(ActionNotPending);

            Status = enAdminActionStatus.Canceled;
            return Result<PendingAdminAction>.Successful(this);
        }
    }
}
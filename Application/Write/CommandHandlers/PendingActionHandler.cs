using Application.Write.Commands;
using Application.Write.Contracts;
using Domain.Entites;
using SharedKernal;
using static Application.ErrorsMenu.ApplicationErrorsMenu.UserHandlersErrors;
using Application.Events;

namespace Application.Write.CommandHandlers
{
    public class PendingActionshandler
    {
        private readonly IPendingActionsRepository _pendingActionsRepository;
        private readonly NewPendingAdminActionCreatedEventhandler _newPendingAdminActionCreatedEventhandler;
        private readonly ApproveFreezAdminEventHandler _approveFreezeAdminEventHandler;
        private readonly ApproveReactivateAdminEventHandler _approveReactivateAdminEventHandler;

        public PendingActionshandler(IPendingActionsRepository pendingActionsRepository, NewPendingAdminActionCreatedEventhandler newPendingAdminActionCreatedEventhandler, 
                                ApproveFreezAdminEventHandler approveFreezeAdminEventHandler, 
                                ApproveReactivateAdminEventHandler approveReactivateAdminEventHandler )
        {
            _pendingActionsRepository = pendingActionsRepository;
            _newPendingAdminActionCreatedEventhandler = newPendingAdminActionCreatedEventhandler; 
            _approveFreezeAdminEventHandler = approveFreezeAdminEventHandler;
            _approveReactivateAdminEventHandler = approveReactivateAdminEventHandler;
        }




        private async Task<Result<bool>> FreezAndReavtivateAdminRequestHandle(int TargetUserId, string? reason, int requestedBy, bool FreezRequest = true)
        {
            var Result = await _pendingActionsRepository.GetAllAdminsWithEmailsAndCheckIfValidRequest(TargetUserId);
            
            if(!Result.RequestIsValid) return Result<bool>.Failure(TargetHasAnotherPendingAction(TargetUserId));

            var AllAdmins = Result.admins;
            var requestedByUser = AllAdmins.FirstOrDefault(a => a.UserId == requestedBy);

            if (requestedByUser == null || requestedByUser.Role != enRole.SystemAdmin) return Result<bool>.Failure(UserNotAdmin(requestedBy));
            if(requestedByUser.status != enStatus.Active) return Result<bool>.Failure(UserNotActive(requestedBy));

            var target = AllAdmins.FirstOrDefault(a => a.UserId == TargetUserId);
            if (target == null) return Result<bool>.Failure(UserNotFound(TargetUserId));
            if (target.Role != enRole.SystemAdmin) return Result<bool>.Failure(UserNotAdmin(TargetUserId));

            if (target.UserId == requestedBy) return Result<bool>.Failure(CannotPerformActionOnSelf());

            PendingAdminAction pendingAction = default!;

            if (FreezRequest)
            {
                if(target.status != enStatus.Active) return Result<bool>.Failure(UserNotActive(TargetUserId));

                var activeAdminsCount = AllAdmins.Count(a => a.status == enStatus.Active && a.UserId != target.UserId);
                if (activeAdminsCount < 3) return Result<bool>.Failure(CannotFreezeLastAdmins());

                pendingAction = PendingAdminAction.CreateFreezeAdminAction(requestedBy, TargetUserId, reason);
            }

            else
            {
                if(target.status != enStatus.Frozen) return Result<bool>.Failure(UserActive(TargetUserId));
                pendingAction = PendingAdminAction.CreateReactivateAdminAction(requestedBy, TargetUserId, reason);
            } 



            await _pendingActionsRepository.AddAsync(pendingAction);

            if (pendingAction.ActionId != default)
            {
                _ = Task.Run(async () => 
                {
                    var adminsToNotify = AllAdmins.Where(u => u.UserId != target.UserId && u.UserId != requestedBy).ToList();
                    enAdminActionType type = FreezRequest ? enAdminActionType.FreezeAdmin : enAdminActionType.ReactivateAdmin;

                    await  _newPendingAdminActionCreatedEventhandler.Invoke(
                        adminsToNotify, 
                        target.UserName, 
                        reason, 
                        requestedByUser.UserName, 
                        pendingAction.ActionId, 
                        type);
                });
            }
            return Result<bool>.Successful(true);
        }

       




       public async Task<Result<bool>> FreezAdminRequestHandle(AdminFreezeCommand command, int requestedBy)
        {
            return await FreezAndReavtivateAdminRequestHandle(command.TargetUserId, command.reason, requestedBy);
        }

        
    
        public async Task<Result<bool>> ReavtivateAdminRequestHandle(AdminReactivateCommand command, int requestedBy)
        {
            return await FreezAndReavtivateAdminRequestHandle(command.TargetUserId, command.reason, requestedBy, false);
        }






       public async Task<Result<bool>> ResponseAdminActionHandle(ResponseAdminActionCommand command, int proccedBy)
        {
            var reposiotry = await _pendingActionsRepository.GetActionAndUserProcceser(command.ActionId, proccedBy);
            var action = reposiotry.action;
            var procceser = reposiotry.procceser;


            if (action == null) return Result<bool>.Failure(PendingActionNotFound(command.ActionId));
            if (action.Status != enAdminActionStatus.Pending) return Result<bool>.Failure(PendingActionAlreadyProcessed(command.ActionId));

            if (procceser == null) return Result<bool>.Failure(UserNotFound(proccedBy));
            if (procceser.Role != enRole.SystemAdmin) return Result<bool>.Failure(UserNotAdmin(proccedBy));

            if(procceser.UserId == action.TargetUserId) return Result<bool>.Failure(AdminTargetProcessHisAction());

            if((enAdminActionStatus)command.response == enAdminActionStatus.Approved)
            {
                if(procceser.UserId == action.RequestedBy) return Result<bool>.Failure(AdminApproveHisAction());
                action.Approve(proccedBy);
            }

            else if((enAdminActionStatus)command.response == enAdminActionStatus.Canceled) action.Cancel();
            

            await _pendingActionsRepository.UpdateAsync(action);

            switch (action.ActionType)
            {
                case enAdminActionType.FreezeAdmin:
                    await _approveFreezeAdminEventHandler.Invoke(action.TargetUserId!, procceser.UserName);
                    break;

                case enAdminActionType.ReactivateAdmin:
                    await _approveReactivateAdminEventHandler.Invoke(action.TargetUserId!, procceser.UserName);
                    break;
            }

            return Result<bool>.Successful(true);
        }
    }
}
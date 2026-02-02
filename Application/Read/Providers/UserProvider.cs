using Application.Cache;
using Domain.Entites;
using Read.Contracts;
using Read.ViewModels;
using SharedKernal;
using static Application.ErrorsMenu.ApplicationErrorsMenu.UsersQueriesErrors;

namespace Read.Providers
{
    public class UserProvider
    {
        private readonly IUserReader _userReader;

        public UserProvider(IUserReader userReader)
        {
            _userReader = userReader;
        }


        public async Task<Result<UserView>> GetUserById(int id)
        {
            var view = await _userReader.GetUserByIdAsync(id);
            return view == null ? Result<UserView>.Failure(UserNotFound()) : Result<UserView>.Successful(view);
        }

        public async Task<Result<UserView>> GetUserByCode(string code)
        {
            var view = await _userReader.GetUserByCodeAsync(code);
            return view == null ? Result<UserView>.Failure(UserNotFound()) : Result<UserView>.Successful(view);
        }

        public async Task<Result<UserView>> GetUserByNationalNumber(string NationalNumber)
        {
            var view = await _userReader.GetUserByNationalNumberAsync(NationalNumber);
            return view == null ? Result<UserView>.Failure(UserNotFound()) : Result<UserView>.Successful(view);
        }

        public async Task<Result<UserView>> GetUserByUserName(string UserName)
        {
            var view = await _userReader.GetUserByUserNameAsync(UserName);
            return view == null ? Result<UserView>.Failure(UserNotFound()) : Result<UserView>.Successful(view);
        }


        public async Task<Result<List<UserView>>> GetAllUsers(int Page, int size)
        {
            var view = await _userReader.GetAllUsersAsync(Page, size);
            return view == null ? Result<List<UserView>>.Failure(UsersEmpty()) : Result<List<UserView>>.Successful(view.ToList());
        }

        public async Task<Result<List<PendingAdminAction>>> GetPendingAdminsActions()
        {
            var view = await _userReader.GetPendingAdminsActions();
            return view == null ? Result<List<PendingAdminAction>>.Failure(PendingAdminsActionsEmpty()) : Result<List<PendingAdminAction>>.Successful(view.ToList());
        }
    }
}
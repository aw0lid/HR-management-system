using Application.Write.Commands;
using Application.Write.Contracts;
using Domain.Entites;
using Domain.ValueObjects;
using SharedKernal;
using Application.Events;
using static Application.ErrorsMenu.ApplicationErrorsMenu.UserHandlersErrors;


namespace Application.Write.CommandHandlers
{
    public record AuthResponse(string JWT, string RefreshToken, DateTime RefreshTokenExpTime);
    public class AuthHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokensGentator;
        private readonly ResetPasswordEventHandler _resetPasswordEventHandler;

        public AuthHandler(IUserRepository userRepository, ITokenService tokensGentator, ResetPasswordEventHandler resetPasswordEventHandler)
        {
            _userRepository = userRepository;
            _tokensGentator = tokensGentator;
            _resetPasswordEventHandler = resetPasswordEventHandler;
        }





        public async Task<Result<AuthResponse>> LoginHandle(LoginCommand command)
        {
            var user = await _userRepository.GetUserByUserNameAsync(command.userName);

            if (user == null || !BCrypt.Net.BCrypt.Verify(command.password, user.UserPasswordHashing!.Value)) 
                return Result<AuthResponse>.Failure(UserLoginInvalid());

            if (user.status != enStatus.Active) return Result<AuthResponse>.Failure(UserNotActive(user.UserName));

            var JWT = _tokensGentator.GenerateJwtToken(user);
            

            var values = _tokensGentator.CreateSecureToken();
            var newRefreshToken = Token.CreateRefreshToken(values.HashedToken, user);

            user.ExecuteLogin(newRefreshToken);
            await _userRepository.UpdateAsync(user);

            var response = new AuthResponse(JWT, values.PlainToken, newRefreshToken.ExpiresAt);
            return Result<AuthResponse>.Successful(response);
        }




        public async Task<Result<AuthResponse>> RefreshUserTokenHandle(string oldTokenValue)
        {
            var HashedToken = _tokensGentator.HashToken(oldTokenValue);
            
            var OldToken = await _userRepository.GetToken(HashedToken);
            if(OldToken == default) return Result<AuthResponse>.Failure();

             var user = OldToken.User;

            if (!OldToken.IsActive || OldToken.Type != enTokenType.RefreshToken)
            {
                user.Logout();
                await _userRepository.UpdateAsync(user);
                return Result<AuthResponse>.Failure();
            }

            var TokenValue = _tokensGentator.CreateSecureToken();
            var NewToken = Token.CreateRefreshToken(TokenValue.HashedToken, user);
            var JWT = _tokensGentator.GenerateJwtToken(OldToken.User!);

            user!.RotateToken(OldToken.TokenId, NewToken);
            await _userRepository.UpdateAsync(user);
            return Result<AuthResponse>.Successful(new(JWT, TokenValue.PlainToken, NewToken.ExpiresAt));
        }

        public async Task<Result<bool>> LogoutHandle(string RefToken)
        {
            var HashedToken = _tokensGentator.HashToken(RefToken);
            var token = await _userRepository.GetToken(HashedToken);
            if(token == default)  return Result<bool>.Failure();

            var user = token.User;
            user.Logout();
                
            await _userRepository.UpdateAsync(user);
            return Result<bool>.Successful(true);
        }

        public async Task<Result<bool>> ActivateNewUserHandle(ActivateUserCommand command)
        {
            var HashedToken = _tokensGentator.HashToken(command.Token);
            var token = await _userRepository.GetToken(HashedToken);
            if(token == default) return Result<bool>.Failure();

            var user = token.User;
            if(user!.UserName != command.Username) return Result<bool>.Failure(InvalidUserName());

            if (!token.IsActive || 
                token.Type != enTokenType.AccountActivation ||
                user.status != enStatus.PendingActivation)
            {
                user.Logout();
                await _userRepository.UpdateAsync(user);
                return Result<bool>.Failure();
            }

            var PasswordCreationResult = Password.Create(command.NewPassword);
            if(!PasswordCreationResult.IsSuccess) return Result<bool>.Failure(PasswordCreationResult.Error);

            var AccountActivationResult = user.CompleteSetup(PasswordCreationResult.Value!);
            if(!AccountActivationResult.IsSuccess) return Result<bool>.Failure(AccountActivationResult.Error);
           
            token.Revoke();
            
            await _userRepository.UpdateAsync(user);
            return Result<bool>.Successful(true);
        }


       


        public async Task<Result<bool>> ForgotPasswordHandle(string username)
        {
            var userResult = await _userRepository.GetUserWithEmailByUserNameAsync(username);

            var user = userResult.user;
            var email = userResult.Email;
            
            if (user == null || email == default) return Result<bool>.Failure(UserNotFound(username));
            if(user.status != enStatus.Active) return Result<bool>.Failure(UserNotActive(username));

            var tokenValues = _tokensGentator.CreateSecureToken();
            var token = Token.CreateResetPassword(tokenValues.HashedToken, user);

            user.AddToken(token);
            await _userRepository.UpdateAsync(user);

            await _resetPasswordEventHandler.Invoke(user.UserName, email, tokenValues.PlainToken);
            return Result<bool>.Successful(true);
        }

        public async Task<Result<bool>> ResetPasswordHandle(ResetPasswordCommand command)
        {
            var HashedToken = _tokensGentator.HashToken(command.Token);
            var token = await _userRepository.GetToken(HashedToken);
            if(token == default) return Result<bool>.Failure();

            var user = token.User;

            if (!token.IsActive ||
                token.Type != enTokenType.PasswordReset ||
                user!.UserName != command.Username)
            {
                user.Logout();
                await _userRepository.UpdateAsync(user);
                return Result<bool>.Failure();
            }


            var PasswordCreation = Password.Create(command.NewPassword);
            if(!PasswordCreation.IsSuccess) return Result<bool>.Failure(PasswordCreation.Error);

            user.SetPasswordHashing(PasswordCreation.Value!);
            token.Revoke();
            
            await _userRepository.UpdateAsync(user);
            return Result<bool>.Successful(true);
        } 
    }
}
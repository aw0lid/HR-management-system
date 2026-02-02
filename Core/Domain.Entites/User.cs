using SharedKernal;
using Domain.ValueObjects;
using static Domain.ErrorsMenu.ErrorsMenu.User;

namespace Domain.Entites
{
    public enum enRole : byte {SystemAdmin = 1, EmployeeManagement = 2, FinancialManagement = 3}
    public enum enStatus : byte {PendingActivation = 1, Active = 2, Frozen = 3}


    public class User
    {
        public int UserId { get; private set; }
        public string UserName { get; private set; } = null!;
        public Password? UserPasswordHashing { get; private set; } = null;
        public enRole Role { get; private set; }
        public enStatus status { get; private set; }
        public int EmployeeId { get; private set; }


        public Employee? Employee { get; private set; }
        private readonly List<UserLog> _userLogs = new List<UserLog>();
        private readonly List<Token> _tokens = new List<Token>();

        public IReadOnlyCollection<UserLog> UserLogs => _userLogs.AsReadOnly();
        public IReadOnlyCollection<Token> Tokens => _tokens.AsReadOnly();
       

        private User() { }



        private User(string userName, enRole role, int employeeId)
        {
            if(employeeId <= 0) throw new ArgumentNullException(nameof(employeeId));
            if(string.IsNullOrEmpty(userName)) throw new ArgumentNullException("UserName is required");
            

            UserName = userName;
            this.Role = role;
            EmployeeId = employeeId;
            status = enStatus.PendingActivation;
        }

        public static User Create(string userName, enRole role, int employeeId)
        {
            var user = new User(userName, role, employeeId);
            return user;
        }
        
        public Result<User> CompleteSetup(Password password)
        {
            if (status != enStatus.PendingActivation)
                return Result<User>.Failure(UserIsActive);

            if (password == null) throw new ArgumentNullException(nameof(password));

            this.UserPasswordHashing = password;
            this.status = enStatus.Active;

            return Result<User>.Successful(this);
        }

        
        public User ChangeUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("User name cannot be empty.", nameof(userName));

            UserName = userName.Trim();
            return this;
        }

        public User SetPasswordHashing(Password passwordHashing)
        {
            if(passwordHashing == null) throw new ArgumentNullException(nameof(passwordHashing));

            UserPasswordHashing = passwordHashing;
            Logout();
            return this;
        }
       
        public Result<User> Freeze()
        {
            if (status != enStatus.Active)return Result<User>.Failure(UserIsNotActive);
            status = enStatus.Frozen;
            return Result<User>.Successful(this);
        }

        public Result<User> Activate()
        {
            if (status == enStatus.Active)return Result<User>.Failure(UserIsActive);
            status = enStatus.Active;
            return Result<User>.Successful(this);
        }
        


        public Result<User> beEmployeeManager()
        {
            if(this.status != enStatus.Active) return Result<User>.Failure(UserIsNotActive);
            if(this.Role == enRole.SystemAdmin) return Result<User>.Failure(ForbiddenRoleChange);
            this.Role = enRole.EmployeeManagement;
            return Result<User>.Successful(this);
        }

        public Result<User> beFinancialManagement()
        {
            if(this.status != enStatus.Active) return Result<User>.Failure(UserIsNotActive);
            if(this.Role == enRole.SystemAdmin) return Result<User>.Failure(ForbiddenRoleChange);
            this.Role = enRole.FinancialManagement;
            return Result<User>.Successful(this);
        }

        public User Logout()
        {
            foreach(var t in _tokens) t.Revoke();
            return this;
        }

        public User AddToken(Token token)
        {
            if(token == default) throw new ArgumentNullException(nameof(token));

            foreach(var t in _tokens) if(t.Type == token.Type) t.ReplaceWith(token);
            
            _tokens.Add(token);
            return this;
        }

        public User RotateToken(int oldTokenId, Token newToken)
        {
            if(oldTokenId <= 0) throw new ArgumentException(nameof(oldTokenId));
            if(newToken == default) throw new ArgumentNullException(nameof(newToken));

            var oldToken = _tokens.FirstOrDefault(t => t.TokenId == oldTokenId);
            if(oldToken == default) throw new ArgumentNullException(nameof(oldToken));

            oldToken.ReplaceWith(newToken);
            return this;
        }

        public User ExecuteLogin(Token RefToken)
        {
            if(RefToken == default) throw new ArgumentNullException(nameof(RefToken));

            var log = new UserLog();
            _userLogs.Add(log);

            var OldToken = _tokens.FirstOrDefault(t => t.IsActive && t.Type == enTokenType.RefreshToken);
            if(OldToken != default) OldToken.ReplaceWith(RefToken);

            _tokens.Add(RefToken);
            return this;
        }


        public Token? GetToken(string HashedToken)
        {
            if(string.IsNullOrWhiteSpace(HashedToken)) throw new ArgumentNullException(nameof(HashedToken));
            return _tokens.FirstOrDefault(t => t.value == HashedToken);
        }
    }
}
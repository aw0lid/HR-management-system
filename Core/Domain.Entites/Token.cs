namespace Domain.Entites
{

    public enum enTokenType : byte {PasswordReset = 1, AccountActivation = 2, RefreshToken = 3,}

    public class Token
    {
        public int TokenId { get; private set; }
        public string value { get; private set; } = null!;
        public enTokenType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public int UserId { get; private set; }
        public int? ReplacedById {get; private set;}

        public Token? ReplacedByToken {get; private set;}
        public User User { get; private set; } = null!;

        private Token(){ }

        private Token(string token, DateTime expiresAt, enTokenType type, User user)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));
            if(user == default) throw new ArgumentNullException(nameof(user));

            value = token;
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
            Type = type;
            User = user;
            UserId = user.UserId;
        }

        public static Token CreateAccountActivation(string token, User user)
        {
            int DaysToExpire = 1;
            var t = new Token(token, DateTime.UtcNow.AddDays(DaysToExpire), enTokenType.AccountActivation, user);
            return t;
        }

        public static Token CreateResetPassword(string token, User user)
        {
            int MinToExpire = 20;
            var t =  new Token(token, DateTime.UtcNow.AddMinutes(MinToExpire), enTokenType.PasswordReset, user);
            return t;
        }

        public static Token CreateRefreshToken(string token, User user)
        {
            int AddDaysToExpire = 5;
            var t =  new Token(token, DateTime.UtcNow.AddDays(AddDaysToExpire), enTokenType.RefreshToken, user);
            return t;
        }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;


        public void Revoke() => RevokedAt = DateTime.UtcNow;

        public void ReplaceWith(Token newToken)
        {
            if (this.Type != newToken.Type)
                throw new InvalidOperationException("Diff Types of Tokens");

            this.Revoke();
            this.ReplacedByToken = newToken;
        }
    }
}
namespace Domain.Entites
{
    public class UserLog
    {
        public int LogId { get; private set; }
        public int UserId { get; private set; }
        public DateTime LogTime { get; private set; } = DateTime.UtcNow;

        public virtual User User { get; private set; } = null!;

        private UserLog(){}

        public static UserLog Create(User user) => new UserLog{User = user, UserId = user.UserId};
    }
}
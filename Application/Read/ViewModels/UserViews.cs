namespace Read.ViewModels
{
    public class PermissionView
    {
        public int Id {get; set;}
        public string Name {get; set;} = null!;
    }

    public class UserView
    {
        public int Id {get; set;}
        public string FullName {get; set;} = null!;
        public string Code {get; set;} = null!;
        public string Username {get; set;} = null!;
        public bool IsActive {get; set;}
    }
}
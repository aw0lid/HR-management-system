namespace Read.ViewModels
{
    public class UserView
    {
        public int Id {get; set;}
        public string FullName {get; set;} = null!;
        public string Code {get; set;} = null!;
        public string Username {get; set;} = null!;
        public byte StatusId {get; set;}
        public string? StatusName {get; set;}
        public byte roleId {get; set;}
        public string? roleName {get; set;}
    }
}
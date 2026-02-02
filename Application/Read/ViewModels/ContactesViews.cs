namespace Read.ViewModels
{
    public class PhoneView
    {
        public int Id { get; set; }
        public string Phone { get; set; } = null!;
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }
    }

    public class EmailView
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }
    }
}
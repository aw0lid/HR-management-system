namespace Read.ViewModels
{

    public interface IEmployeeView{int Id {get; set;}}

    public class EmployeePersonalInfoView : IEmployeeView
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public string ThirdName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string NationalNumber { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int Age { get; set; }
        public string CurrentAddress { get; set; } = null!;
        public string Nationality { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string StatusName { get; set; } = null!;
    }

  

    public class EmployeeWorkInfoView : IEmployeeView
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Code { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public DateTime? EmployeeResignationDate { get; set; }
        public string Department { get; set; } = null!;
        public string JobTitleLevel { get; set; } = null!;
        public int Weight { get; set; }
        public bool IsManager { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerCode { get; set; }
    }

  
    public class EmployeeFullProfileView : IEmployeeView
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public string ThirdName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string NationalNumber { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string CurrentAddress { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string JobTitleLevel { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public int Age { get; set; }
        public string StatusName { get; set; } = null!;
        public string Nationality { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public bool IsManager { get; set; }
        public string? ManagerName { get; set; }

        public ICollection<PhoneView> Phones { get; set; } = new List<PhoneView>();
        public ICollection<EmailView> Emails { get; set; } = new List<EmailView>();
    }
}
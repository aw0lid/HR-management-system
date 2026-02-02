namespace Application.Read.ViewModels
{
    public class DepartmentView
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Description {get; set;}
    }

    public class JobTitleView
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class JobGradeView
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int Weigth { get; set; }
        public string? Description { get; set; }
    }

    public class JobTitleLevelView
    {
        public int Id { get; set; }
        public string FullTitle { get; set; } = null!;
        public int GradeWeight { get; set; }
    }



    public class NationalityView
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
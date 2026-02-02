using SharedKernal;

namespace Domain.Entites
{
    public class Department
    {
        public int DepartmentId { get; private set; }
        public string DepartmentName { get; private set; } = null!;
        public string? DepartmentDescription { get; private set; }
        public string DepartmentCode { get; private set; } = null!;

       
        private Department() { }

   
        public static Result<Department> Create(string name, string code, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Department name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Department code cannot be empty", nameof(code));

            

            return Result<Department>.Successful(new Department
            {
                DepartmentName = name,
                DepartmentCode = code,
                DepartmentDescription = description
            });
        }

        
      
        public override bool Equals(object? obj)
        {
            if (obj is not Department other) return false;
            if (ReferenceEquals(this, other)) return true;
            if (DepartmentId == 0 || other.DepartmentId == 0) return false;
            return DepartmentId == other.DepartmentId;
        }

        public override int GetHashCode() => DepartmentId.GetHashCode();
    }
}
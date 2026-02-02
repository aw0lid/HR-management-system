using SharedKernal;
using static Domain.ErrorsMenu.ErrorsMenu.Employee;

namespace Domain.Entites
{
    public class JobGrade : IComparable<JobGrade>
    {
        public int JobGradeId { get; private set; }
        public string JobGradeName { get; private set; } = null!;
        public string? LevelDescription { get; private set; }
        public string GradeCode { get; private set; } = null!;
        public int Weight { get; private set; }

        private JobGrade() { }



        public static JobGrade Create(string gradeName, string code, int weight, string description = "")
        {
            if (string.IsNullOrWhiteSpace(gradeName)) throw new ArgumentException(nameof(gradeName));
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException(nameof(code));
            if (weight <= 0) throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be positive");

            return new JobGrade()
            {
                JobGradeName = gradeName.Trim(),
                GradeCode = code.Trim(),
                Weight = weight,
                LevelDescription = description?.Trim() ?? ""
            };
        }

       
       


        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is JobGrade)) return false;
            var other = (JobGrade)obj;

            if (ReferenceEquals(this, other))
                return true;

            return this.JobGradeId == other.JobGradeId;
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(JobGradeId);
        }

        public int CompareTo(JobGrade? other)
        {
            if (other is null) return 1;
            return this.Weight.CompareTo(other.Weight);
        }
    }
}
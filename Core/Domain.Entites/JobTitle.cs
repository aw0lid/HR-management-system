using SharedKernal;
using static Domain.ErrorsMenu.ErrorsMenu.Employee;

namespace Domain.Entites
{
    public class JobTitle
    {
        public int JobTitleId { get; private set; }
        public string JobTitleName { get; private set; } = null!;
        public string? JobTitleDescription { get; private set; } = default;
        public string JobTitleCode { get; private set; } = null!;


        private JobTitle() { }


        public static JobTitle Create(string titleName, string code, string description = "")
        {
            if (string.IsNullOrWhiteSpace(titleName)) throw new ArgumentException(nameof(titleName));
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException(nameof(code));

            return new JobTitle()
            {
                JobTitleName = titleName.Trim(),
                JobTitleCode = code.Trim(),
                JobTitleDescription = description?.Trim() ?? ""
            };
        }

        
        


        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is JobTitle)) return false;
            var other = (JobTitle)obj;

            if (ReferenceEquals(this, other))
                return true;

            return this.JobTitleId == other.JobTitleId;        
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(JobTitleId);
        }

    }
}



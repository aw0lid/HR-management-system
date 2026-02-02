using SharedKernal;
using static Domain.ErrorsMenu.ErrorsMenu.Employee;

namespace Domain.Entites
{
    public class JobTitleLevel
    {
        public int JobTitleLevelId { get; private set; }

        public int JobGradeId { get; private set; }
        public int JobTitleId { get; private set; }

        public JobGrade JobGrade { get; private set; } = null!;
        public JobTitle JobTitle { get; private set; } = null!;



        private JobTitleLevel() { }


        public static JobTitleLevel Create(int jobTitleId, int jobGradeId)
        {
            if (jobTitleId <= 0) throw new ArgumentException(nameof(jobTitleId));
            if (jobGradeId <= 0) throw new ArgumentException(nameof(jobGradeId));

            return new JobTitleLevel()
            {
                JobTitleId = jobTitleId,
                JobGradeId = jobGradeId
            };
        }



        public override bool Equals(object? obj)
        {
            if(obj == null || !(obj is JobTitleLevel))
                return false;

            var other = (JobTitleLevel)obj;

            if (ReferenceEquals(this, other))
                return true;

            return
                  this.JobTitleLevelId == other.JobTitleLevelId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(JobTitleLevelId);
        }
    }
}
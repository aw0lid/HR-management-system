using SharedKernal;
using static Domain.ErrorsMenu.ErrorsMenu.Employee;

namespace Domain.Entites
{
    public class EmployeeWorkInfo
    {
        public int EmployeeWorkInfoId { get; private set; }
        public int EmployeeId { get; private set; }
        public int? ManagerId { get; private set; }
        public int DepartmentId { get; private set; }
        public int JobTitleLevelId { get; private set; }
        

        public Employee? Manager { get; private set; }
        public Department Department { get; private set; } = null!;
        public JobTitleLevel JobTitleLevel { get; private set; } = null!;
        public bool IsCurrent { get; private set; }
        public DateTime FromDate { get; private set; } = DateTime.Now.Date;
        public DateTime? ToDate { get; private set; } = null;


        private EmployeeWorkInfo() { }


        


        public static EmployeeWorkInfo Create(int departmentId,int jobTitleLevelId,int? ManagerId = default)
        {
            if (departmentId <= 0) throw new ArgumentException(nameof(departmentId));
            if (jobTitleLevelId <= 0) throw new ArgumentException(nameof(jobTitleLevelId));
            
            

            var WorkInfo = new EmployeeWorkInfo()
            {
                IsCurrent = true,
                JobTitleLevelId = jobTitleLevelId,
                DepartmentId = departmentId,
                ManagerId = ManagerId
            };

            return WorkInfo;
        }


        public Result<EmployeeWorkInfo> Freeze()
        {
            if (!this.IsCurrent)
                return Result<EmployeeWorkInfo>.Failure(WorkInfoIsNotCurrent);

            this.IsCurrent = false;

            return Result<EmployeeWorkInfo>.Successful(this);
        }
    }
}
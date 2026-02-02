using Read.ViewModels;

namespace Read.Contracts
{
    public interface IEmployeeReader
    {
        Task<EmployeePersonalInfoView?> GetPersonalInfoByIdAsync(int id);
        Task<EmployeePersonalInfoView?> GetPersonalInfoByCodeAsync(string code);
        Task<EmployeePersonalInfoView?> GetPersonalInfoByNationalNumberAsync(string nationalNumber);
        Task<IEnumerable<EmployeePersonalInfoView>> GetAllPersonalInfoAsync(int PageNumber, int Size);

     
        Task<EmployeeWorkInfoView?> GetWorkInfoByIdAsync(int id);
        Task<EmployeeWorkInfoView?> GetWorkInfoByCodeAsync(string code);
        Task<EmployeeWorkInfoView?> GetWorkInfoByNationalNumberAsync(string nationalNumber);
        Task<IEnumerable<EmployeeWorkInfoView>> GetAllWorkInfoAsync(int PageNumber, int Size);

      
        Task<EmployeeFullProfileView?> GetFullProfileByIdAsync(int id);
        Task<EmployeeFullProfileView?> GetFullProfileByCodeAsync(string code);
        Task<EmployeeFullProfileView?> GetFullProfileByNationalNumberAsync(string nationalNumber);
    }
}
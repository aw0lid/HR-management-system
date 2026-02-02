using Domain.Entites;

namespace Application.Write.Contracts
{
    


    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<bool> IsPhoneNumberExistsAsync(string phoneNumber);
        Task<bool> IsEmailExistsAsync(string email);

        Task<bool> IsNationalNumberExistsAsync(string NationalNumber);


        Task<(bool nationalIdExists, bool phoneExists, bool emailExists)>
        CheckConstraintsAsync(
            string nationalId,
            string phone,
            string email);

        Task<IEnumerable<Employee>> GetAllManagersAsync(int Id);
        Task<Employee> GetWithPhones(int Id);
        Task<Employee> GetWithEmails(int Id);
    }
}
using Read.Contracts;
using Read.ViewModels;
using SharedKernal;
using static Application.ErrorsMenu.ApplicationErrorsMenu.EmployeeQueriesErrors;

namespace Read.Providers
{
    public class EmployeeProvider
    {
        private readonly IEmployeeReader _employeeReader;

        public EmployeeProvider(IEmployeeReader employeeReader)
        {
            _employeeReader = employeeReader;
        }

    
        public async Task<Result<EmployeePersonalInfoView>> GetPersonalInfoById(int id)
        {
            var view = await _employeeReader.GetPersonalInfoByIdAsync(id);
            return view == null ? Result<EmployeePersonalInfoView>.Failure(EmployeeNotFound()) : Result<EmployeePersonalInfoView>.Successful(view);
        }

        public async Task<Result<EmployeePersonalInfoView>> GetPersonalInfoByCode(string code)
        {
            var view = await _employeeReader.GetPersonalInfoByCodeAsync(code);
            return view == null ? Result<EmployeePersonalInfoView>.Failure(EmployeeNotFound()) : Result<EmployeePersonalInfoView>.Successful(view);
        }

        public async Task<Result<EmployeePersonalInfoView>> GetPersonalInfoByNationalNumber(string nationalNumber)
        {
            var view = await _employeeReader.GetPersonalInfoByNationalNumberAsync(nationalNumber);
            return view == null ? Result<EmployeePersonalInfoView>.Failure(EmployeeNotFound()) : Result<EmployeePersonalInfoView>.Successful(view);
        }

        public async Task<Result<IEnumerable<EmployeePersonalInfoView>>> GetAllPersonalInfo(int pagenumber = 1, int Size = 10)
        {
            var view = await _employeeReader.GetAllPersonalInfoAsync(pagenumber, Size);
            return view == null ? Result<IEnumerable<EmployeePersonalInfoView>>.Failure(EmployeesEmpty()) : Result<IEnumerable<EmployeePersonalInfoView>>.Successful(view);
        }


     
        public async Task<Result<EmployeeWorkInfoView>> GetWorkInfoById(int id)
        {
            var view = await _employeeReader.GetWorkInfoByIdAsync(id);
            return view == null ? Result<EmployeeWorkInfoView>.Failure(EmployeeNotFound()) : Result<EmployeeWorkInfoView>.Successful(view);
        }

        public async Task<Result<EmployeeWorkInfoView>> GetWorkInfoByCode(string code)
        {
            var view = await _employeeReader.GetWorkInfoByCodeAsync(code);
            return view == null ? Result<EmployeeWorkInfoView>.Failure(EmployeeNotFound()) : Result<EmployeeWorkInfoView>.Successful(view);
        }

        public async Task<Result<EmployeeWorkInfoView>> GetWorkInfoByNationalNumber(string nationalNumber)
        {
            var view = await _employeeReader.GetWorkInfoByNationalNumberAsync(nationalNumber);
            return view == null ? Result<EmployeeWorkInfoView>.Failure(EmployeeNotFound()) : Result<EmployeeWorkInfoView>.Successful(view);
        }

        public async Task<Result<IEnumerable<EmployeeWorkInfoView>>> GetAllWorkInfo(int pagenumber = 1, int Size = 10)
        {
            var view = await _employeeReader.GetAllWorkInfoAsync(pagenumber, Size);
            return view == null ? Result<IEnumerable<EmployeeWorkInfoView>>.Failure(EmployeesEmpty()) : Result<IEnumerable<EmployeeWorkInfoView>>.Successful(view);
        }


     
        public async Task<Result<EmployeeFullProfileView>> GetFullProfileById(int id)
        {
            var view = await _employeeReader.GetFullProfileByIdAsync(id);
            return view == null ? Result<EmployeeFullProfileView>.Failure(EmployeeNotFound()) : Result<EmployeeFullProfileView>.Successful(view);
        }

        public async Task<Result<EmployeeFullProfileView>> GetFullProfileByCode(string code)
        {
            var view = await _employeeReader.GetFullProfileByCodeAsync(code);
            return view == null ? Result<EmployeeFullProfileView>.Failure(EmployeeNotFound()) : Result<EmployeeFullProfileView>.Successful(view);
        }

        public async Task<Result<EmployeeFullProfileView>> GetFullProfileByNationalNumber(string nationalNumber)
        {
            var view = await _employeeReader.GetFullProfileByNationalNumberAsync(nationalNumber);
            return view == null ? Result<EmployeeFullProfileView>.Failure(EmployeeNotFound()) : Result<EmployeeFullProfileView>.Successful(view);
        }
    }
}
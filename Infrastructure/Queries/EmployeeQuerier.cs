using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Read.Contracts;
using Read.ViewModels;
using System.Data;
using System.Data.Common;

namespace Infrastructure.Queries
{
    public class EmployeeQuerier : IEmployeeReader
    {
        private readonly HRDBContext _context;
        public EmployeeQuerier(HRDBContext context) => _context = context;

        



        public async Task<EmployeePersonalInfoView?> GetPersonalInfoByIdAsync(int id) =>
            _context.Database.SqlQueryRaw<EmployeePersonalInfoView>("EXEC sp_GetEmployeePersonalInfo @Id = {0}", id)
            .AsEnumerable().FirstOrDefault();

        public async Task<EmployeePersonalInfoView?> GetPersonalInfoByCodeAsync(string code) =>
            _context.Database.SqlQueryRaw<EmployeePersonalInfoView>("EXEC sp_GetEmployeePersonalInfo @Code = {0}", code)
            .AsEnumerable().FirstOrDefault();

        public async Task<EmployeePersonalInfoView?> GetPersonalInfoByNationalNumberAsync(string nationalNumber) =>
            _context.Database.SqlQueryRaw<EmployeePersonalInfoView>("EXEC sp_GetEmployeePersonalInfo @NationalNumber = {0}", nationalNumber)
            .AsEnumerable().FirstOrDefault();

        public async Task<IEnumerable<EmployeePersonalInfoView>> GetAllPersonalInfoAsync(int PageNumber, int Size) =>
            _context.Database.SqlQueryRaw<EmployeePersonalInfoView>("EXEC sp_GetEmployeePersonalInfoPaged @PageNumber = {0}, @PageSize = {1}", PageNumber, Size)
            .AsEnumerable().ToList();

       

       

        public async Task<EmployeeWorkInfoView?> GetWorkInfoByIdAsync(int id) =>
            _context.Database.SqlQueryRaw<EmployeeWorkInfoView>("EXEC sp_GetEmployeeWorkInfo @Id = {0}", id)
            .AsEnumerable().FirstOrDefault();

        public async Task<EmployeeWorkInfoView?> GetWorkInfoByCodeAsync(string code) =>
            _context.Database.SqlQueryRaw<EmployeeWorkInfoView>("EXEC sp_GetEmployeeWorkInfo @Code = {0}", code)
            .AsEnumerable().FirstOrDefault();

        public async Task<EmployeeWorkInfoView?> GetWorkInfoByNationalNumberAsync(string nationalNumber) =>
            _context.Database.SqlQueryRaw<EmployeeWorkInfoView>("EXEC sp_GetEmployeeWorkInfo @NationalNumber = {0}", nationalNumber)
            .AsEnumerable().FirstOrDefault();

        public async Task<IEnumerable<EmployeeWorkInfoView>> GetAllWorkInfoAsync(int PageNumber, int Size) =>
            _context.Database.SqlQueryRaw<EmployeeWorkInfoView>("EXEC sp_GetEmployeeWorkInfoPaged @PageNumber = {0}, @PageSize = {1}", PageNumber, Size)
            .AsEnumerable().ToList();

        

        

        public Task<EmployeeFullProfileView?> GetFullProfileByIdAsync(int id) => FetchFullProfileInternal(id: id);
        public Task<EmployeeFullProfileView?> GetFullProfileByCodeAsync(string code) => FetchFullProfileInternal(code: code);
        public Task<EmployeeFullProfileView?> GetFullProfileByNationalNumberAsync(string nationalNumber) => FetchFullProfileInternal(nationalNumber: nationalNumber);

        private async Task<EmployeeFullProfileView?> FetchFullProfileInternal(int? id = null, string? code = null, string? nationalNumber = null)
        {
            var connection = _context.Database.GetDbConnection();
            bool wasClosed = connection.State == ConnectionState.Closed;

            try
            {
                if (wasClosed) await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = "sp_GetEmployeeFullProfile";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@Id", (object?)id ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Code", (object?)code ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@NationalNumber", (object?)nationalNumber ?? DBNull.Value));

                await using var reader = await ((DbCommand)command).ExecuteReaderAsync();

                if (!await reader.ReadAsync()) return null;

                var profile = new EmployeeFullProfileView
                {
                    Id = reader.GetInt32("Id"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Code = reader.GetString("Code"),
                    NationalNumber = reader.GetString("NationalNumber"),
                    CurrentAddress = reader.GetString("CurrentAddress"),
                    Department = reader.GetString("Department"),
                    JobTitleLevel = reader.GetString("JobTitleLevel"),
                    HireDate = reader.GetDateTime("HireDate"),
                    Age = Convert.ToInt16(reader["Age"]),
                    StatusName = reader.GetString("StatusName"),
                    Nationality = reader.GetString("Nationality"),
                    Gender = reader.GetString("Gender"),
                    IsManager = reader.GetBoolean("IsManager"),
                    ManagerName = reader.IsDBNull("ManagerName") ? null : reader.GetString("ManagerName")
                };

                
                if (await reader.NextResultAsync())
                    while (await reader.ReadAsync())
                        profile.Phones.Add(new PhoneView { Id = reader.GetInt32("Id"), Phone = reader.GetString("Number"), IsPrimary = reader.GetBoolean("IsPrimary") });

               
                if (await reader.NextResultAsync())
                    while (await reader.ReadAsync())
                        profile.Emails.Add(new EmailView { Id = reader.GetInt32("Id"), Email = reader.GetString("Email"), IsPrimary = reader.GetBoolean("IsPrimary") });

                return profile;
            }
            finally
            {
                
                if (wasClosed && connection.State != ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }
    }
}
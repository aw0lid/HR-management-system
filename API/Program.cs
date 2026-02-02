using Infrastructure;
using Infrastructure.Queries;
using Application.Write.CommandHandlers;
using Application.Read.Providers;
using Read.Contracts;
using Read.Providers;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Application.Cache;
using FluentValidation;
using API.Middleware;
using Domain.Entites;
using Infrastructure.Repositories;
using Application.Write.Contracts;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<HRDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions => 
    {
        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    });
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});




builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmployeeReader, EmployeeQuerier>();
builder.Services.AddScoped<IUserReader, UserQuerier>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EmployeeHandler>();
builder.Services.AddScoped<UserHandler>();
builder.Services.AddScoped<EmployeeContactsHandler>();
builder.Services.AddScoped<EmployeeProvider>();
builder.Services.AddScoped<UserProvider>();
builder.Services.AddScoped<NationalityProvider>();
builder.Services.AddScoped<DepartmentProvider>();
builder.Services.AddScoped<JobTitleProvider>();
builder.Services.AddScoped<JobGradeProvider>();
builder.Services.AddScoped<JobTitleLevelProvider>();
builder.Services.AddScoped<PermissionProvider>();

builder.Services.AddSingleton<NationalitiesCache>();
builder.Services.AddSingleton<DepartmentsCache>();
builder.Services.AddSingleton<JobTitlesCache>();
builder.Services.AddSingleton<JobTitleGradesCache>();
builder.Services.AddSingleton<JobTitleLevelsCache>();
builder.Services.AddSingleton<PermissionsCache>();

builder.Services.AddSingleton<CacheProvider<Nationality>>(sp => sp.GetRequiredService<NationalitiesCache>());
builder.Services.AddSingleton<CacheProvider<Department>>(sp => sp.GetRequiredService<DepartmentsCache>());
builder.Services.AddSingleton<CacheProvider<JobTitle>>(sp => sp.GetRequiredService<JobTitlesCache>());
builder.Services.AddSingleton<CacheProvider<JobGrade>>(sp => sp.GetRequiredService<JobTitleGradesCache>());
builder.Services.AddSingleton<CacheProvider<JobTitleLevel>>(sp => sp.GetRequiredService<JobTitleLevelsCache>());
builder.Services.AddSingleton<CacheProvider<Permission>>(sp => sp.GetRequiredService<PermissionsCache>());

builder.Services.AddScoped<IDataLoader<Department>, DepartmentLoader>();
builder.Services.AddScoped<IDataLoader<Nationality>, NationalityLoader>();
builder.Services.AddScoped<IDataLoader<JobTitle>, JobTitleLoader>();
builder.Services.AddScoped<IDataLoader<JobGrade>, JobGradeLoader>();
builder.Services.AddScoped<IDataLoader<JobTitleLevel>, JobTitleLevelLoader>();
builder.Services.AddScoped<IDataLoader<Permission>, PermissionLoader>();


builder.Services.AddValidatorsFromAssemblyContaining<Application.Write.FluentValidation.EmployeeAddValidator>();




builder.Services.AddScoped<API.Filters.ValidationFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<API.Filters.ValidationFilter>(); 
})
.AddJsonOptions(options => {
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});







var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HR System API V1");
        c.RoutePrefix = string.Empty; 
    });
}

app.UseAuthorization();
app.MapControllers();

app.Run();
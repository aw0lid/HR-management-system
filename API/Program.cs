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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Services;
using System.Threading.RateLimiting;
using Application.Services;
using Application.Events;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "HR System API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter Token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});




builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));




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
builder.Services.AddScoped<IPendingActionsRepository, PendingActionRepository>();
builder.Services.AddScoped<IEmployeeReader, EmployeeQuerier>();
builder.Services.AddScoped<IUserReader, UserQuerier>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<EmployeeHandler>();
builder.Services.AddScoped<UserHandler>();
builder.Services.AddScoped<AuthHandler>();
builder.Services.AddScoped<PendingActionshandler>();
builder.Services.AddScoped<EmployeeContactsHandler>();


builder.Services.AddScoped<CrateNewUserEventHandler>();
builder.Services.AddScoped<ResetPasswordEventHandler>();
builder.Services.AddScoped<NewPendingAdminActionCreatedEventhandler>();
builder.Services.AddScoped<ApproveReactivateAdminEventHandler>();
builder.Services.AddScoped<ApproveFreezAdminEventHandler>();



builder.Services.AddScoped<EmployeeProvider>();
builder.Services.AddScoped<UserProvider>();
builder.Services.AddScoped<NationalityProvider>();
builder.Services.AddScoped<DepartmentProvider>();
builder.Services.AddScoped<JobTitleProvider>();
builder.Services.AddScoped<JobGradeProvider>();
builder.Services.AddScoped<JobTitleLevelProvider>();
builder.Services.AddScoped<EmailTemplateGenerator>();

builder.Services.AddSingleton<NationalitiesCache>();
builder.Services.AddSingleton<DepartmentsCache>();
builder.Services.AddSingleton<JobTitlesCache>();
builder.Services.AddSingleton<JobTitleGradesCache>();
builder.Services.AddSingleton<JobTitleLevelsCache>();

builder.Services.AddSingleton<CacheProvider<Nationality>>(sp => sp.GetRequiredService<NationalitiesCache>());
builder.Services.AddSingleton<CacheProvider<Department>>(sp => sp.GetRequiredService<DepartmentsCache>());
builder.Services.AddSingleton<CacheProvider<JobTitle>>(sp => sp.GetRequiredService<JobTitlesCache>());
builder.Services.AddSingleton<CacheProvider<JobGrade>>(sp => sp.GetRequiredService<JobTitleGradesCache>());
builder.Services.AddSingleton<CacheProvider<JobTitleLevel>>(sp => sp.GetRequiredService<JobTitleLevelsCache>());

builder.Services.AddScoped<IDataLoader<Department>, DepartmentLoader>();
builder.Services.AddScoped<IDataLoader<Nationality>, NationalityLoader>();
builder.Services.AddScoped<IDataLoader<JobTitle>, JobTitleLoader>();
builder.Services.AddScoped<IDataLoader<JobGrade>, JobGradeLoader>();
builder.Services.AddScoped<IDataLoader<JobTitleLevel>, JobTitleLevelLoader>();

builder.Services.AddScoped<ITokenService, TokensGentator>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateGenerator>();

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









builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            status = 429,
        }, token);
    };

    options.AddPolicy("auth-policy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.AddPolicy("global-user-policy", httpContext =>
    {
        var userId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? httpContext.User.FindFirst("sub")?.Value 
                     ?? httpContext.Connection.RemoteIpAddress?.ToString(); 

        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: userId,
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 50,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 4,
                QueueLimit = 0
            });
    });
});








var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),

       NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
       RoleClaimType = System.Security.Claims.ClaimTypes.Role,

        ClockSkew = TimeSpan.Zero
    };


    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var statusClaim = context.Principal?.FindFirst("status")?.Value;
            if (string.IsNullOrEmpty(statusClaim) || statusClaim != "Active")
                context.Fail("UserAccountInactive");
            
            return Task.CompletedTask;
        }
    };
});





var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("StrictPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HR System API V1");
        c.RoutePrefix = string.Empty; 
        c.HeadContent = @"
<style>
    :root {
        --bg-main: #18191a;
        --bg-card: #242526;
        --text-primary: #e4e6eb;
        --text-secondary: #b0b3b8;
        --accent: #2d88ff;
        --border: #3e4042;
    }

    body { background-color: var(--bg-main) !important; color: var(--text-primary) !important; }
    
    .swagger-ui .topbar { background-color: var(--bg-card) !important; border-bottom: 1px solid var(--border); }
    
    .swagger-ui .info .title, .swagger-ui .info li, .swagger-ui .info p, .swagger-ui .info table { color: var(--text-primary) !important; }
    
    .swagger-ui .opblock-tag { color: var(--text-primary) !important; border-bottom: 1px solid var(--border); }
    
    .swagger-ui .opblock { background: var(--bg-card) !important; border: 1px solid var(--border) !important; }
    
    .swagger-ui .opblock .opblock-summary-description { color: var(--text-secondary) !important; }
    
    .swagger-ui input, .swagger-ui select, .swagger-ui textarea { 
        background: #3a3b3c !important; 
        color: white !important; 
        border: 1px solid var(--border) !important; 
    }

    .swagger-ui .btn.authorize { color: var(--accent) !important; border-color: var(--accent) !important; }
    .swagger-ui .btn.authorize svg { fill: var(--accent) !important; }

    .swagger-ui .model-box { background: #3a3b3c !important; }
    .swagger-ui section.models { border: 1px solid var(--border) !important; }
    .swagger-ui section.models.standard h4 { color: var(--text-secondary) !important; }
    
    .swagger-ui .responses-table, .swagger-ui .response-col_status, .swagger-ui .response-col_description { 
        color: var(--text-primary) !important; 
    }
</style>";
    });
}


app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
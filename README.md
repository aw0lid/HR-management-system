# HR Management System 💼

A personal learning project built with **.NET 8** and **Entity Framework Core 9**. I built this to practice organizing code into clear layers, implementing security patterns (JWT, RBAC, password hashing), and learning how to work with a complex database schema.

---

## Table of Contents 📚

1. [What This Is](#what-this-is)
2. [Code Organization](#code-organization)
3. [Data Access Strategy](#data-access-strategy)
4. [Security Implementation](#security-implementation)
5. [What I've Built](#what-ive-built)
6. [How a Request Flows Through](#how-a-request-flows-through)
7. [Caching](#caching)
8. [Getting Started](#getting-started)
9. [Tech Stack](#tech-stack)

---

## What This Is 📌

This is an HR system built for learning. The main goals were:

1. **Organize code into layers** — Keep the domain logic separate from HTTP concerns and data access
2. **Learn JWT and role-based access** — Implement token-based auth and role validation across multiple layers
3. **Practice working with complex schemas** — Use both EF Core and raw SQL where each makes sense
4. **Monitor what the ORM is doing** — Enable logging to see the SQL queries EF Core generates
5. **Build something real** — Not a toy example, but entities that actually represent an HR system

---

## Architecture Diagram 🏗️

```
                           API Layer (API Project) 🌐

                 ┌────────────────────────────────────┐
                 │              API (API)             │
                 │  - Controllers                     │
                 │  - Meddilwares                     │
                 │  - FilterActions                   │
                 └────────────────┬───────────────────┘
                                  │
                                  │ depends on
                                  ▼
        ┌────────────────────────────────────────────────────────┐
        │        Application Layer (Application) ⚙️              │
        │  - CommandHandlers                                     │
        │  - Providers                                           │
        │  - FluentValidation                                    │
        │  - Commands                                            │
        │  - ViewModles                                          │
        │  - Contracts                                           │
        │  - Caching                                             │
        │  - EventsHandlers                                      │
        │  - ApplicationErrorsMenu                               │
        └───────────────┬───────────────────────┬────────────────┘
                        │                       │
              depends on│                       │ defines
                        ▼                       ▼
                  ┌─────┴────────────────────────────────────────┐
                  │      Core Layer (Core / Doamin) 💠           │
                  │         (Center: Pure Domain Model)          │
                  │  - Entites                                   │
                  │  - Services                                  │
                  │  - ValueObjects                              │
                  │  - SharedKernal                              │
                  │  - DomainErrorsMenu                          │
                  └───────────────▲──────────────────────────────┘
                                  │
                                  │ depends on
                                  │ (implements Application contracts)
        ┌─────────────────────────┴───────────────────────────────┐
        │   Infrastructure Layer (Infrastructure) 🔌              │
        │  - Cinfigrations                                        │
        │  - Repositories                                         │
        │  - Queries (EF, ADO, SPs, Views ...)                    │
        │  - Migrations                                           │
        │  - Services (Emails, Tokens ...)                        │
        │  - HRDBContext.cs (EF Core DbContext)                   │
        └─────────────────────────────────────────────────────────┘
```

---

### **Core Layer** (`Core/Domain.*`) 💠
Pure business logic. No dependencies on HTTP, EF Core, or external frameworks.
- 🔹 **Entities**: `User`, `Employee`, `Department`, `JobTitle`, etc.
- 🔹 **Value Objects**: `Password` (BCrypt hashing), `EmailAddress`, `NationalNumber`
- 🔹 **Domain Services**: Business rules and validation
- 🔹 **Result Pattern**: All operations return `Result<T>` for predictable error handling

```csharp
// Example: Password is always hashed, never stored as plaintext
public sealed class Password
{
    public string Value { get; private set; } = null!;  // Hashed value
    
    public static Result<Password> Create(string plainPassword)
    {
        if (plainPassword.Length < 8) return Result<Password>.Failure(TooShort);
        if (!plainPassword.Any(char.IsUpper) || !plainPassword.Any(char.IsDigit))
            return Result<Password>.Failure(Invalid);
        
        string hashed = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        return Result<Password>.Successful(new Password(hashed));
    }
}
```

### **Application Layer** (`Application/`) ⚙️
Orchestration. Takes commands/queries, coordinates with domain and infrastructure, returns results.
- 🔹 **Command Handlers**: Process writes (e.g., `UserHandler.AddHandle`)
- 🔹 **Query Providers**: Fetch data (e.g., `UserProvider.GetByIdAsync`)
- 🔹 **Validators**: FluentValidation for request shape
- 🔹 **Event Handlers**: Send emails on user creation, password reset, etc. 📧
- 🔹 **Cache Layer**: In-memory lookups for reference data

```csharp
// Example: Handler coordinates domain logic with infrastructure
public async Task<Result<bool>> AddHandle(UserAddCommand command)
{
    // 1. Validate via repository
    var validation = await _repo.GetEmployeeValidationStatus(command.EmployeeId);
    if (validation == null) return Result<bool>.Failure(EmployeeNotFound);
    
    // 2. Create domain entity
    var user = User.Create(command.UserName, (enRole)command.Role, command.EmployeeId);
    
    // 3. Persist
    await _repo.AddAsync(user);
    
    // 4. Send email (side effect)
    await _emailService.SendActivationAsync(validation.Email);
    
    return Result<bool>.Successful(true);
}
```

### **Infrastructure Layer** (`Infrastructure/`) 🔌
Data access and external services.
- 🔹 **Repositories**: EF Core for writes and simple reads
- 🔹 **Queriers**: Raw SQL for complex projections
- 🔹 **Services**: Email, caching 📧
- 🔹 **DbContext**: EF Core configuration

```csharp
// EF Core: Write operation preserves domain invariants
public async Task AddAsync(User user)
{
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();
}

// Raw SQL: Complex read returns DTO, no change tracking
public async Task<EmployeeDetailsDto?> GetFullProfileAsync(int id)
{
    return await _context.Database.SqlQueryRaw<EmployeeDetailsDto>(
        @"SELECT e.EmployeeId, e.FirstName, e.LastName,
                 d.DepartmentName, j.JobTitleName, COUNT(em.EmailId) AS EmailCount
          FROM Employees e
          LEFT JOIN EmployeeWorkInfo w ON e.EmployeeId = w.EmployeeId AND w.IsCurrent = 1
          LEFT JOIN Departments d ON w.DepartmentId = d.DepartmentId
          LEFT JOIN JobTitles j ON ... WHERE e.EmployeeId = {0}",
        id
    ).FirstOrDefaultAsync();
}
```

### **API Layer** (`API/`) 🌐
HTTP endpoints and middleware.
- 🔹 **Controllers**: REST routes with JWT validation
- 🔹 **Middleware**: Global exception handler, request logging
- 🔹 **Filters**: Input validation before handlers run

```csharp
[HttpPost("/create-user")]
[Authorize(Roles = "SystemAdmin")]
public async Task<IActionResult> CreateUser([FromBody] UserAddCommand command)
{
    var result = await _handler.AddHandle(command);
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
}
```

---

## Data Access Strategy 🗄️

I use two different approaches depending on the job:

### **EF Core: Writes & Simple Reads** ⚙️

When I need to:
- 🔹 Write data and enforce business rules
- 🔹 Update multiple related entities in one transaction
- 🔹 Use foreign keys and constraints

**Why**: EF Core's unit-of-work guarantees everything is consistent before saving.

```csharp
// Write: EF tracks changes and enforces domain rules
var user = User.Create(name, role, employeeId);  // Validates in constructor
await _context.Users.AddAsync(user);
await _context.SaveChangesAsync();  // Single transaction
```

### **Raw SQL: Complex Reads & Reports** 🚀

When I need to:
- 🔹 Join 5+ tables with complex conditions
- 🔹 Return DTOs (not domain entities)
- 🔹 Get results fast without ORM overhead

**Why**: The database engine is better at joining and filtering. I get results in one round-trip and see the actual SQL.

```csharp
// Read: Raw SQL, no change tracking, single query
var result = await _context.Database.SqlQueryRaw<DepartmentReportDto>(
    "EXEC sp_GetDepartmentStats @DeptId = {0}", deptId
).ToListAsync();
```

### **Decision Table**

| Scenario | Use | Reason |
|----------|-----|--------|
| Create user, update related data | EF Core | Need transaction & invariants |
| Get user by ID | EF Core | Simple, EF is efficient |
| Complex dashboard with joins | Raw SQL | Performance, see the SQL |
| Report: all employees by dept | Raw SQL | Set-based, needs optimization |
| Validate employee exists | EF Core | Need consistency check |

---

## Security Implementation 🛡️

### **JWT Bearer Tokens** 🔑

Requests include an Authorization header with a JWT token. The API validates it before processing.

```csharp
// Configuration: Validate token signature, expiry, issuer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });

// Usage: Endpoints require [Authorize]
[Authorize]
public async Task<IActionResult> GetUser(int id) { ... }
```

**Token Types:**
- 🔑 **AccessToken**: 15 minutes (short-lived, for security)
- 🔑 **RefreshToken**: 5 days (used to get a new AccessToken)

### **Token Rotation** 🔁

Every time a client refreshes, the old token is revoked and a new one is issued. If a token is leaked, it's only valid once.

```csharp
// Refresh endpoint: Revoke old, issue new
var oldToken = await _repo.GetToken(hashedRefreshToken);
oldToken.Revoke();

var newAccessToken = _generator.GenerateJwtToken(user);
var newRefreshToken = Token.CreateRefreshToken(_generator.CreateSecureToken());

user.RotateToken(oldToken, newRefreshToken);
await _repo.UpdateAsync(user);
```

### **Password Hashing with BCrypt** 🔒

Passwords are hashed immediately when created. The domain `Password` value object enforces this.

```csharp
// Passwords: Validated (8+ chars, uppercase, digit), then hashed with BCrypt
public static Result<Password> Create(string plainPassword)
{
    if (plainPassword.Length < 8) return Failure;
    if (!plainPassword.Any(char.IsUpper) || !plainPassword.Any(char.IsDigit)) return Failure;
    
    string hashed = BCrypt.Net.BCrypt.HashPassword(plainPassword);  // Salt embedded
    return Result<Password>.Successful(new Password(hashed));
}

// Verify: BCrypt compares plaintext to stored hash
public static bool Verify(string plainPassword, string hash)
    => BCrypt.Net.BCrypt.Verify(plainPassword, hash);
```

### **Role-Based Access Control** 🧩

Three roles restrict what users can do:
- **SystemAdmin** (1): Can create users, freeze admins, manage system
- **EmployeeManagement** (2): Can manage employees
- **FinancialManagement** (3): Can manage payroll

Checked at two levels:

**1. Controller Level** — Block access to the endpoint
```csharp
[Authorize(Roles = "SystemAdmin")]
public async Task<IActionResult> CreateUser([FromBody] UserAddCommand command) { ... }
```

**2. Handler Level** — Double-check the rule
```csharp
if ((enRole)command.Role == enRole.SystemAdmin)
{
    var jobCode = await _repo.GetEmployeeJobCode(command.EmployeeId);
    if (jobCode != "SYSAD")  // Only employees with job code SYSAD can be system admin
        return Result<bool>.Failure(EmployeeNotSystemAdmin);
}
```

### **Four-Eyes Approval** 👀

Sensitive operations (freezing an admin) require approval from a different admin. Prevents one person from locking everyone out.

```csharp
// Step 1: Request freeze
var pendingAction = PendingAdminAction.CreateFreezeAdminAction(requestedBy, targetUserId);
await _repo.AddAsync(pendingAction);
// Notify other admins via email

// Step 2: Another admin approves
public async Task<Result<bool>> ResponseAdminActionHandle(ApproveCommand cmd, int approverId)
{
    var action = await _repo.GetByIdAsync(cmd.ActionId);
    
    // Guard: Can't approve your own request
    if (action.RequestedBy == approverId) return Failure;
    
    // Guard: Can't approve actions targeting yourself
    if (action.TargetUserId == approverId) return Failure;
    
    action.Approve(approverId);
    // Execute: Freeze the admin
}
```

### **Logging to See SQL** 📡

⚠️ **EnableSensitiveDataLogging is ON** — this logs all SQL queries with their parameter values.

```csharp
builder.Services.AddDbContext<HRDBContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.EnableSensitiveDataLogging();  // ✅ ON for learning
    options.EnableDetailedErrors();
});
```

**Why**: I want to see exactly what SQL EF Core generates, how many round-trips it makes, and where N+1 problems might hide. This is for learning; it would be disabled in production.

---

## What I've Built 🧱

### **Implemented: 12 Core Entities** ✅

These are finished and working:

| Entity | Purpose |
|--------|---------|
| `Employee` | Central employee record |
| `EmployeeWorkInfo` | Employment history, department, job assignments |
| `EmployeeContacts` | Phone numbers and email addresses |
| `Department` | Organization structure |
| `JobTitle` | Job definitions (e.g., "Software Engineer") |
| `JobGrade` | Compensation levels |
| `JobTitleLevel` | Combination of grade + title |
| `Nationality` | Country reference data |
| `User` | Identity (username, password, role) |
| `Token` | JWT refresh tokens and account activation tokens |
| `UserLog` | Audit trail (login history) |
| `PendingAdminAction` | Four-eyes approval workflow |

**Supporting Infrastructure:**
- ✔️ Repositories for data access
- ✔️ Handlers for business logic
- ✔️ Email service for notifications 📧
- ✔️ In-memory cache for lookups

### **Roadmap: Not Yet Started** ⏳

These are in the database schema but not implemented:

- ⏳ **Recruitment**: Job openings, applications, offers
- ⏳ **Payroll**: Salary records, tax, insurance deductions
- ⏳ **Attendance**: Shifts, clock in/out, daily attendance
- ⏳ **Leave**: Leave requests, balances, public holidays
- ⏳ **Contracts**: Employment contracts, resignations, exits

Each represents a chance to apply the same patterns I've learned.

---

## How a Request Flows Through 🔄

Example: Creating a new user

```
1. HTTP Request arrives
   POST /api/users/create-user
   { "employeeId": 42, "userName": "john.doe", "role": 1 }

2. API Layer: Exception Middleware wraps everything
   (catches errors, returns JSON)

3. JWT Validation: Token signature & expiry checked
   (rejects if invalid)

4. Validation Filter: Request shape validated
   (returns 400 if invalid)

5. Authorization: Check [Authorize(Roles = "SystemAdmin")]
   (rejects if user lacks role)

6. Controller: Calls handler
   var result = await _handler.AddHandle(command);

7. Application Handler: Coordinates logic
   a. Load employee to validate they exist
   b. Check role assignment rules
   c. Create User domain entity (validates invariants)
   d. Persist via repository
   e. Send activation email
   
8. Infrastructure: EF Core saves
   await _context.SaveChangesAsync();

9. Response: Map result to HTTP
   if (result.IsSuccess) return Ok(result.Value);
   else return BadRequest(result.Error);

10. HTTP Response sent
    { "success": true }
```

---

## Caching 🧠

In-memory cache stores reference data that's read often but changes rarely:

| Cache | Data | Refresh | Used For |
|-------|------|---------|----------|
| Nationalities | All countries | 7 days | Employee forms |
| Departments | All departments | 1 day | Dropdowns, validation |
| JobTitles | All job titles | 1 day | Employee assignments |
| JobGrades | All salary grades | 1 day | Compensation |
| JobTitleLevels | Grade+Title combos | 1 day | Reports |

When a handler needs this data, it asks the cache. If the data is stale, the cache reloads from the database with `AsNoTracking()` (no change tracking overhead).

```csharp
// Handler: Use cached departments
var departments = await _cache.GetAllAsync();  // Hits cache or DB
var dept = departments.FirstOrDefault(d => d.Id == deptId);
```

---

## Getting Started 🚀

### **Prerequisites**
- 🔹 .NET 8 SDK
- 🔹 SQL Server 2019 or later

### **Setup**

1. **Configure database connection**
   ```json
   // API/appsettings.Development.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=HRSystem;Trusted_Connection=true;"
     }
   }
   ```

2. **Create database schema**
   ```bash
   sqlcmd -S YOUR_SERVER -d HRSystem -i StoredProceduresAndViews.sql
   ```

3. **Apply EF Core migrations**
   ```bash
   cd Core
   dotnet ef database update --project Infrastructure --startup-project ../API
   ```

4. **Run the API**
   ```bash
   dotnet run --project API
   ```
   - Open `https://localhost:5001/swagger` in your browser

5. **Test an endpoint**
   - Click "Authorize" in Swagger and get a JWT token
   - Try `POST /api/users/create-user` with your token

---

## Tech Stack 🧰

| Layer | Technology | Why |
|-------|-----------|-----|
| **Runtime** | .NET 8 | Modern, async-first |
| **ORM** | Entity Framework Core 9 | Learn unit-of-work and change tracking |
| **Database** | SQL Server | Complex schema, stored procedures |
| **Auth** | JWT + Refresh Tokens | Stateless, learn token patterns |
| **Hashing** | BCrypt.Net-Next | Adaptive, secure password hashing |
| **Validation** | FluentValidation | Clean, declarative validation |
| **Email** | MailKit + SMTP | Async email notifications |
| **Caching** | MemoryCache | Simple in-memory lookups |
| **API** | ASP.NET Core Web API | REST endpoints with Swagger |

---



## License 📄

© 2026 HR Management System — Personal Learning Project

---

**Last Updated**: February, 2026  
**Status**: Active — Continuing to build and learn

# HR System

An enterprise HR management platform built on .NET 8 and Entity Framework Core 9. The system is architected to support a full-spectrum HR ecosystem spanning recruitment, payroll, attendance, leave, employee lifecycle, and identity management.

The **Identity & Access** The solution employs a Purpose-Driven Data Access Strategy, selecting the optimal tool based on the operation's nature and performance requirements:
EF Core 9 (Command-Side & Standard Reads): Used for all write operations and standard entity lookups. It ensures Domain Integrity by enforcing business invariants through factory methods and private setters. EF Core manages the Unit of Work and change tracking, ensuring transactional consistency across multi-entity updates.
T-SQL & Raw ADO.NET (Complex Query-Side): Explicitly reserved for high-performance, read-only projections and complex reporting. For scenarios involving heavy multi-table joins, computed fields, and server-side pagination, the system utilizes T-SQL (via Stored Procedures and Views) and raw ADO.NET (ExecuteReaderAsync). This bypasses EF Core's mapping overhead and N+1 risks, providing granular control over the SQL execution plan and ensuring maximum database throughput for "heavy-duty" operations.



![DB digram](./HR_SystemDB.png)


---

## Full Business Capabilities (Schema as Source of Truth)

The database schema defines the complete business scope. The following describes the intended capabilities and data flow across all domains.

### Core Employee Master Data

**Employees** — Central entity: personal data (`EmployeeFirstName`, `EmployeeSecondName`, `EmployeeThirdName`, `EmployeeLastName`), `EmployeeCode` (unique), `EmployeeNationalNumber`, `EmployeeGender`, `Status` (Active, Resigned, Terminated, Retired, On Leave, Suspended), `CurrentAddress`, `EmployeeNationalityId`, `IsForeign`, `EmployeeBirthDate`, `HireDate`, `EmployeeResignationDate`. Links to **Nationalities** for citizenship.

**EmployeeWorkInfo** — Work assignment history with `IsCurrent` flag. Tracks `Department`, `JobTitleLevel`, `Manager`, `FromDate`/`ToDate`. Supports organizational hierarchy and role changes.

**EmployeePhones** / **EmployeeEmails** — Contact data with `IsPrimary` and `IsActive` for soft deactivation. Enforces uniqueness at the database level.

### Organization Structure

**Departments** — `DepartmentName`, `DepartmentCode`, `DepartmentDescription`.

**JobGrades** — `JobGradeName`, `JobGradeCode`, `Weight`, `LevelDescription` for hierarchy and compensation alignment.

**JobTitles** — `JobTitleName`, `JobTitleCode`, `JobTitleDescription`.

**JobTitleLevels** — Combines `JobGrade` and `JobTitle`; used by EmployeeWorkInfo, Recruitment, and Payroll.

### Recruitment Pipeline

**JobTitleProcesses** — Defines the recruitment stages per role. Each `JobTitleLevel` maps to an ordered sequence of `RecruitmentProcesses` via `SequenceOrder`. Stages can be role-specific (e.g., technical roles vs administrative).

**JobOpenings** — Vacancies tied to `JobTitleLevel` and `Department`. `NumberOfOpenings`, `PostingDate`, `ExpiryDate`, `ClosedDate`, `IsPending`.

**JobApplications** — Candidates linked to `JobOpening_id`. `ApplicantFirstName`, `ApplicantLastName`, `ApplicantEmail`, `ApplicantPhone`, `JobApplicationStatus`, `SubmissionDate`. **ApplicationAttachments** stores documents; **ApplicationAttachmentTypes** classifies them.

**ApplicationStageProgress** — Tracks each application through the pipeline. Per `JobTitleProcess_id`: `ApplicationStageProgressStatus`, `ScheduledDate`, `StartTime`/`EndTime`, `ResponsibleEmployee_id`, `Notes`. Enables stage-based workflows and accountability.

**EmploymentOffers** — Final step: `Application_id`, `JobTitleLevel_id`, `Department_id`, `ContractType_id`, `BaseSalary`, `StartDate`. `OfferStatus`, `CreatedBy`, `CreatedAt` for audit.

### Financial Engine: Payroll & Compliance

**EmployeeSalaryRecords** — Per contract: `BasicSalary`, `InsuranceSalaryBase`, `HousingAllowance`, `TransportationAllowance`, `SalaryStartDate`, `SalaryEndDate`.

**TaxBrackets** — Progressive taxation: `LowerLimit`, `TaxRate`, `ExemptionLimit`, `EffectiveDate`/`ExpierDate`, `SequenceOrder`. Supports multiple rules over time.

**InsuranceTypes** — `InsuranceType_name`, `IsMandatory`, `Description`.

**InsurancePlans** — Tied to `InsuranceType`. `CalculationBasis`, `EmployeeShareValue`, `EmployerShareValue`.

**EmployeeInsurances** — Per employee, contract, and plan. Overridable `EmployeeShareValue`, `EmployerShareValue`, `CalculatedAmount`. `StartDate`, `EndDate`, `IsActive`.

**EmployeeLoans** — Deductions: `LoanAmount`, `InstallmentsCount`, `MonthlyDeductionAmount`, `CurrentBalance`, `Status`, `SettlementDate`.

**EmployeeMonthlyEvents** — Variable pay (allowances, bonuses). Linked to `EmployeeDailyAttendance` or `EmployeePayroll`. `ComponentType_id`, `TransactionAmount`, `TransactionDate`, `IsPending`.

**EmployeePayrolls** — Net salary record: `GrossSalary`, `TotalFixedAllowances`, `TotalVariableAllowances`, `TotalTaxDeduction`, `TotalInsuranceDeduction`, `TotalLoanDeduction`, `TotalDeductions`, `NetSalary`. `EmployeePayrollStatus`, `PaymentMethod`, `PaymentDate`. Links to **EmployeeSalaryRecords** and **PayrollBatches**.

**PayrollBatches** — Groups payroll runs. `ReferenceCode`, `BatchMonth`, `TotalPay`, `PayrollBatchStatus`. Enables batch processing, approval workflows, and period-based reporting.

**EmployeePayrollTaxDetails** / **EmployeePayrollInsurances** / **EmployeePayrollLoans** — Line-level breakdown for audit and compliance.

**EmployeePaymentAccounts** — Bank/payment details. `EmployeePaymentAccountType`, `IsPrimary`, `Details`.

### Workforce Management: Shifts & Attendance

**ShiftDefinitions** — `Shift_Code`, `StartTime`, `EndTime`, `TotalShiftHours`, `PaidBreakMinutes`, `IsOverNight`, `LateThresholdMinutes`.

**EmployeeSchedules** — Assigns shifts to employees. `StartDate`, `EndDate`, `IsActive`.

**AttendanceDevices** — Physical devices: `DeviceCode`, `DeviceLocation`, `IsActive`.

**AttendanceLogs** — Raw punches: `Employee_id`, `CheckTime`, `AttendanceLogType`, `Device_id`.

**EmployeeDailyAttendance** — Aggregated daily status: `WorkDate`, `EmployeeDailyStuts`, `ActualHoursWorked`, `LateInMinutes`, `EarlyOutMinutes`. Linked to `Shift_id`.

**EmployeeBreaks** — Break intervals tied to `EmployeeDailyAttendance` for compliance and paid time.

### Leave Management

**LeaveTypes** — `DefaultDays`, `IsPaid`, `Description`.

**LeaveRequests** — `StartDate`, `EndDate`, `Dayes`, `Status`. Optional `Approver_id`, `ApprovedDate`, `Reason`.

**EmployeeLeaveBalances** — Per employee, leave type, year: `TotalDays`, `UsedDays`, `RemainingDays`.

**EmployeeLeaves** — Actual leave entries. Optional `LeaveRequest_id`, `DaysCount`.

**PublicHolidays** — `HolidayName`, `StartDate`, `EndDate`, `Dayes`, `IsPaid`, `Year`.

**WeekDays** — Weekend definitions for schedule and leave calculations.

### Employee Lifecycle: Exits

**ResignationRequests** — Employee-initiated: `ProposedLastWorkingDay`, `Reason`, `DocumentPath`, `ResignationRequestStatus`. `ReviewedBy` (User), `ReviewedAt`, `Notes`.

**EmployeeExsits** (Separation) — Actual exit: `SeparationType`, `SeparationDate`, `LastWorkingDay`. Optional `ResignationRequest_id`, `DocumentPath`. `processedBy`, `processedAt` for audit.

### Documents & Contracts

**EmployeeDocuments** — `DocumentPath`, `IssueDate`, `ExpireDate`, `IsActive`. Linked to **EmployeeDocumentTypes**.

**EmployeeContracts** — `ContractType_id`, `ContractStartDate`, `ContractEndDate`, `NoticePeriodDays`, `IsCurrent`. Optional `Document_id`.

### Identity & Access

**Users** — `User_name`, `User_passwordHashing`, `PermissionsMask`, `IsActive`, `Employee_id`.

**Permissions** — `Permission_name`, `BitValue`.

**UserLogs** — Audit: `User_id`, `Log_Time`.

---

## 🛠 Hybrid Data Access Strategy

The Infrastructure layer implements a **Purpose-Driven Data Access Strategy**, selecting the optimal tool for each operation based on performance requirements and domain integrity. The system leverages a hybrid approach combining **EF Core 9** for transactional consistency and **T-SQL** with **Raw ADO.NET** for high-performance data retrieval.

---

### 1. EF Core: Commands, Standard Reads, and Domain Integrity
All write operations and standard read operations (entity lookups, validation checks, command-side loads) utilize **EF Core 9**. This ensures:

* **Domain Integrity:** Entities enforce invariants via factory methods and private setters. EF tracks change detection and applies domain rules before persistence.
* **Transaction Scope:** Multi-entity updates (e.g., Employee + WorkInfo + Contacts) are orchestrated within a single unit-of-work transaction.
* **Referential Consistency:** Foreign key constraints, cascade behaviors, and unique indexes are configured via `IEntityTypeConfiguration<T>` and enforced by EF.
* **Unit of Work:** The `DbContext` acts as the identity map and unit of work for all command-side operations.

---

### 2. T-SQL & Raw ADO.NET: Complex Projections and Reporting
For **Complex Query-Side operations**—specifically those requiring heavy joins, computed fields, or multi-result sets—the system bypasses EF Core's overhead in favor of **T-SQL** and **Raw ADO.NET**. This approach utilizes **Stored Procedures** and **Views** to achieve:

* **Zero Mapping Overhead:** No change tracking or navigation materialization, eliminating the risk of N+1 lazy-loading in read-heavy scenarios.
* **Optimized Join Orchestration:** T-SQL encapsulates multi-table joins (e.g., merging Employees, Departments, and Managers) into a single database round-trip.
* **Granular Execution Control:** Using **Raw ADO.NET** (`ExecuteReaderAsync`, `NextResultAsync`) allows for the consumption of **multiple result sets** and precise control over the SQL execution plan.
* **Server-Side Pagination:** Operations like `OFFSET/FETCH` are executed directly within the T-SQL engine, ensuring only the requested data subset is transferred.

---

### ⚖️ Engineering Criteria: Choosing the Right Tool

| Feature | EF Core 9 | T-SQL / ADO.NET |
| :--- | :--- | :--- |
| **Primary Use Case** | Commands, Writes, and State Changes | Complex Reads, DTO Projections, Reporting |
| **Logic Location** | Domain Entities / Application Handlers | Database (Stored Procedures & Views) |
| **Object Type** | Domain Entities (Tracked) | Read-only DTOs / Views (Non-tracked) |
| **Performance** | Optimized for individual entity lifecycle | Optimized for set-based operations and joins |

---

### 💻 Code Examples: Strategy in Action

#### A. EF Core (Command-Side Entity Load)
When a handler needs to mutate state, it loads the full domain entity via EF to ensure business rules are applied.
```csharp
// UserRepository.GetByIdAsync — EF Core
return await _context.Users.FindAsync(Id);
```

#### B. T-SQL via Stored Procedure (Read Projection)
For displaying data, a specialized T-SQL SP is called to return a flattened DTO.
```csharp
// EmployeeQuerier.GetPersonalInfoByIdAsync — T-SQL via SqlQueryRaw
return _context.Database.SqlQueryRaw<EmployeePersonalInfoView>(
    "EXEC sp_GetEmployeePersonalInfo @Id = {0}", id)
    .AsEnumerable().FirstOrDefault();
```

#### C. Raw ADO.NET (Multi-Result Set Projection)
When a single call must return disparate data sets, raw ADO.NET is used for maximum efficiency.
```csharp
// EmployeeQuerier.FetchFullProfileInternal — Raw ADO.NET
var connection = _context.Database.GetDbConnection();
await using var command = connection.CreateCommand();
command.CommandText = "sp_GetEmployeeFullProfile";
command.CommandType = CommandType.StoredProcedure;

await using var reader = await ((DbCommand)command).ExecuteReaderAsync();
```

---

### 🗄️ Database-Level Logic (T-SQL)
The external SQL script SotoredProceduresAndViews.sql defines the core of the query-side performance. Complex business logic is offloaded to the T-SQL Engine via:

Flexible SPs: sp_GetEmployeePersonalInfo handles multiple lookup modes (ID, Code, National Number) in one routine.

Computed Fields: Server-side calculation of Age, Years of Service, and Status labels.

Standardized Views: Normalized projections for lookups (Departments, JobTitles) that can be reused across reporting tools.

---

## Deep Architectural Flow: The Journey of a Request

The following traces a request from the outermost layer inward, then back out as a response.

### Presentation (API): Middleware, Filtering, and Routing

**Routing** — Controllers are registered via `MapControllers()`. Attribute routing (`[Route("api/[controller]")]`) maps HTTP verbs to actions (e.g., `POST /api/User` → `CreateUser`, `GET /api/Employee/{id}` → `GetPersonalInfo`).

**Middleware** — `ExceptionMiddleware` wraps the pipeline. It invokes `_next(httpContext)`; on unhandled exception, it catches, logs, and returns a JSON error (`INTERNAL_SERVER_ERROR`, HTTP 500). The Result pattern handles expected failures; middleware catches unexpected ones.

**Filtering** — `ValidationFilter` (`IAsyncActionFilter`) runs before each action. It takes the first action argument, resolves a FluentValidation `IValidator<T>` for that type, and calls `ValidateAsync`. If invalid, it short-circuits with `BadRequestObjectResult` containing the first error (Key, Type, Args). If no validator exists, it proceeds. `AddValidatorsFromAssemblyContaining<EmployeeAddValidator>` registers validators; `ValidationFilter` is applied globally via `options.Filters.AddService<ValidationFilter>()`.

**Controller Flow** — A controller receives a command/query, invokes a Handler or Provider, checks `result.IsSuccess`, and either returns `Ok(result.Value)` or `Helpers.Result(result.Error!)` which maps `Error.Type` to HTTP status (Validation→400, NotFound→404, Conflict→409, Failure→400).

### Application: Handlers, Result Pattern, FluentValidation

**Handlers** — Command handlers (e.g., `UserHandler.AddHandle`) receive a command, perform orchestration, and return `Result<T>`. They do not throw for business failures; they return `Result.Failure(error)`. The flow: validate input (FluentValidation at API boundary, domain validation in handler) → load entities via Repository (EF) → invoke Domain or Domain Service → persist via Repository → return `Result.Successful(value)`.

**Result Pattern** — `Result<T>` is a readonly struct with `Value`, `Error`, `IsSuccess`. Domain methods (`Freeze`, `Activate`, `SetAdmin`) return `Result<User>`. Handlers propagate: `var result = user.Activate(); if (!result.IsSuccess) return Result<bool>.Failure(result.Error!);`. The API maps `Error.Type` (enErrorType: Failure, Validation, NotFound, Conflict, Unauthorized) to HTTP status via `Helpers.Result`.

**FluentValidation** — Validators (e.g., `UserAddValidator`) enforce request integrity before the handler runs: `EmployeeId > 0`, `userName` length, `password` rules, `Permissions` non-empty. Errors use `ValidationKeys` (Required, TooShort, InvalidSelection). The filter runs first; handlers receive only pre-validated commands.

### Domain: Core Logic, Value Objects, Domain Services

**Entities** — Entities encapsulate invariants via private setters and factory methods. `User.Create` requires non-null `userName`, `PasswordHashing`, valid `employeeId`. State changes (e.g., `ChangePasswordHashing`, `Activate`) return `this` or `Result<T>`.

**Value Objects** — `Password`, `EmailAddress`, `NationalNumber`, `BirthDate`, `PhoneNumber` wrap primitives and enforce invariants. `Password.Create` validates length and composition, hashes with BCrypt, returns `Result<Password>`. Plain-text never leaves the factory.

**Domain Services** — `UserService.AddUseCase` orchestrates user creation: calculates permission mask via `Permissions.Aggregate(0L, (m, p) => m | p.BitValue)`, validates mask via `Permission.IsValidPermissionMask`, creates `User` via `User.Create`. It does not touch persistence; that is the handler’s job.

### Infrastructure: Contracts for EF Core and SPs

**Repositories (EF Core)** — Implement `IUserRepository`, `IEmployeeRepository`. `AddAsync`, `UpdateAsync`, `GetByIdAsync`, `GetUserByUserName`, `GetEmployeeValidationStatus` use DbContext. `GetEmployeeValidationStatus` runs a projection: `Employees.Where(e => e.EmployeeId == id).Select(e => new { Exists, IsHr = e.WorkInfoHistory.Any(ew => ew.IsCurrent && ew.Department.DepartmentCode == "HR"), IsUser = _context.Users.Any(u => u.EmployeeId == id) })`—EF translates to a single query with joins.

**Queriers (SPs / Raw SQL)** — Implement `IEmployeeReader`, `IUserReader`. They call `Database.SqlQueryRaw` or raw ADO.NET (`GetDbConnection`, `CreateCommand`, `CommandType.StoredProcedure`, `ExecuteReaderAsync`, `NextResultAsync`). They return DTOs; no domain entity materialization.

### End-to-End Flow

1. **Request** → Routing → `ExceptionMiddleware` → `ValidationFilter` (FluentValidation) → Controller.
2. **Controller** → `_handler.AddHandle(command)` or `_provider.GetUserById(id)`.
3. **Handler** → Domain validation (e.g., `Password.Create`) → Repository (`GetEmployeeValidationStatus`, `GetByIdAsync`) → Domain Service (`UserService.AddUseCase`) → Repository (`AddAsync`) → `Result<bool>.Successful(true)`.
4. **Provider** → Querier (`GetUserByIdAsync` → `SqlQueryRaw`) or Cache (e.g., `PermissionsCache.GetAllAsync`) → `Result<UserView>.Successful(view)` or `Failure`.
5. **Controller** → `if (!result.IsSuccess) return Helpers.Result(result.Error!); return Ok(result.Value);`
6. **Response** → JSON serialization (camelCase) → client.

---

## Architecture: Inward Dependency Flow

The solution follows **Clean Architecture**. Dependencies point inward; the Domain has no knowledge of persistence, HTTP, or external frameworks. The Infrastructure layer hosts **both** EF Core and raw SQL implementations. The diagram below includes the **Caching Layer** (in-memory, used by Handlers for lookups) and the **Result Pattern** flow (Handler → Domain/Infrastructure → Result → Controller → HTTP).



- **Domain** — Entities, Value Objects, Domain Services, Result/Error. Zero project references. Defines business rules and invariants.
- **Application** — Handlers orchestrate commands; Providers serve queries. Defines contracts. Uses Cache for lookups (Permissions, Departments, etc.). Returns `Result<T>`.
- **Infrastructure** — Repositories (EF) for writes and entity loads; Queriers (SPs/Raw SQL) for complex projections. DataLoaders populate cache via EF.
- **Caching** — In-memory (`IMemoryCache`). CacheProvider calls IDataLoader on miss; TTL-based expiration (1–20 days per entity type).

---

## Caching Strategy

### In-Memory Caching (Microsoft.Extensions.Caching.Memory)

The system uses **in-memory caching** via `IMemoryCache` (registered with `AddMemoryCache()`). There is no distributed cache (Redis, SQL Server) in the current implementation. Caching is applied to **static or semi-static lookup data** that is read frequently and changes rarely.

### Where Caching Is Used

| Cache | Entity | TTL | Used By |
|-------|--------|-----|---------|
| NationalitiesCache | Nationality | 7 days | NationalityProvider, Employee creation/updates |
| DepartmentsCache | Department | 1 day | DepartmentProvider, Employee creation/updates |
| JobTitlesCache | JobTitle | 1 day | JobTitleProvider |
| JobTitleGradesCache | JobGrade | 1 day | JobGradeProvider |
| JobTitleLevelsCache | JobTitleLevel | 1 day | JobTitleLevelProvider |
| PermissionsCache | Permission | 20 days | UserHandler (AddHandle, UpdateHandle) |

**Why these entities** — Nationalities, Departments, JobTitles, JobGrades, JobTitleLevels, and Permissions are reference/lookup data. They are loaded repeatedly when creating employees, users, or validating permissions. Caching avoids round-trips to the database on every request. Permissions have the longest TTL (20 days) because they change infrequently; Nationalities have 7 days; organizational structure (Departments, JobTitles, etc.) has 1 day.

**How it works** — `CacheProvider<T>` uses `GetOrCreateAsync(cacheKey, factory)`. On cache miss, it creates a scoped service and calls `IDataLoader<T>.GetAsync()`, which loads entities from the database via EF Core (`AsNoTracking`). The result is stored with `AbsoluteExpirationRelativeToNow`. Handlers (e.g., `UserHandler`) inject `PermissionsCache` and call `GetAllAsync()` to resolve permission IDs to entities—no DB hit if the cache is warm.

### Cache Invalidation

**TTL-based expiration** — Each cache has an `AbsoluteExpirationRelativeToNow` (1–20 days). Entries expire automatically; the next request triggers a reload from the database via `IDataLoader`.

**Manual invalidation** — `CacheProvider` exposes `Invalidate()` which calls `_memoryCache.Remove(_cacheKey)`. The method is available but **not currently invoked** on mutations (e.g., when a Department or Permission is added or updated). The primary invalidation mechanism is TTL. To keep data fresh after administrative changes, either call `Invalidate()` from a mutation handler or reduce TTL for frequently updated lookups.

---

## Functional Communication: Result Pattern

Cross-layer communication avoids exceptions for expected failures. Operations return `Result<T>` with a typed `Error`. Handlers call domain or repository, receive `Result<T>`, and map to HTTP responses. Domain methods such as `Freeze()`, `Activate()`, `SetAdmin()` return `Result<User>`. The API maps `Error.Type` (Validation→400, NotFound→404, Conflict→409, Failure→400) via `Helpers.Result`. Side effects and control flow remain explicit.

---

## Domain Purity

The Domain layer stays independent of external concerns: no EF attributes, no HTTP dependencies. Value Objects (Password, EmailAddress, NationalNumber, BirthDate, PhoneNumber) encapsulate validation. Application defines interfaces; Infrastructure implements them. The Domain does not depend on storage mechanics.

---

## Engineering Rationale

### Why ADO.NET / Stored Procedures Alongside EF?

This choice is about **surgical performance**: using each technology for what it does best. The database engine excels at **joining and aggregating** large datasets—it is optimized for set-based operations, indexed lookups, and execution plan optimization. EF Core excels at **object mapping and state tracking**—it manages entity lifecycles, change detection, and transactional consistency for writes. Routing complex read projections (multi-table joins, computed fields, pagination) to SPs leverages the database's strengths; routing writes and command-side entity loads to EF leverages its strengths. The hybrid approach avoids forcing either technology into scenarios where it incurs unnecessary overhead. The result: EF handles the majority of operations (writes and standard reads), while SPs handle the minority (complex projections) with maximum efficiency.

### Why Bitwise Permissions?

Permissions use a `long` bitmask. Each permission maps to a power-of-two value; `Admin` uses `-1`. A single column and a bitwise AND yield **O(1)** authorization. No joins, no permission-table lookups per request. The mask is validated at creation via `Permission.IsValidPermissionMask()`.

### Why Value Objects?

Raw primitives are wrapped (Password, EmailAddress, NationalNumber) so invariants are enforced once, intent is explicit, and validation stays in the Domain. FluentValidation handles request shape; domain validation handles business rules.

### Why O(1) Logging?

Recording a login requires inserting a `UserLog`. Loading `User.UserLogs` would trigger a lazy load or Include of a large collection and bloat the Identity Map. A dedicated `AddNewLogAsync(UserLog)` persists the row without loading the collection—login logging stays O(1) in memory with respect to log history.

### Why EF Core Value Converters?

The domain exposes `User.UserPasswordHashing` as `Password`, not `string`. The database stores `nvarchar`. A value converter bridges the gap. The Domain never references connection strings or column types; EF performs the translation. Persistence ignorance is preserved.

---

## Module Focus: Identity & Access (Implemented Core)

The **Identity & Access** module is the gatekeeper for the entire HR ecosystem. It is **fully implemented** in code. The following describes the logic in detail.

### Bitwise Permissions: Calculation and Checking

**Enum and bit values** — `enEmployeePermissions` defines power-of-two values: `None = 0`, `ViewEmployees = 1`, `AddEmployees = 2`, `EditEmployees = 4`, `ViewOrganizations = 8`. `Admin = -1` is a sentinel (all bits set) meaning full access.

**Mask calculation** — When creating or updating a user, the handler resolves permission IDs to `Permission` entities (from cache), each with a `BitValue`. `UserService.PermissionsMaskCalculater` aggregates: `Permissions.Aggregate(0L, (currentMask, p) => currentMask | p.BitValue)`. Adding permissions: `user.ChangePermissionsMask(user.PermissionsMask | targetMask)`. Removing: `user.ChangePermissionsMask(user.PermissionsMask & ~targetMask)`.

**Check logic** — `User.HasPermission(enEmployeePermissions permission)`:

```csharp
public bool HasPermission(enEmployeePermissions permission)
{
    if (PermissionsMask == (long)enEmployeePermissions.Admin) return true;
    return (PermissionsMask & (long)permission) == (long)permission;
}
```

If the user is Admin (`PermissionsMask == -1`), any permission check passes. Otherwise, `(mask & permission) == permission` tests whether the permission’s bits are set in the mask. This is O(1); no joins or extra DB lookups.

**Validation** — `Permission.IsValidPermissionMask(mask)` ensures the mask is a subset of allowed flags: it builds `allPossiblePermissions` by OR-ing all enum values, then checks `(mask & allPossiblePermissions) == mask`. Prevents invalid or reserved bits.

### User–Employee Lifecycle: Department Restriction and Link Maintenance

**Why restrict to HR** — Only HR department employees receive system credentials. The handler calls `GetEmployeeValidationStatus(employeeId)` which returns `IsHr = e.WorkInfoHistory.Any(ew => ew.IsCurrent && ew.Department.DepartmentCode == "HR")`. If `!EmployeeValidationResult.IsHr`, the handler returns `EmployeeNotHR(employeeId)` and user creation fails. This keeps system access limited to HR staff.

**How the link is maintained** — `User` has `EmployeeId` (FK to `Employees`). `UserConfiguration` configures `HasOne(u => u.Employee).WithOne().HasForeignKey<User>(u => u.EmployeeId).OnDelete(DeleteBehavior.Restrict)`. Restrict prevents deleting an employee who has a user; the user must be removed first. `GetEmployeeValidationStatus` also returns `IsUser = _context.Users.Any(u => u.EmployeeId == id)`; if true, creation fails because that employee already has credentials. The domain enforces one user per employee.

### Password Security: Salt/Hash via the Password Value Object

**Creation** — `Password.Create(plainText)` validates: non-null, length ≥ 8, at least one uppercase and one digit. If valid, it calls `BCrypt.Net.BCrypt.HashPassword(password)`. BCrypt generates a **salt** internally and embeds it in the hash string (e.g., `$2a$11$...`). The factory returns `Result<Password>.Successful(new Password(hashedPassword))`. Plain-text never persists; only the hash is stored.

**Storage** — The database stores `nvarchar(500)` (the BCrypt hash string). EF Core value converter: `p => p.Value` (domain → DB), `v => new Password(v)` (DB → domain). The domain type remains `Password`; the DB sees a string.

**Verification** — On login, `LoginHandle` retrieves the user via `GetUserByUserName`, then calls `BCrypt.Net.BCrypt.Verify(command.password, user.UserPasswordHashing.Value)`. BCrypt extracts the salt from the stored hash and recomputes; if it matches, verification succeeds. The Password value object does not expose a verify method; verification is in the handler (Application layer) because it requires the plain-text input.

### Status Control and Audit Trail

**Status Control** — `User.Freeze()` sets `IsActive = false`; `Activate()` sets `IsActive = true`. Both return `Result<User>`; redundant transitions (e.g., activating an already active user) return `UserIsActive`. `LoginHandle` checks `if (!user.IsActive)` and returns `UserNotActive()` before granting a session.

**Audit Trail** — On successful login, `UserLog.Create(user)` instantiates a `UserLog` with `UserId` and `LogTime` (default `DateTime.UtcNow`). `AddNewLogAsync(log)` inserts directly into `UserLogs` via the repository—no load of `User.UserLogs`. The link is maintained by FK; the handler never materializes the collection.

---

## Future Ecosystem Roadmap

The database schema is **ready** for the full HR ecosystem: Recruitment, Payroll, Attendance, Leave, Employee Lifecycle, Documents, and Contracts. Tables such as `JobOpenings`, `JobApplications`, `EmployeePayrolls`, `ShiftDefinitions`, `AttendanceLogs`, `LeaveRequests`, `ResignationRequests`, and `EmployeeExsits` are defined. The **application logic** for these modules is the next phase. Each will follow the same architectural patterns: Domain entities and services, Application handlers and contracts, Infrastructure repositories (EF for writes) and queriers (SPs for complex reads). The Identity & Access module will serve as the gatekeeper for all of them—schema columns such as `ReviewedBy`, `CreatedBy`, and `processedBy` reference `Users.User_id`, establishing the dependency on this core.

---

### 🛡️ Security & Authentication Roadmap

As the system scales, the following security reinforcements are planned to enhance the **Identity & Access** core:

* **Advanced Token Management (JWT):**
    * **Refresh Tokens:** Implementing a secure re-authentication flow using Refresh Tokens stored in **HttpOnly, Secure, and SameSite** cookies to mitigate XSS risks and prevent token theft.
    * **Token Revocation:** Building a centralized "Blacklist" or "Revocation Store" to immediately invalidate Access Tokens upon logout or detecting suspicious activity.

* **Enhanced API Security:**
    * **Rate Limiting & Throttling:** Protecting sensitive endpoints (Login/Reset Password) from Brute-force and DoS attacks by limiting request frequency.
    * **Anti-Forgery Measures:** Implementing CSRF protection for any state-changing operations that rely on cookie-based authentication.

* **Cookie-Based Security:** Transitioning toward a **Hybrid Auth** approach where sensitive session identifiers are kept in encrypted, server-side cookies rather than accessible LocalStorage.

* **Permission Management (Bitwise Masks):** Enhancing the current **Bitwise Permission System** by providing a dedicated UI. This will allow administrators to visually configure, combine, and assign Permission Masks to different roles, ensuring granular and high-performance access control without changing the underlying architecture.

---

## Tech Stack

| Component | Technology |
|-----------|------------|
| Runtime | .NET 8 |
| ORM | Entity Framework Core 9 |
| Database | SQL Server |
| Read Strategy | Stored Procedures, Raw SQL (SqlQueryRaw, ADO.NET) |
| Password Hashing | BCrypt.Net-Next |
| Validation | FluentValidation |
| API | ASP.NET Core Web API, Swagger/OpenAPI |

---

## Getting Started

1. Run the SQL script `SotoredProceduresAndViews.sql` against the target database to create SPs and Views.
2. Configure the connection string in `appsettings.json`.
3. Run migrations: `dotnet ef database update --project Infrastructure --startup-project API`
4. Start the API: `dotnet run --project API`
5. Swagger UI is available at `/` in Development.
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DepartmentCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "JobGrades",
                columns: table => new
                {
                    JobGradeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobGradeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LevelDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GradeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobGrades", x => x.JobGradeId);
                });

            migrationBuilder.CreateTable(
                name: "JobTitles",
                columns: table => new
                {
                    JobTitleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobTitleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    JobTitleDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    JobTitleCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTitles", x => x.JobTitleId);
                });

            migrationBuilder.CreateTable(
                name: "Nationalities",
                columns: table => new
                {
                    NationalityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NationalityName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nationalities", x => x.NationalityId);
                });

            migrationBuilder.CreateTable(
                name: "JobTitleLevels",
                columns: table => new
                {
                    JobTitleLevelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobGradeId = table.Column<int>(type: "int", nullable: false),
                    JobTitleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTitleLevels", x => x.JobTitleLevelId);
                    table.ForeignKey(
                        name: "FK_JobTitleLevels_JobGrades_JobGradeId",
                        column: x => x.JobGradeId,
                        principalTable: "JobGrades",
                        principalColumn: "JobGradeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTitleLevels_JobTitles_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "JobTitles",
                        principalColumn: "JobTitleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeSecondName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeThirdName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeNationalityId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    HireDate = table.Column<DateTime>(type: "date", nullable: false),
                    EmployeeResignationDate = table.Column<DateTime>(type: "date", nullable: true),
                    EmployeeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EmployeeCurrentAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmployeeGender = table.Column<byte>(type: "tinyint", nullable: false),
                    IsForeign = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: false),
                    NationalNumber = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employees_Nationalities_EmployeeNationalityId",
                        column: x => x.EmployeeNationalityId,
                        principalTable: "Nationalities",
                        principalColumn: "NationalityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeEmails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeEmails_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePhones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeePhones_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeWorkInfo",
                columns: table => new
                {
                    EmployeeWorkInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    JobTitleLevelId = table.Column<int>(type: "int", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ToDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeWorkInfo", x => x.EmployeeWorkInfoId);
                    table.ForeignKey(
                        name: "FK_EmployeeWorkInfo_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeWorkInfo_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeWorkInfo_Employees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeWorkInfo_JobTitleLevels_JobTitleLevelId",
                        column: x => x.JobTitleLevelId,
                        principalTable: "JobTitleLevels",
                        principalColumn: "JobTitleLevelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Role = table.Column<byte>(type: "tinyint", nullable: false),
                    status = table.Column<byte>(type: "tinyint", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PendingAdminActions",
                columns: table => new
                {
                    ActionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionType = table.Column<byte>(type: "tinyint", nullable: false),
                    RequestedBy = table.Column<int>(type: "int", nullable: false),
                    ProcessedBy = table.Column<int>(type: "int", nullable: true),
                    TargetUserId = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    RequestReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingAdminActions", x => x.ActionId);
                    table.ForeignKey(
                        name: "FK_PendingAdminActions_Users_ProcessedBy",
                        column: x => x.ProcessedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PendingAdminActions_Users_RequestedBy",
                        column: x => x.RequestedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PendingAdminActions_Users_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    TokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<string>(type: "varchar(500)", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ReplacedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_Tokens_Tokens_ReplacedById",
                        column: x => x.ReplacedById,
                        principalTable: "Tokens",
                        principalColumn: "TokenId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LogTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_UserLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "DepartmentCode", "DepartmentDescription", "DepartmentName" },
                values: new object[,]
                {
                    { 1, "IT", "Infrastructure & Software Development", "Information Technology" },
                    { 2, "HR", "Talent Management & Employee Relations", "Human Resources" },
                    { 3, "FIN", "Financial Planning & Payroll", "Finance & Accounting" },
                    { 4, "S&M", "Revenue Generation & Brand Awareness", "Sales & Marketing" },
                    { 5, "OPS", "Core Business Operations & Logistics", "Operations" }
                });

            migrationBuilder.InsertData(
                table: "JobGrades",
                columns: new[] { "JobGradeId", "GradeCode", "JobGradeName", "LevelDescription", "Weight" },
                values: new object[,]
                {
                    { 1, "G1", "Junior Level", "Entry level for fresh graduates", 100 },
                    { 2, "G2", "Mid Level", "Individual contributor with experience", 250 },
                    { 3, "G3", "Senior Level", "Subject matter expert", 450 },
                    { 4, "G4", "Team Lead", "Technical or functional leadership", 700 },
                    { 5, "G5", "Management", "Strategic leadership and decision making", 1000 }
                });

            migrationBuilder.InsertData(
                table: "JobTitles",
                columns: new[] { "JobTitleId", "JobTitleCode", "JobTitleDescription", "JobTitleName" },
                values: new object[,]
                {
                    { 1, "BEDEV", "Developing scalable backend systems", "Backend .NET Developer" },
                    { 2, "HRGEN", "Handling day-to-day HR operations", "HR Generalist" },
                    { 3, "ACC", "Managing company accounts and taxes", "Financial Accountant" },
                    { 4, "SYSAD", "Managing server infrastructure", "System Administrator" },
                    { 5, "OPSM", "Overseeing operational workflows", "Operations Manager" }
                });

            migrationBuilder.InsertData(
                table: "Nationalities",
                columns: new[] { "NationalityId", "NationalityName" },
                values: new object[,]
                {
                    { 1, "Egyptian" },
                    { 2, "Saudi" },
                    { 3, "Emirati" },
                    { 4, "Jordanian" },
                    { 5, "American" }
                });

            migrationBuilder.InsertData(
                table: "JobTitleLevels",
                columns: new[] { "JobTitleLevelId", "JobGradeId", "JobTitleId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 3, 1 },
                    { 4, 4, 1 },
                    { 5, 1, 2 },
                    { 6, 2, 2 },
                    { 7, 4, 5 },
                    { 8, 5, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentCode",
                table: "Departments",
                column: "DepartmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentName",
                table: "Departments",
                column: "DepartmentName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeEmails_EmployeeId_IsPrimary",
                table: "EmployeeEmails",
                columns: new[] { "EmployeeId", "IsPrimary" },
                unique: true,
                filter: "[IsPrimary] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePhones_EmployeeId_IsPrimary",
                table: "EmployeePhones",
                columns: new[] { "EmployeeId", "IsPrimary" },
                unique: true,
                filter: "[IsPrimary] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeCode",
                table: "Employees",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeNationalityId",
                table: "Employees",
                column: "EmployeeNationalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_WorkInfo_Current_Unique",
                table: "EmployeeWorkInfo",
                columns: new[] { "EmployeeId", "IsCurrent" },
                unique: true,
                filter: "[IsCurrent] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeWorkInfo_DepartmentId",
                table: "EmployeeWorkInfo",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeWorkInfo_JobTitleLevelId",
                table: "EmployeeWorkInfo",
                column: "JobTitleLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeWorkInfo_ManagerId",
                table: "EmployeeWorkInfo",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_JobGrades_GradeCode",
                table: "JobGrades",
                column: "GradeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobGrades_JobGradeName",
                table: "JobGrades",
                column: "JobGradeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobGrades_Weight",
                table: "JobGrades",
                column: "Weight",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobTitleLevels_JobGradeId",
                table: "JobTitleLevels",
                column: "JobGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTitleLevels_JobTitleId_JobGradeId",
                table: "JobTitleLevels",
                columns: new[] { "JobTitleId", "JobGradeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobTitles_JobTitleCode",
                table: "JobTitles",
                column: "JobTitleCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobTitles_JobTitleName",
                table: "JobTitles",
                column: "JobTitleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nationalities_NationalityName",
                table: "Nationalities",
                column: "NationalityName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PendingAdminActions_ProcessedBy",
                table: "PendingAdminActions",
                column: "ProcessedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PendingAdminActions_RequestedBy",
                table: "PendingAdminActions",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PendingAdminActions_TargetUserId",
                table: "PendingAdminActions",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_ReplacedById",
                table: "Tokens",
                column: "ReplacedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_UserId",
                table: "Tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogs_UserId",
                table: "UserLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmployeeId",
                table: "Users",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeEmails");

            migrationBuilder.DropTable(
                name: "EmployeePhones");

            migrationBuilder.DropTable(
                name: "EmployeeWorkInfo");

            migrationBuilder.DropTable(
                name: "PendingAdminActions");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "UserLogs");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "JobTitleLevels");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "JobGrades");

            migrationBuilder.DropTable(
                name: "JobTitles");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Nationalities");
        }
    }
}

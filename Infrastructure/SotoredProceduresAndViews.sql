-----------------------------------------------------------
-- 1. Stored Procedure: Get Employee Personal Info (Single)
-----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetEmployeePersonalInfo
    @Id INT = NULL,
    @Code NVARCHAR(50) = NULL,
    @NationalNumber NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TargetId INT;
    IF @Id IS NOT NULL SET @TargetId = @Id;
    ELSE IF @Code IS NOT NULL SELECT @TargetId = EmployeeId FROM Employees WHERE EmployeeCode = @Code;
    ELSE IF @NationalNumber IS NOT NULL SELECT @TargetId = EmployeeId FROM Employees WHERE NationalNumber = @NationalNumber;

    SELECT 
        e.EmployeeId AS Id,
        e.EmployeeFirstName AS FirstName, 
        e.EmployeeSecondName AS SecondName,
        e.EmployeeThirdName AS ThirdName, 
        e.EmployeeLastName AS LastName,
        e.NationalNumber, 
        e.EmployeeCode AS Code,
        CAST(FLOOR(DATEDIFF(DAY, e.BirthDate, GETDATE()) / 365.25) AS int) AS Age,
        e.EmployeeCurrentAddress AS CurrentAddress,
        n.NationalityName AS Nationality,
        CASE e.EmployeeGender WHEN 1 THEN 'Male' WHEN 2 THEN 'Female' ELSE 'Other' END AS Gender,
        CASE e.Status 
            WHEN 1 THEN 'Active' 
            WHEN 2 THEN 'Resigned' 
            WHEN 3 THEN 'Terminated' 
            WHEN 4 THEN 'Retired' 
            WHEN 5 THEN 'On Leave' 
            WHEN 6 THEN 'Suspended' 
            ELSE 'Unknown' 
        END AS StatusName
    FROM Employees e
    LEFT JOIN Nationalities n ON e.EmployeeNationalityId = n.NationalityId
    WHERE e.EmployeeId = @TargetId;
END
GO






-----------------------------------------------------------
-- 2. Stored Procedure: Get Employee Work Info (Single)
-----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetEmployeeWorkInfo
    @Id INT = NULL,
    @Code NVARCHAR(50) = NULL,
    @NationalNumber NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TargetId INT;
    IF @Id IS NOT NULL SET @TargetId = @Id;
    ELSE IF @Code IS NOT NULL SELECT @TargetId = EmployeeId FROM Employees WHERE EmployeeCode = @Code;
    ELSE IF @NationalNumber IS NOT NULL SELECT @TargetId = EmployeeId FROM Employees WHERE NationalNumber = @NationalNumber;

    SELECT 
        e.EmployeeId AS Id,
        e.EmployeeFirstName AS FirstName, 
        e.EmployeeLastName AS LastName,
        e.EmployeeCode AS Code,
        e.HireDate, 
        e.EmployeeResignationDate,
        d.DepartmentName AS Department, 
        jg.JobGradeName + ' ' + jt.JobTitleName AS JobTitleLevel,
        jg.Weight AS Weight,
        CAST(CASE WHEN EXISTS (SELECT 1 FROM EmployeeWorkInfo WHERE ManagerId = e.EmployeeId) THEN 1 ELSE 0 END AS BIT) AS IsManager,
        ISNULL(m.EmployeeFirstName + ' ' + m.EmployeeLastName, 'N/A (Top Level)') AS ManagerName,
        ISNULL(m.EmployeeCode, 'N/A') AS ManagerCode
    FROM Employees e
    LEFT JOIN EmployeeWorkInfo ew ON e.EmployeeId = ew.EmployeeId AND ew.IsCurrent = 1
    LEFT JOIN Departments d ON ew.DepartmentId = d.DepartmentId
    LEFT JOIN JobTitleLevels jtl ON ew.JobTitleLevelId = jtl.JobTitleLevelId
    LEFT JOIN JobTitles jt ON jtl.JobTitleId = jt.JobTitleId
    LEFT JOIN JobGrades jg on jtl.JobGradeId = jg.JobGradeId
    LEFT JOIN Employees m ON ew.ManagerId = m.EmployeeId
    WHERE e.EmployeeId = @TargetId;
END
GO





-----------------------------------------------------------
-- 3. Stored Procedure: Get Employee Full Profile (Multiple Results)
-----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetEmployeeFullProfile
    @Id INT = NULL,
    @Code NVARCHAR(50) = NULL,
    @NationalNumber NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TargetId INT;
    IF @Id IS NOT NULL SET @TargetId = @Id;
    ELSE IF @Code IS NOT NULL SELECT @TargetId = EmployeeId FROM Employees WHERE EmployeeCode = @Code;
    ELSE IF @NationalNumber IS NOT NULL SELECT @TargetId = EmployeeId FROM Employees WHERE NationalNumber = @NationalNumber;

  
    SELECT 
        e.EmployeeId AS Id, 
        e.EmployeeFirstName AS FirstName, 
        e.EmployeeSecondName AS SecondName,
        e.EmployeeThirdName AS ThirdName, 
        e.EmployeeLastName AS LastName,
        e.NationalNumber, 
        e.EmployeeCode AS Code, 
        e.EmployeeCurrentAddress AS CurrentAddress,
        d.DepartmentName AS Department, 
        jg.JobGradeName + ' ' + jt.JobTitleName AS JobTitleLevel, 
        e.HireDate,
        CAST(FLOOR(DATEDIFF(DAY, e.BirthDate, GETDATE()) / 365.25) AS int) AS Age,
        CASE e.Status 
            WHEN 1 THEN 'Active' WHEN 2 THEN 'Resigned' WHEN 3 THEN 'Terminated' 
            WHEN 4 THEN 'Retired' WHEN 5 THEN 'On Leave' WHEN 6 THEN 'Suspended' 
            ELSE 'Unknown' END AS StatusName,
        n.NationalityName AS Nationality,
        CASE e.EmployeeGender WHEN 1 THEN 'Male' ELSE 'Female' END AS Gender,
        CAST(CASE WHEN EXISTS (SELECT 1 FROM EmployeeWorkInfo WHERE ManagerId = e.EmployeeId) THEN 1 ELSE 0 END AS BIT) AS IsManager,
        ISNULL(m.EmployeeFirstName + ' ' + m.EmployeeLastName, 'N/A (Top Level)') AS ManagerName
    FROM Employees e
    LEFT JOIN Nationalities n ON e.EmployeeNationalityId = n.NationalityId
    LEFT JOIN EmployeeWorkInfo ew ON e.EmployeeId = ew.EmployeeId AND ew.IsCurrent = 1
    LEFT JOIN Departments d ON ew.DepartmentId = d.DepartmentId
    LEFT JOIN JobTitleLevels jtl ON ew.JobTitleLevelId = jtl.JobTitleLevelId
    LEFT JOIN JobTitles jt ON jtl.JobTitleId = jt.JobTitleId
    LEFT JOIN JobGrades jg on jtl.JobGradeId = jg.JobGradeId
    LEFT JOIN Employees m ON ew.ManagerId = m.EmployeeId
    WHERE e.EmployeeId = @TargetId;

    
    SELECT Id, PhoneNumber AS Number, IsPrimary, IsActive 
    FROM EmployeePhones 
    WHERE EmployeeId = @TargetId;

    
    SELECT Id, Email AS Email, IsPrimary 
    FROM EmployeeEmails 
    WHERE EmployeeId = @TargetId;
END
GO






-----------------------------------------------------------
-- 4. Stored Procedure: Paged Personal Info
-----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetEmployeePersonalInfoPaged
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        e.EmployeeId AS Id,
        e.EmployeeFirstName AS FirstName, 
        e.EmployeeSecondName AS SecondName,
        e.EmployeeThirdName AS ThirdName, 
        e.EmployeeLastName AS LastName,
        e.NationalNumber, 
        e.EmployeeCode AS Code,
        CAST(FLOOR(DATEDIFF(DAY, e.BirthDate, GETDATE()) / 365.25) AS int) AS Age,
        e.EmployeeCurrentAddress AS CurrentAddress,
        n.NationalityName AS Nationality,
        CASE e.EmployeeGender WHEN 1 THEN 'Male' WHEN 2 THEN 'Female' ELSE 'Other' END AS Gender,
        CASE e.Status 
            WHEN 1 THEN 'Active' WHEN 2 THEN 'Resigned' ELSE 'Other'
        END AS StatusName
    FROM Employees e
    LEFT JOIN Nationalities n ON e.EmployeeNationalityId = n.NationalityId
    ORDER BY e.EmployeeId
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO





-----------------------------------------------------------
-- 5. Stored Procedure: Paged Work Info
-----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetEmployeeWorkInfoPaged
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        e.EmployeeId AS Id,
        e.EmployeeFirstName AS FirstName, 
        e.EmployeeLastName AS LastName,
        e.EmployeeCode AS Code,
        e.HireDate,
        
        e.EmployeeResignationDate AS EmployeeResignationDate,
        d.DepartmentName AS Department, 
        jg.JobGradeName + ' ' + jt.JobTitleName AS JobTitleLevel,
        jg.Weight AS Weight,
        
        
        CAST(0 AS BIT) AS IsManager,
        NULL AS ManagerName,
        NULL AS ManagerCode
        
    FROM Employees e
    LEFT JOIN EmployeeWorkInfo ew ON e.EmployeeId = ew.EmployeeId AND ew.IsCurrent = 1
    LEFT JOIN Departments d ON ew.DepartmentId = d.DepartmentId
    LEFT JOIN JobTitleLevels jtl ON ew.JobTitleLevelId = jtl.JobTitleLevelId
    LEFT JOIN JobTitles jt ON jtl.JobTitleId = jt.JobTitleId
    LEFT JOIN JobGrades jg on jtl.JobGradeId = jg.JobGradeId
    ORDER BY e.EmployeeId
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO







-----------------------------------------------------------
-- 6. Stored Procedure: User Info
-----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetUser
    @Id INT = NULL,
    @Code NVARCHAR(20) = NULL,
    @NationalNumber VARCHAR(14) = NULL,
    @UserName NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TargetUserId INT;

    IF @Id IS NOT NULL 
    BEGIN
        SET @TargetUserId = @Id;
    END
    ELSE IF @Code IS NOT NULL 
    BEGIN
        SELECT @TargetUserId = u.UserId 
        FROM Users u 
        INNER JOIN Employees e ON u.EmployeeId = e.EmployeeId 
        WHERE e.EmployeeCode = @Code;
    END
    ELSE IF @NationalNumber IS NOT NULL 
    BEGIN
        SELECT @TargetUserId = u.UserId 
        FROM Users u 
        INNER JOIN Employees e ON u.EmployeeId = e.EmployeeId 
        WHERE e.NationalNumber = @NationalNumber;
    END
    ELSE IF @UserName IS NOT NULL
    BEGIN
        SELECT @TargetUserId = UserId FROM Users WHERE UserName = @UserName;
    END

   
    SELECT 
        u.UserId AS Id,
        CONCAT_WS(' ', e.EmployeeFirstName, e.EmployeeCode, e.EmployeeThirdName, e.EmployeeLastName) AS FullName,
        e.EmployeeCode AS Code,
        u.UserName AS Username,
        u.IsActive
    FROM Users u
    INNER JOIN Employees e ON u.EmployeeId = e.EmployeeId
    WHERE u.UserId = @TargetUserId;
END
GO







-----------------------------------------------------------
-- 5. Stored Procedure: Paged User Info
-----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetUserPaged
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        u.UserId AS Id,
        CONCAT_WS(' ', e.EmployeeFirstName, e.EmployeeCode, e.EmployeeThirdName, e.EmployeeLastName) AS FullName,
        e.EmployeeCode AS Code,
        u.UserName AS Username,
        u.IsActive
    FROM Users u
    INNER JOIN Employees e ON u.EmployeeId = e.EmployeeId
    ORDER BY u.UserId
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO











-----------------------------------------------------------
-- 6. Views
-----------------------------------------------------------
CREATE OR ALTER VIEW v_DepartmentView AS
SELECT DepartmentId AS Id, DepartmentName AS Name, DepartmentCode AS Code, DepartmentDescription AS Description FROM Departments;
GO

CREATE OR ALTER VIEW v_JobTitleView AS
SELECT JobTitleId AS Id, JobTitleName AS Name, JobTitleCode AS Code, JobTitleDescription AS Description FROM JobTitles;
GO

CREATE OR ALTER VIEW v_JobGradeView AS
SELECT JobGradeId AS Id, JobGradeName AS Name, GradeCode AS Code, Weight AS Weigth, LevelDescription AS Description FROM JobGrades;
GO

CREATE OR ALTER VIEW v_JobTitleLevelView AS
SELECT 
    jtl.JobTitleLevelId AS Id,
    jg.JobGradeName + ' ' + jt.JobTitleName AS FullTitle,
    jg.Weight AS GradeWeight
FROM JobTitleLevels jtl
JOIN JobTitles jt ON jtl.JobTitleId = jt.JobTitleId
JOIN JobGrades jg ON jtl.JobGradeId = jg.JobGradeId;
GO

CREATE OR ALTER VIEW v_NationalityView AS
SELECT NationalityId AS Id, NationalityName AS Name FROM Nationalities;
GO
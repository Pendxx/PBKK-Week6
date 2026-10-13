-- ==========================================================
-- Student Registration App
-- Struktur Database SQL Server
-- Tabel Program Studi dan Tabel Mahasiswa
-- Relasi 1 : N (One-to-Many)
-- ==========================================================

-- 1. Membuat Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'StudentRegistrationDB')
BEGIN
    CREATE DATABASE StudentRegistrationDB;
END
GO

USE StudentRegistrationDB;
GO

-- 2. Tabel Programs (Program Studi)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Programs')
BEGIN
    CREATE TABLE Programs
    (
        ProgramId INT IDENTITY(1,1)
            CONSTRAINT PK_Programs PRIMARY KEY,
        ProgramCode VARCHAR(20) NOT NULL
            CONSTRAINT UQ_Programs_ProgramCode UNIQUE,
        ProgramName VARCHAR(100) NOT NULL,
        IsActive BIT NOT NULL
            CONSTRAINT DF_Programs_IsActive DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL
            CONSTRAINT DF_Programs_CreatedAt DEFAULT SYSDATETIME()
    );
END
GO

-- 3. Tabel Students (Mahasiswa)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
BEGIN
    CREATE TABLE Students
    (
        StudentId INT IDENTITY(1,1)
            CONSTRAINT PK_Students PRIMARY KEY,
        NIM VARCHAR(20) NOT NULL
            CONSTRAINT UQ_Students_NIM UNIQUE,
        Name VARCHAR(100) NOT NULL,
        ProgramId INT NOT NULL,
        BirthDate DATE NULL,
        Address VARCHAR(250) NULL,
        PhoneNumber VARCHAR(20) NULL,
        CreatedAt DATETIME2 NOT NULL
            CONSTRAINT DF_Students_CreatedAt DEFAULT SYSDATETIME(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT FK_Students_Programs
            FOREIGN KEY (ProgramId)
            REFERENCES Programs(ProgramId)
    );
END
GO

-- 4. Insert Data Program Studi
IF NOT EXISTS (SELECT * FROM Programs)
BEGIN
    INSERT INTO Programs (ProgramCode, ProgramName)
    VALUES
        ('TI', 'Teknik Informatika'),
        ('SI', 'Sistem Informasi'),
        ('MNJ', 'Manajemen'),
        ('TE', 'Teknik Elektro'),
        ('DKV', 'Desain Komunikasi Visual');
END
GO

-- 5. Insert Data Mahasiswa (Contoh)
IF NOT EXISTS (SELECT * FROM Students)
BEGIN
    INSERT INTO Students (NIM, Name, ProgramId, BirthDate, Address, PhoneNumber)
    VALUES
        ('20231001', 'Ahmad Rizki', 1, '2003-03-12', 'Jl. Merdeka No. 123, Surabaya', '081234567890'),
        ('20231002', 'Siti Aisyah', 2, '2004-07-21', 'Jl. Diponegoro No. 45, Surabaya', '082233445566'),
        ('20231003', 'Budi Santoso', 1, '2003-11-05', 'Jl. Ahmad Yani No. 10, Surabaya', '081355667788'),
        ('20231004', 'Citra Lestari', 3, '2004-01-18', 'Jl. Darmo No. 20, Surabaya', '089900112233'),
        ('20231005', 'Dimas Prayoga', 4, '2003-09-27', 'Jl. Raya ITS, Surabaya', '081288776655');
END
GO

-- 6. Query Menampilkan Data Mahasiswa
SELECT 
    s.StudentId,
    s.NIM,
    s.Name,
    p.ProgramCode,
    p.ProgramName,
    s.BirthDate,
    s.Address,
    s.PhoneNumber,
    s.CreatedAt,
    s.UpdatedAt
FROM Students s
INNER JOIN Programs p ON s.ProgramId = p.ProgramId
ORDER BY s.StudentId;

-- 7. Query Pencarian Mahasiswa (Contoh: @Search = 'Ahmad')
/*
DECLARE @Search VARCHAR(50) = 'Ahmad';
SELECT 
    s.StudentId,
    s.NIM,
    s.Name,
    p.ProgramName,
    s.BirthDate,
    s.PhoneNumber
FROM Students s
INNER JOIN Programs p ON s.ProgramId = p.ProgramId
WHERE
    s.NIM LIKE '%' + @Search + '%'
    OR s.Name LIKE '%' + @Search + '%'
    OR p.ProgramName LIKE '%' + @Search + '%'
ORDER BY s.StudentId;
*/

-- 8. Query Jumlah Mahasiswa
-- Total mahasiswa
SELECT COUNT(*) AS TotalStudents
FROM Students;

-- Jumlah mahasiswa per program studi
SELECT 
    p.ProgramName,
    COUNT(s.StudentId) AS TotalStudents
FROM Programs p
LEFT JOIN Students s ON p.ProgramId = s.ProgramId
GROUP BY p.ProgramId, p.ProgramName
ORDER BY TotalStudents DESC;

-- 9. Query Edit dan Hapus
-- Edit mahasiswa
/*
UPDATE Students
SET 
    NIM = @NIM,
    Name = @Name,
    ProgramId = @ProgramId,
    BirthDate = @BirthDate,
    Address = @Address,
    PhoneNumber = @PhoneNumber,
    UpdatedAt = SYSDATETIME()
WHERE StudentId = @StudentId;
*/

-- Hapus mahasiswa
/*
DELETE FROM Students
WHERE StudentId = @StudentId;
*/

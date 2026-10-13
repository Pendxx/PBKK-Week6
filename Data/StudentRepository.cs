using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using StudentRegistrationApp.Models;

namespace StudentRegistrationApp.Data
{
    public class StudentRepository : IStudentRepository
    {
        private string _sqlServerConnectionString = "Server=localhost;Database=StudentRegistrationDB;Trusted_Connection=True;TrustServerCertificate=True;";
        private readonly string _sqlitePath;
        private readonly string _sqliteConnectionString;
        private bool _useSqlServer = false;

        public string CurrentProviderName => _useSqlServer ? "SQL Server (StudentRegistrationDB)" : "SQLite (Local Fallback)";
        public string CurrentConnectionString => _useSqlServer ? _sqlServerConnectionString : _sqliteConnectionString;

        public StudentRepository()
        {
            var appData = AppDomain.CurrentDomain.BaseDirectory;
            var rootCandidate = Path.GetFullPath(Path.Combine(appData, @"..\..\..\..\StudentRegistrationDB.db"));
            if (File.Exists(rootCandidate))
            {
                _sqlitePath = rootCandidate;
            }
            else
            {
                _sqlitePath = Path.Combine(appData, "StudentRegistrationDB.db");
            }
            _sqliteConnectionString = $"Data Source={_sqlitePath}";

            InitializeDatabase();
        }

        public void SetConnectionString(string connectionString)
        {
            _sqlServerConnectionString = connectionString;
        }

        public async Task<bool> TryConnectSqlServerAsync(string connectionString)
        {
            try
            {
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();
                _sqlServerConnectionString = connectionString;
                _useSqlServer = true;
                await EnsureSqlServerSchemaAsync(conn);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void InitializeDatabase()
        {
            // First check if SQL Server is accessible
            try
            {
                using var conn = new SqlConnection(_sqlServerConnectionString);
                conn.Open();
                _useSqlServer = true;
                EnsureSqlServerSchema(conn);
                return;
            }
            catch
            {
                // SQL Server is not reachable currently, fallback to SQLite
                _useSqlServer = false;
                EnsureSqliteSchema();
            }
        }

        private void EnsureSqlServerSchema(SqlConnection conn)
        {
            var sqlScript = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Programs')
                BEGIN
                    CREATE TABLE Programs
                    (
                        ProgramId INT IDENTITY(1,1) CONSTRAINT PK_Programs PRIMARY KEY,
                        ProgramCode VARCHAR(20) NOT NULL CONSTRAINT UQ_Programs_ProgramCode UNIQUE,
                        ProgramName VARCHAR(100) NOT NULL,
                        IsActive BIT NOT NULL CONSTRAINT DF_Programs_IsActive DEFAULT 1,
                        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Programs_CreatedAt DEFAULT SYSDATETIME()
                    );

                    INSERT INTO Programs (ProgramCode, ProgramName) VALUES
                    ('TI', 'Teknik Informatika'),
                    ('SI', 'Sistem Informasi'),
                    ('MNJ', 'Manajemen'),
                    ('TE', 'Teknik Elektro'),
                    ('DKV', 'Desain Komunikasi Visual');
                END;

                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
                BEGIN
                    CREATE TABLE Students
                    (
                        StudentId INT IDENTITY(1,1) CONSTRAINT PK_Students PRIMARY KEY,
                        NIM VARCHAR(20) NOT NULL CONSTRAINT UQ_Students_NIM UNIQUE,
                        Name VARCHAR(100) NOT NULL,
                        ProgramId INT NOT NULL,
                        BirthDate DATE NULL,
                        Address VARCHAR(250) NULL,
                        PhoneNumber VARCHAR(20) NULL,
                        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Students_CreatedAt DEFAULT SYSDATETIME(),
                        UpdatedAt DATETIME2 NULL,
                        CONSTRAINT FK_Students_Programs FOREIGN KEY (ProgramId) REFERENCES Programs(ProgramId)
                    );

                    INSERT INTO Students (NIM, Name, ProgramId, BirthDate, Address, PhoneNumber) VALUES
                    ('20231001', 'Ahmad Rizki', 1, '2003-03-12', 'Jl. Merdeka No. 123, Surabaya', '081234567890'),
                    ('20231002', 'Siti Aisyah', 2, '2004-07-21', 'Jl. Diponegoro No. 45, Surabaya', '082233445566'),
                    ('20231003', 'Budi Santoso', 1, '2003-11-05', 'Jl. Ahmad Yani No. 10, Surabaya', '081355667788'),
                    ('20231004', 'Citra Lestari', 3, '2004-01-18', 'Jl. Darmo No. 20, Surabaya', '089900112233'),
                    ('20231005', 'Dimas Prayoga', 4, '2003-09-27', 'Jl. Raya ITS, Surabaya', '081288776655');
                END;";

            using var cmd = new SqlCommand(sqlScript, conn);
            cmd.ExecuteNonQuery();
        }

        private async Task EnsureSqlServerSchemaAsync(SqlConnection conn)
        {
            var sqlScript = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Programs')
                BEGIN
                    CREATE TABLE Programs
                    (
                        ProgramId INT IDENTITY(1,1) CONSTRAINT PK_Programs PRIMARY KEY,
                        ProgramCode VARCHAR(20) NOT NULL CONSTRAINT UQ_Programs_ProgramCode UNIQUE,
                        ProgramName VARCHAR(100) NOT NULL,
                        IsActive BIT NOT NULL CONSTRAINT DF_Programs_IsActive DEFAULT 1,
                        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Programs_CreatedAt DEFAULT SYSDATETIME()
                    );

                    INSERT INTO Programs (ProgramCode, ProgramName) VALUES
                    ('TI', 'Teknik Informatika'),
                    ('SI', 'Sistem Informasi'),
                    ('MNJ', 'Manajemen'),
                    ('TE', 'Teknik Elektro'),
                    ('DKV', 'Desain Komunikasi Visual');
                END;

                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
                BEGIN
                    CREATE TABLE Students
                    (
                        StudentId INT IDENTITY(1,1) CONSTRAINT PK_Students PRIMARY KEY,
                        NIM VARCHAR(20) NOT NULL CONSTRAINT UQ_Students_NIM UNIQUE,
                        Name VARCHAR(100) NOT NULL,
                        ProgramId INT NOT NULL,
                        BirthDate DATE NULL,
                        Address VARCHAR(250) NULL,
                        PhoneNumber VARCHAR(20) NULL,
                        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Students_CreatedAt DEFAULT SYSDATETIME(),
                        UpdatedAt DATETIME2 NULL,
                        CONSTRAINT FK_Students_Programs FOREIGN KEY (ProgramId) REFERENCES Programs(ProgramId)
                    );

                    INSERT INTO Students (NIM, Name, ProgramId, BirthDate, Address, PhoneNumber) VALUES
                    ('20231001', 'Ahmad Rizki', 1, '2003-03-12', 'Jl. Merdeka No. 123, Surabaya', '081234567890'),
                    ('20231002', 'Siti Aisyah', 2, '2004-07-21', 'Jl. Diponegoro No. 45, Surabaya', '082233445566'),
                    ('20231003', 'Budi Santoso', 1, '2003-11-05', 'Jl. Ahmad Yani No. 10, Surabaya', '081355667788'),
                    ('20231004', 'Citra Lestari', 3, '2004-01-18', 'Jl. Darmo No. 20, Surabaya', '089900112233'),
                    ('20231005', 'Dimas Prayoga', 4, '2003-09-27', 'Jl. Raya ITS, Surabaya', '081288776655');
                END;";

            using var cmd = new SqlCommand(sqlScript, conn);
            await cmd.ExecuteNonQueryAsync();
        }

        private void EnsureSqliteSchema()
        {
            using var conn = new SqliteConnection(_sqliteConnectionString);
            conn.Open();

            var sqliteScript = @"
                CREATE TABLE IF NOT EXISTS Programs (
                    ProgramId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProgramCode TEXT NOT NULL UNIQUE,
                    ProgramName TEXT NOT NULL,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS Students (
                    StudentId INTEGER PRIMARY KEY AUTOINCREMENT,
                    NIM TEXT NOT NULL UNIQUE,
                    Name TEXT NOT NULL,
                    ProgramId INTEGER NOT NULL,
                    BirthDate TEXT NULL,
                    Address TEXT NULL,
                    PhoneNumber TEXT NULL,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    UpdatedAt TEXT NULL,
                    FOREIGN KEY (ProgramId) REFERENCES Programs(ProgramId)
                );";

            using (var cmd = new SqliteCommand(sqliteScript, conn))
            {
                cmd.ExecuteNonQuery();
            }

            // Seed Programs if empty
            using (var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM Programs;", conn))
            {
                long count = (long)(checkCmd.ExecuteScalar() ?? 0);
                if (count == 0)
                {
                    using var insertCmd = new SqliteCommand(@"
                        INSERT INTO Programs (ProgramCode, ProgramName) VALUES
                        ('TI', 'Teknik Informatika'),
                        ('SI', 'Sistem Informasi'),
                        ('MNJ', 'Manajemen'),
                        ('TE', 'Teknik Elektro'),
                        ('DKV', 'Desain Komunikasi Visual');", conn);
                    insertCmd.ExecuteNonQuery();
                }
            }

            // Seed Students if empty
            using (var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM Students;", conn))
            {
                long count = (long)(checkCmd.ExecuteScalar() ?? 0);
                if (count == 0)
                {
                    using var insertCmd = new SqliteCommand(@"
                        INSERT INTO Students (NIM, Name, ProgramId, BirthDate, Address, PhoneNumber) VALUES
                        ('20231001', 'Ahmad Rizki', 1, '2003-03-12', 'Jl. Merdeka No. 123, Surabaya', '081234567890'),
                        ('20231002', 'Siti Aisyah', 2, '2004-07-21', 'Jl. Diponegoro No. 45, Surabaya', '082233445566'),
                        ('20231003', 'Budi Santoso', 1, '2003-11-05', 'Jl. Ahmad Yani No. 10, Surabaya', '081355667788'),
                        ('20231004', 'Citra Lestari', 3, '2004-01-18', 'Jl. Darmo No. 20, Surabaya', '089900112233'),
                        ('20231005', 'Dimas Prayoga', 4, '2003-09-27', 'Jl. Raya ITS, Surabaya', '081288776655');", conn);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        // ==========================================
        // 6. Query Menampilkan Data Mahasiswa
        // ==========================================
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            var list = new List<Student>();
            string query = @"
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
                    s.UpdatedAt,
                    s.ProgramId
                FROM Students s
                INNER JOIN Programs p ON s.ProgramId = p.ProgramId
                ORDER BY s.StudentId;";

            if (_useSqlServer)
            {
                using var conn = new SqlConnection(_sqlServerConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                int row = 1;
                while (await reader.ReadAsync())
                {
                    list.Add(ReadStudentFromReader(reader, row++));
                }
            }
            else
            {
                using var conn = new SqliteConnection(_sqliteConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqliteCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                int row = 1;
                while (await reader.ReadAsync())
                {
                    list.Add(ReadStudentFromReader(reader, row++));
                }
            }

            return list;
        }

        // ==========================================
        // 7. Query Pencarian Mahasiswa
        // ==========================================
        public async Task<List<Student>> SearchStudentsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllStudentsAsync();

            var list = new List<Student>();
            string query = @"
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
                    s.UpdatedAt,
                    s.ProgramId
                FROM Students s
                INNER JOIN Programs p ON s.ProgramId = p.ProgramId
                WHERE
                    s.NIM LIKE @Search
                    OR s.Name LIKE @Search
                    OR p.ProgramName LIKE @Search
                ORDER BY s.StudentId;";

            string searchPattern = $"%{keyword.Trim()}%";

            if (_useSqlServer)
            {
                using var conn = new SqlConnection(_sqlServerConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Search", searchPattern);
                using var reader = await cmd.ExecuteReaderAsync();
                int row = 1;
                while (await reader.ReadAsync())
                {
                    list.Add(ReadStudentFromReader(reader, row++));
                }
            }
            else
            {
                using var conn = new SqliteConnection(_sqliteConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@Search", searchPattern);
                using var reader = await cmd.ExecuteReaderAsync();
                int row = 1;
                while (await reader.ReadAsync())
                {
                    list.Add(ReadStudentFromReader(reader, row++));
                }
            }

            return list;
        }

        public async Task<Student?> GetStudentByIdAsync(int studentId)
        {
            string query = @"
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
                    s.UpdatedAt,
                    s.ProgramId
                FROM Students s
                INNER JOIN Programs p ON s.ProgramId = p.ProgramId
                WHERE s.StudentId = @StudentId;";

            if (_useSqlServer)
            {
                using var conn = new SqlConnection(_sqlServerConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return ReadStudentFromReader(reader, 1);
                }
            }
            else
            {
                using var conn = new SqliteConnection(_sqliteConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return ReadStudentFromReader(reader, 1);
                }
            }

            return null;
        }

        // ==========================================
        // 5 & Insert Student
        // ==========================================
        public async Task<bool> AddStudentAsync(Student student)
        {
            string query = @"
                INSERT INTO Students (NIM, Name, ProgramId, BirthDate, Address, PhoneNumber)
                VALUES (@NIM, @Name, @ProgramId, @BirthDate, @Address, @PhoneNumber);";

            if (_useSqlServer)
            {
                using var conn = new SqlConnection(_sqlServerConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NIM", student.NIM);
                cmd.Parameters.AddWithValue("@Name", student.Name);
                cmd.Parameters.AddWithValue("@ProgramId", student.ProgramId);
                cmd.Parameters.AddWithValue("@BirthDate", (object?)student.BirthDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object?)student.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object?)student.PhoneNumber ?? DBNull.Value);
                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
            else
            {
                using var conn = new SqliteConnection(_sqliteConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@NIM", student.NIM);
                cmd.Parameters.AddWithValue("@Name", student.Name);
                cmd.Parameters.AddWithValue("@ProgramId", student.ProgramId);
                cmd.Parameters.AddWithValue("@BirthDate", student.BirthDate.HasValue ? student.BirthDate.Value.ToString("yyyy-MM-dd") : DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object?)student.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object?)student.PhoneNumber ?? DBNull.Value);
                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
        }

        // ==========================================
        // 9. Query Edit Mahasiswa
        // ==========================================
        public async Task<bool> UpdateStudentAsync(Student student)
        {
            string querySqlServer = @"
                UPDATE Students
                SET 
                    NIM = @NIM,
                    Name = @Name,
                    ProgramId = @ProgramId,
                    BirthDate = @BirthDate,
                    Address = @Address,
                    PhoneNumber = @PhoneNumber,
                    UpdatedAt = SYSDATETIME()
                WHERE StudentId = @StudentId;";

            string querySqlite = @"
                UPDATE Students
                SET 
                    NIM = @NIM,
                    Name = @Name,
                    ProgramId = @ProgramId,
                    BirthDate = @BirthDate,
                    Address = @Address,
                    PhoneNumber = @PhoneNumber,
                    UpdatedAt = datetime('now')
                WHERE StudentId = @StudentId;";

            if (_useSqlServer)
            {
                using var conn = new SqlConnection(_sqlServerConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(querySqlServer, conn);
                cmd.Parameters.AddWithValue("@StudentId", student.StudentId);
                cmd.Parameters.AddWithValue("@NIM", student.NIM);
                cmd.Parameters.AddWithValue("@Name", student.Name);
                cmd.Parameters.AddWithValue("@ProgramId", student.ProgramId);
                cmd.Parameters.AddWithValue("@BirthDate", (object?)student.BirthDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object?)student.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object?)student.PhoneNumber ?? DBNull.Value);
                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
            else
            {
                using var conn = new SqliteConnection(_sqliteConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqliteCommand(querySqlite, conn);
                cmd.Parameters.AddWithValue("@StudentId", student.StudentId);
                cmd.Parameters.AddWithValue("@NIM", student.NIM);
                cmd.Parameters.AddWithValue("@Name", student.Name);
                cmd.Parameters.AddWithValue("@ProgramId", student.ProgramId);
                cmd.Parameters.AddWithValue("@BirthDate", student.BirthDate.HasValue ? student.BirthDate.Value.ToString("yyyy-MM-dd") : DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object?)student.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object?)student.PhoneNumber ?? DBNull.Value);
                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
        }

        // ==========================================
        // 9. Query Hapus Mahasiswa
        // ==========================================
        public async Task<bool> DeleteStudentAsync(int studentId)
        {
            string query = "DELETE FROM Students WHERE StudentId = @StudentId;";

            if (_useSqlServer)
            {
                using var conn = new SqlConnection(_sqlServerConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
            else
            {
                using var conn = new SqliteConnection(_sqliteConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
        }

        public async Task<List<ProgramItem>> GetAllProgramsAsync()
        {
            var list = new List<ProgramItem>();
            string query = "SELECT ProgramId, ProgramCode, ProgramName, IsActive FROM Programs WHERE IsActive = 1 ORDER BY ProgramId;";

            if (_useSqlServer)
            {
                using var conn = new SqlConnection(_sqlServerConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new ProgramItem
                    {
                        ProgramId = reader.GetInt32(0),
                        ProgramCode = reader.GetString(1),
                        ProgramName = reader.GetString(2),
                        IsActive = reader.GetBoolean(3)
                    });
                }
            }
            else
            {
                using var conn = new SqliteConnection(_sqliteConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqliteCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new ProgramItem
                    {
                        ProgramId = reader.GetInt32(0),
                        ProgramCode = reader.GetString(1),
                        ProgramName = reader.GetString(2),
                        IsActive = Convert.ToInt32(reader[3]) == 1
                    });
                }
            }

            return list;
        }

        // ==========================================
        // 8. Query Jumlah Mahasiswa (Total & Per Prodi)
        // ==========================================
        public async Task<int> GetTotalStudentsCountAsync()
        {
            string query = "SELECT COUNT(*) FROM Students;";

            if (_useSqlServer)
            {
                using var conn = new SqlConnection(_sqlServerConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(query, conn);
                object? result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result ?? 0);
            }
            else
            {
                using var conn = new SqliteConnection(_sqliteConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqliteCommand(query, conn);
                object? result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result ?? 0);
            }
        }

        public async Task<List<ProgramStatistic>> GetStudentStatisticsByProgramAsync()
        {
            var list = new List<ProgramStatistic>();
            string query = @"
                SELECT 
                    p.ProgramCode,
                    p.ProgramName,
                    COUNT(s.StudentId) AS TotalStudents
                FROM Programs p
                LEFT JOIN Students s ON p.ProgramId = s.ProgramId
                GROUP BY p.ProgramId, p.ProgramCode, p.ProgramName
                ORDER BY TotalStudents DESC;";

            int maxCount = 1;

            if (_useSqlServer)
            {
                using var conn = new SqlConnection(_sqlServerConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    int count = Convert.ToInt32(reader[2]);
                    if (count > maxCount) maxCount = count;
                    list.Add(new ProgramStatistic
                    {
                        ProgramCode = reader.GetString(0),
                        ProgramName = reader.GetString(1),
                        TotalStudents = count
                    });
                }
            }
            else
            {
                using var conn = new SqliteConnection(_sqliteConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqliteCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    int count = Convert.ToInt32(reader[2]);
                    if (count > maxCount) maxCount = count;
                    list.Add(new ProgramStatistic
                    {
                        ProgramCode = reader.GetString(0),
                        ProgramName = reader.GetString(1),
                        TotalStudents = count
                    });
                }
            }

            // Assign vibrant colors and proportional BarHeight for UI chart
            string[] colors = new[] { "#3B82F6", "#06B6D4", "#F59E0B", "#8B5CF6", "#EC4899", "#10B981" };
            for (int i = 0; i < list.Count; i++)
            {
                list[i].BarColor = colors[i % colors.Length];
                list[i].BarHeight = Math.Max(15, (list[i].TotalStudents / (double)maxCount) * 110.0);
            }

            return list;
        }

        private Student ReadStudentFromReader(IDataReader reader, int rowNumber)
        {
            var s = new Student
            {
                RowNumber = rowNumber,
                StudentId = Convert.ToInt32(reader["StudentId"]),
                NIM = reader["NIM"].ToString() ?? "",
                Name = reader["Name"].ToString() ?? "",
                ProgramCode = reader["ProgramCode"].ToString() ?? "",
                ProgramName = reader["ProgramName"].ToString() ?? "",
                Address = reader["Address"] is DBNull ? null : reader["Address"].ToString(),
                PhoneNumber = reader["PhoneNumber"] is DBNull ? null : reader["PhoneNumber"].ToString(),
                ProgramId = Convert.ToInt32(reader["ProgramId"])
            };

            if (reader["BirthDate"] != DBNull.Value)
            {
                var val = reader["BirthDate"];
                if (val is DateTime dt)
                {
                    s.BirthDate = dt;
                }
                else if (DateTime.TryParse(val.ToString(), out DateTime parsed))
                {
                    s.BirthDate = parsed;
                }
            }

            if (reader["CreatedAt"] != DBNull.Value)
            {
                var val = reader["CreatedAt"];
                if (val is DateTime dt) s.CreatedAt = dt;
                else if (DateTime.TryParse(val.ToString(), out DateTime parsed)) s.CreatedAt = parsed;
            }

            if (reader["UpdatedAt"] != DBNull.Value)
            {
                var val = reader["UpdatedAt"];
                if (val is DateTime dt) s.UpdatedAt = dt;
                else if (DateTime.TryParse(val.ToString(), out DateTime parsed)) s.UpdatedAt = parsed;
            }

            return s;
        }
    }
}

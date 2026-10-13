using System.Collections.Generic;
using System.Threading.Tasks;
using StudentRegistrationApp.Models;

namespace StudentRegistrationApp.Data
{
    public interface IStudentRepository
    {
        string CurrentProviderName { get; }
        string CurrentConnectionString { get; }

        Task<List<Student>> GetAllStudentsAsync();
        Task<List<Student>> SearchStudentsAsync(string keyword);
        Task<Student?> GetStudentByIdAsync(int studentId);
        Task<bool> AddStudentAsync(Student student);
        Task<bool> UpdateStudentAsync(Student student);
        Task<bool> DeleteStudentAsync(int studentId);

        Task<List<ProgramItem>> GetAllProgramsAsync();
        Task<int> GetTotalStudentsCountAsync();
        Task<List<ProgramStatistic>> GetStudentStatisticsByProgramAsync();

        Task<bool> TryConnectSqlServerAsync(string connectionString);
        void SetConnectionString(string connectionString);
    }
}

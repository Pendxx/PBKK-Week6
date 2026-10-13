using System;

namespace StudentRegistrationApp.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string NIM { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ProgramId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Joined fields from Programs table
        public string ProgramCode { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;

        // Display helper
        public int RowNumber { get; set; }
        public string FormattedBirthDate => BirthDate.HasValue ? BirthDate.Value.ToString("dd/MM/yyyy") : "-";

        public Student Clone()
        {
            return new Student
            {
                StudentId = this.StudentId,
                NIM = this.NIM,
                Name = this.Name,
                ProgramId = this.ProgramId,
                BirthDate = this.BirthDate,
                Address = this.Address,
                PhoneNumber = this.PhoneNumber,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt,
                ProgramCode = this.ProgramCode,
                ProgramName = this.ProgramName,
                RowNumber = this.RowNumber
            };
        }
    }
}

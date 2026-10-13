using System;

namespace StudentRegistrationApp.Models
{
    public class ProgramItem
    {
        public int ProgramId { get; set; }
        public string ProgramCode { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"{ProgramCode} - {ProgramName}";
        }
    }
}

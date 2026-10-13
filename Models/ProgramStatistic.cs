namespace StudentRegistrationApp.Models
{
    public class ProgramStatistic
    {
        public string ProgramCode { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public double BarHeight { get; set; }
        public string BarColor { get; set; } = "#3B82F6";
    }
}

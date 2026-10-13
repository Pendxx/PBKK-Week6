using System;

namespace StudentRegistrationApp.Models
{
    public class ActivityLog
    {
        public string Title { get; set; } = string.Empty;
        public string TimeAgo { get; set; } = string.Empty;
        public string ActionType { get; set; } = "Add"; // Add, Update, Delete
        public string BadgeColor => ActionType switch
        {
            "Add" => "#10B981",    // Green
            "Update" => "#3B82F6", // Blue
            "Delete" => "#EF4444", // Red
            _ => "#6B7280"
        };
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}

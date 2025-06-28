using System;

namespace VelzonModerna.ViewModels.CRM
{
    public class LogViewModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string? UserId { get; set; }
        public Guid? LeadId { get; set; }
    }
}
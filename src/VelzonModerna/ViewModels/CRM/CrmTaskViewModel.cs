using GeneralLabSolutions.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VelzonModerna.ViewModels.CRM
{
    public class CrmTaskViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskPriority Priority { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CrmTaskStatus Status { get; set; }

        public Guid LeadId { get; set; }
        public string? Assignee { get; set; }
        public int Progress { get; set; }
    }
}
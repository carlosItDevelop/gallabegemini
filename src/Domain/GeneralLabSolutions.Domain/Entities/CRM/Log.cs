using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities.CRM
{
    public class Log : EntityBase
    {
        public Log() { }

        public Log(string type, string title, string? description, string? userId, Guid? leadId)
        {
            Type = type;
            Title = title;
            Description = description;
            UserId = userId;
            LeadId = leadId;
            Timestamp = DateTime.UtcNow;
        }

        public string Type { get; private set; } // Ex: "lead_creation", "status_change"
        public string Title { get; private set; }
        public string? Description { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string? UserId { get; private set; }

        // Relacionamento com Lead (opcional)
        public Guid? LeadId { get; private set; }
        public virtual Lead? Lead { get; private set; }
    }
}

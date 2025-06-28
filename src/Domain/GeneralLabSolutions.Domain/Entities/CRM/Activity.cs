using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities.Audit;

namespace GeneralLabSolutions.Domain.Entities.CRM
{
    public class Activity : EntityAudit
    {
        public Activity() { }

        public Activity(Guid? leadId, string type, string title, string? description, DateTime scheduledDate)
        {
            LeadId = leadId;
            Type = type;
            Title = title;
            Description = description;
            ScheduledDate = scheduledDate;
        }

        public string Type { get; private set; } // Ex: "call", "meeting", "email"
        public string Title { get; private set; }
        public string? Description { get; private set; }
        public DateTime ScheduledDate { get; private set; }

        // Relacionamento com Lead (pode ser nulo, pois uma atividade pode não estar atrelada a um lead)
        public Guid? LeadId { get; private set; }
        public virtual Lead? Lead { get; private set; }
    }
}

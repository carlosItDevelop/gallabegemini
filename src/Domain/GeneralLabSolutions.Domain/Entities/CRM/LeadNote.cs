using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities.CRM
{
    public class LeadNote : EntityAudit
    {
        public LeadNote() { }

        public LeadNote(Guid leadId, string content, string color, string? userId)
        {
            LeadId = leadId;
            Content = content;
            Color = color;
            UserId = userId;
        }

        public string Content { get; private set; }
        public string Color { get; private set; } // Ex: "blue", "yellow"
        public string? UserId { get; private set; }

        // Relacionamento com Lead
        public Guid LeadId { get; private set; }
        public virtual Lead Lead { get; private set; }
    }
}

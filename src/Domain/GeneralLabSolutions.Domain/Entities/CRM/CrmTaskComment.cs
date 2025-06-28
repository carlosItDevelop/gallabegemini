using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities.CRM
{
    public class CrmTaskComment : EntityAudit
    {
        public CrmTaskComment() { }

        public CrmTaskComment(Guid crmTaskId, string content, string? userId)
        {
            CrmTaskId = crmTaskId;
            Content = content;
            UserId = userId; // ID do usuário que comentou
        }

        public string Content { get; private set; }
        public string? UserId { get; private set; }

        // Relacionamento com CrmTask
        public Guid CrmTaskId { get; set; }
        public virtual CrmTask CrmTask { get; set; }
    }
}

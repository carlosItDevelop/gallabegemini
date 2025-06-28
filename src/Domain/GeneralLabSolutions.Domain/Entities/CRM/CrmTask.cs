using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Entities.CRM;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities.CRM
{
    public class CrmTask : EntityAudit
    {
        // EF Core
        public CrmTask() { }

        public CrmTask(Guid leadId, string title, string? description, DateTime? dueDate, TaskPriority priority, string? assignee)
        {
            LeadId = leadId;
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            Assignee = assignee;

            // Valores padrão
            Status = CrmTaskStatus.Pendente;
            Progress = 0;
            SortOrder = 0;
        }

        public string Title { get; private set; }
        public string? Description { get; private set; }
        public DateTime? DueDate { get; private set; }
        public TaskPriority Priority { get; private set; } // Reutilizando seu Enum existente
        public CrmTaskStatus Status { get; private set; }
        public string? Assignee { get; private set; } // Responsável pela tarefa
        public int Progress { get; private set; }
        public int SortOrder { get; private set; }

        // Relacionamento com Lead
        public Guid LeadId { get; private set; }
        public virtual Lead Lead { get; private set; }

        // Relacionamentos com as novas entidades filhas
        public virtual ICollection<CrmTaskComment> Comments { get; private set; } = new List<CrmTaskComment>();
        public virtual ICollection<CrmTaskAttachment> Attachments { get; private set; } = new List<CrmTaskAttachment>();

        // Métodos de negócio
        public void UpdateProgress(int progress)
        {
            if (progress < 0)
                progress = 0;
            if (progress > 100)
                progress = 100;
            Progress = progress;

            if (Progress == 100)
                Status = CrmTaskStatus.Concluida;
            else if (Status == CrmTaskStatus.Concluida)
                Status = CrmTaskStatus.Pendente;
        }

        public void CompleteTask() => Status = CrmTaskStatus.Concluida;
        public void ReopenTask() => Status = CrmTaskStatus.Pendente;
    }
}

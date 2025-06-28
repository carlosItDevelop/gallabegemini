using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.DomainObjects;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities.CRM
{
    public class Lead : EntityAudit, IAggregateRoot
    {
        // Construtor para o EF Core
        public Lead() { }

        // Construtor principal
        public Lead(string name, string email, string? company = null, string? phone = null, string? position = null, string? source = null, string? responsible = null)
        {
            Name = name;
            Email = email;
            Company = company;
            Phone = phone;
            Position = position;
            Source = source;
            Responsible = responsible;

            // Valores padrão definidos aqui
            Status = LeadStatus.Novo;
            Temperature = LeadTemperature.Morno;
            Score = 50;
            Value = 0;
            LastContact = DateTime.UtcNow;
        }

        public string Name { get; private set; }
        public string? Company { get; private set; }
        public string Email { get; private set; }
        public string? Phone { get; private set; }
        public string? Position { get; private set; }
        public string? Source { get; private set; }
        public string? Responsible { get; private set; }
        public int Score { get; private set; }
        public decimal Value { get; private set; }
        public string? Notes { get; private set; }
        public DateTime? LastContact { get; private set; }

        // Usando os Enums que você já criou
        public LeadStatus Status { get; private set; }
        public LeadTemperature Temperature { get; private set; }


        // --- INÍCIO DAS NOVAS COLEÇÕES ---
        public virtual ICollection<CrmTask> Tasks { get; private set; } = new List<CrmTask>();
        public virtual ICollection<Activity> Activities { get; private set; } = new List<Activity>();
        public virtual ICollection<LeadNote> LeadNotes { get; private set; } = new List<LeadNote>();
        public virtual ICollection<Log> Logs { get; private set; } = new List<Log>();
        // --- FIM DAS NOVAS COLEÇÕES ---

        // Métodos para modificar a entidade (exemplo)
        public void UpdateStatus(LeadStatus newStatus)
        {
            Status = newStatus;
            // Adicionar evento de domínio se necessário
        }

        public void UpdateTemperature(LeadTemperature newTemperature)
        {
            Temperature = newTemperature;
        }
    }
}
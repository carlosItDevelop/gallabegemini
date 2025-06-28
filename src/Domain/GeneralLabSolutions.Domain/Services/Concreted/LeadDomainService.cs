using GeneralLabSolutions.Domain.Entities.CRM;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Interfaces.CRM;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;

namespace GeneralLabSolutions.Domain.Services.Concreted
{
    public class LeadDomainService : BaseService, ILeadDomainService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IGenericRepository<Lead, Guid> _query;

        public LeadDomainService(INotificador notificador,
                                 ILeadRepository leadRepository,
                                 IGenericRepository<Lead, Guid> query) : base(notificador)
        {
            _leadRepository = leadRepository;
            _query = query;
        }

        // --- MÉTODOS DE VALIDAÇÃO ASSÍNCRONOS ---
        private async Task<bool> ValidateAddLead(Lead lead)
        {
            if ((await _query.SearchAsync(l => l.Email == lead.Email)).Any())
            {
                Notificar("Já existe um Lead com o e-mail informado.");
                return false;
            }
            // if (!ExecutarValidacao(new LeadValidation(), lead)) return false; // Adicionar quando a validação for criada
            return true;
        }

        private async Task<bool> ValidateUpdateLead(Lead lead)
        {
            if ((await _query.SearchAsync(l => l.Email == lead.Email && l.Id != lead.Id)).Any())
            {
                Notificar("O e-mail informado já está em uso por outro Lead.");
                return false;
            }
            // if (!ExecutarValidacao(new LeadValidation(), lead)) return false;
            return true;
        }

        private async Task<bool> ValidateDeleteLead(Guid id)
        {
            var lead = await _leadRepository.GetByIdAsync(id); // Precisamos carregar o lead com suas coleções
            if (lead == null)
            {
                Notificar("Lead não encontrado.");
                return false;
            }
            if (lead.Tasks.Any())
            {
                Notificar("Não é possível excluir um Lead que possui tarefas. Remova as tarefas primeiro.");
                return false;
            }
            return true;
        }


        // --- MÉTODOS DE SERVIÇO ASSÍNCRONOS ---
        public async Task AddLeadAsync(Lead lead)
        {
            if (!await ValidateAddLead(lead))
                return;
            await _leadRepository.AddAsync(lead);
        }

        public async Task UpdateLeadAsync(Lead lead)
        {
            if (!await ValidateUpdateLead(lead))
                return;
            await _leadRepository.UpdateAsync(lead);
        }

        public async Task DeleteLeadAsync(Guid id)
        {
            // A validação já verifica se o lead existe
            if (!await ValidateDeleteLead(id))
                return;
            var leadToDelete = await _leadRepository.GetByIdAsync(id);
            await _leadRepository.DeleteAsync(leadToDelete);
        }
    }
}
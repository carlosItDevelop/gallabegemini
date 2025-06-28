using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.CRM;

namespace GeneralLabSolutions.Domain.Services.Abstractions
{
    public interface ILeadDomainService
    {
        Task AddLeadAsync(Lead lead);
        Task UpdateLeadAsync(Lead lead);
        Task DeleteLeadAsync(Guid id);
        // Outros métodos de negócio podem ser adicionados aqui
    }
}
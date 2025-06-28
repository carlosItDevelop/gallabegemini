using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.CRM;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Services.Abstractions
{
    public interface ICrmTaskDomainService
    {
        Task AddTaskAsync(CrmTask task);
        Task UpdateTaskAsync(CrmTask task);
        Task DeleteTaskAsync(Guid id);
        Task UpdateTaskStatusAsync(Guid taskId, CrmTaskStatus newStatus);
    }
}
using GeneralLabSolutions.Domain.Entities;

namespace GeneralLabSolutions.Domain.Services.Abstractions
{
    public interface IKanbanTaskDomainService
    {
        Task<bool> ValidarAddKanbanTaskAsync(KanbanTask model);
        Task AddkanbanTaskAsync(KanbanTask model);
    }

}

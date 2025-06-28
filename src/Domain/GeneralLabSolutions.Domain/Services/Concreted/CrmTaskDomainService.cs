using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.CRM;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Interfaces.CRM;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;

namespace GeneralLabSolutions.Domain.Services.Concreted
{
    public class CrmTaskDomainService : BaseService, ICrmTaskDomainService
    {
        private readonly ICrmTaskRepository _taskRepository;

        public CrmTaskDomainService(INotificador notificador, ICrmTaskRepository taskRepository) : base(notificador)
        {
            _taskRepository = taskRepository;
        }

        public async Task AddTaskAsync(CrmTask task)
        {
            // Validações aqui
            await _taskRepository.AddAsync(task);
        }

        public async Task UpdateTaskAsync(CrmTask task)
        {
            // Validações aqui
            await _taskRepository.UpdateAsync(task);
        }

        public async Task DeleteTaskAsync(Guid id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                Notificar("Tarefa não encontrada.");
                return;
            }
            await _taskRepository.DeleteAsync(task);
        }

        public async Task UpdateTaskStatusAsync(Guid taskId, CrmTaskStatus newStatus)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                Notificar("Tarefa não encontrada.");
                return;
            }

            // A lógica de mudar o status já está na própria entidade
            if (newStatus == CrmTaskStatus.Concluida)
                task.CompleteTask();
            else
                task.ReopenTask();

            await _taskRepository.UpdateAsync(task);
        }
    }
}
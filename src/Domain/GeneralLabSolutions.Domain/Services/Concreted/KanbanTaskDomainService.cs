using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;

namespace GeneralLabSolutions.Domain.Services.Concreted
{
    public class KanbanTaskDomainService : BaseService, IKanbanTaskDomainService
    {

        private readonly IKanbanTaskRepository _kanbanTaskRepository;
        private readonly IGenericRepository<KanbanTask, Guid> _query;

        public KanbanTaskDomainService(INotificador notificador, IKanbanTaskRepository kanbanTaskRepository, IGenericRepository<KanbanTask, Guid> query) : base(notificador)
        {
            _kanbanTaskRepository = kanbanTaskRepository;
            _query = query;
        }

        public async Task<bool> ValidarAddKanbanTaskAsync(KanbanTask model)
        {
            #region: Regras de Negócios Agrupadas para evitar várias interrupções: Add

            bool isValid = true;

            if (_query.SearchAsync(c => c.Title == model.Title).Result.Any())
            {
                Notificar("Já existe uma Tarefa com este Título informado.");
                isValid = false;
            }

            #endregion

            return await Task.FromResult(isValid);

        }

        public async Task AddkanbanTaskAsync(KanbanTask model)
        {
            // Verifica as regras de negócio e validações
            if (!await ValidarAddKanbanTaskAsync(model))
                return;


            //model.AdicionarEvento(new AddKanbanTaskRegistrado)

            await _kanbanTaskRepository.AddAsync(model);

            // Todo: PersistirDados (Podemos fazer isso daqui, chamando "PersistirDados" ou na Controller)
        }

    }
}

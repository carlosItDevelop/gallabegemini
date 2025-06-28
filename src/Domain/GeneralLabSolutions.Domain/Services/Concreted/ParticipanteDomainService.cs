using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;

namespace GeneralLabSolutions.Domain.Services.Concreted
{
    public class ParticipanteDomainService : BaseService, IParticipanteDomainService
    {

        private readonly IParticipanteRepository _participanteRepository;
        private readonly IGenericRepository<Participante, Guid> _query;

        public ParticipanteDomainService(INotificador notificador, 
                                         IParticipanteRepository participanteRepository,      
                                         IGenericRepository<Participante, Guid> query) 
                                         : base(notificador)
        {
            _participanteRepository = participanteRepository;
            _query = query;
        }

        public async Task<bool> ValidarAddParticipanteAsync(Participante model)
        {
            #region: Regras de Negócios Agrupadas para evitar várias interrupções: Add

            bool isValid = true;

            if (_query.SearchAsync(c => c.Name == model.Name).Result.Any())
            {
                Notificar("Já existe um Participante com este Nome informado.");
                isValid = false;
            }

            if (_query.SearchAsync(c => c.Email == model.Email).Result.Any())
            {
                Notificar("Já existe um Participante com este Email informado.");
                isValid = false;
            }

            #endregion

            return await Task.FromResult(isValid);
        }

        public async Task AddParticipanteAsync(Participante model)
        {
            // Verifica as regras de negócio e validações
            if (!await ValidarAddParticipanteAsync(model))
                return;


            //model.AdicionarEvento(new AddKanbanTaskRegistrado)

            await _participanteRepository.AddAsync(model);

            // Todo: PersistirDados (Podemos fazer isso daqui, chamando "PersistirDados" ou na Controller)
        }


    }
}

using GeneralLabSolutions.Domain.Entities;

namespace GeneralLabSolutions.Domain.Services.Abstractions
{
    public interface IParticipanteDomainService
    {
        Task<bool> ValidarAddParticipanteAsync(Participante model);
        Task AddParticipanteAsync(Participante model);

    }

}

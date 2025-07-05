using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Services.Helpers
{
    public interface IPessoaContainer
    {
        Pessoa Pessoa { get; }
        Guid Id { get; }
        void AdicionarEvento(Event evento);
    }
}
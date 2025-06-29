using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Entities
{
    public interface IPessoaContainer
    {
        Pessoa Pessoa { get; }
        Guid Id { get; }
        void AdicionarEvento(Event evento);
    }
}
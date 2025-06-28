using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.Domain.Enums; // Para TipoDeContato

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class ContatoAdicionadoEvent : DomainEvent
    {
        public Guid ContatoId { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Telefone { get; private set; }
        public TipoDeContato TipoDeContato { get; private set; }
        // Outras propriedades como EmailAlternativo, etc., podem ser adicionadas se necessário para os handlers

        // AggregateId aqui será o Cliente.Id (ou Fornecedor.Id / Vendedor.Id)
        public ContatoAdicionadoEvent(Guid aggregateId, Guid contatoId, string nome, string email, string telefone, TipoDeContato tipoDeContato)
            : base(aggregateId)
        {
            ContatoId = contatoId;
            Nome = nome;
            Email = email;
            Telefone = telefone;
            TipoDeContato = tipoDeContato;
        }
    }
}
using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.Domain.Enums; // Para TipoDeContato

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class ContatoAtualizadoEvent : DomainEvent
    {
        public Guid ContatoId { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Telefone { get; private set; }
        public TipoDeContato TipoDeContato { get; private set; }
        // Pode incluir valores antigos/novos ou apenas os novos

        // AggregateId será o ID do Cliente/Fornecedor/Vendedor ao qual o contato pertence
        public ContatoAtualizadoEvent(Guid aggregateId, Guid contatoId, string nome, string email, string telefone, TipoDeContato tipoDeContato)
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
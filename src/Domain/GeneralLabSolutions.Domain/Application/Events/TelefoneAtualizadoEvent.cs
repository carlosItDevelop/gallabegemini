using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.Domain.Enums; // Para TipoDeTelefone

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class TelefoneAtualizadoEvent : DomainEvent
    {
        public Guid TelefoneId { get; private set; }
        // Poderia incluir valores antigos/novos se relevante para os handlers
        public string DDD { get; private set; }
        public string Numero { get; private set; }
        public TipoDeTelefone TipoDeTelefone { get; private set; }

        // AggregateId será o ID do Cliente/Fornecedor/Vendedor ao qual o telefone pertence
        public TelefoneAtualizadoEvent(Guid aggregateId, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoDeTelefone)
            : base(aggregateId)
        {
            TelefoneId = telefoneId;
            DDD = ddd;
            Numero = numero;
            TipoDeTelefone = tipoDeTelefone;
        }
    }
}
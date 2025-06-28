using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.Domain.Enums; // Para TipoDeTelefone

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class TelefoneAdicionadoEvent : DomainEvent
    {
        // Id do Telefone criado
        public Guid TelefoneId { get; private set; }
        // Dados do telefone adicionado
        public string DDD { get; private set; }
        public string Numero { get; private set; }
        public TipoDeTelefone TipoDeTelefone { get; private set; }

        // AggregateId aqui será o Cliente.Id (ou Fornecedor.Id / Vendedor.Id)
        // Estamos adicionando um Telefone *ao* agregado raiz.
        public TelefoneAdicionadoEvent(Guid aggregateId, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoDeTelefone)
            : base(aggregateId) // Passa o ID do Cliente/Fornecedor/Vendedor para a classe base
        {
            TelefoneId = telefoneId;
            DDD = ddd;
            Numero = numero;
            TipoDeTelefone = tipoDeTelefone;
        }
    }
}
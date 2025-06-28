using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    // QuantidadeItemOrcamentoAtualizadaEvent.cs
    public class QuantidadeItemOrcamentoAtualizadaEvent : DomainEvent
    {
        public Guid ItemOrcamentoId { get; }
        public int NovaQuantidade { get; }
        public QuantidadeItemOrcamentoAtualizadaEvent(Guid orcamentoId, Guid itemOrcamentoId, int novaQuantidade) : base(orcamentoId)
        {
            ItemOrcamentoId = itemOrcamentoId;
            NovaQuantidade = novaQuantidade;
        }
    }

    // OrcamentoConvertidoEmPedidoVendaEvent.cs (a ser implementado quando tivermos o Pedido de Venda)
    // OrcamentoCanceladoEvent.cs
}

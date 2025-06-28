using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    // ItemRemovidoDoOrcamentoEvent.cs
    public class ItemRemovidoDoOrcamentoEvent : DomainEvent
    {
        public Guid ItemOrcamentoId { get; }
        public ItemRemovidoDoOrcamentoEvent(Guid orcamentoId, Guid itemOrcamentoId) : base(orcamentoId)
        {
            ItemOrcamentoId = itemOrcamentoId;
        }
    }

    // OrcamentoConvertidoEmPedidoVendaEvent.cs (a ser implementado quando tivermos o Pedido de Venda)
    // OrcamentoCanceladoEvent.cs
}

// PedidoDeCompraCriadoEvent.cs
using GeneralLabSolutions.Domain.Mensageria;

public class NotificarItemOrcamentoCompraConcluidaEvent : Event
{
    public Guid ItemOrcamentoId { get; }
    public Guid PedidoDeCompraId { get; }
    public Guid ItemPedidoDeCompraId { get; }
    public NotificarItemOrcamentoCompraConcluidaEvent(Guid itemOrcamentoId, Guid pedidoDeCompraId, Guid itemPedidoDeCompraId)
    {
        base.AggregateId = itemOrcamentoId;
        ItemOrcamentoId = itemOrcamentoId;
        PedidoDeCompraId = pedidoDeCompraId;
        ItemPedidoDeCompraId = itemPedidoDeCompraId;
    }
}

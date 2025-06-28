// PedidoDeCompraCriadoEvent.cs
using GeneralLabSolutions.Domain.Mensageria;

public class NotificarItemOrcamentoCompraCanceladaEvent : Event
{
    public Guid ItemOrcamentoId { get; }
    public Guid PedidoDeCompraId { get; }
    public Guid ItemPedidoDeCompraId { get; }
    public NotificarItemOrcamentoCompraCanceladaEvent(Guid itemOrcamentoId, Guid pedidoDeCompraId, Guid itemPedidoDeCompraId)
    {
        base.AggregateId = itemOrcamentoId;
        ItemOrcamentoId = itemOrcamentoId;
        PedidoDeCompraId = pedidoDeCompraId;
        ItemPedidoDeCompraId = itemPedidoDeCompraId;
    }
}
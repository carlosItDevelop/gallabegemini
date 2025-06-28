// PedidoDeCompraCriadoEvent.cs
using GeneralLabSolutions.Domain.Mensageria;
// ItemRemovidoDoPedidoDeCompraEvent.cs
public class ItemRemovidoDoPedidoDeCompraEvent : DomainEvent
{
    public Guid ItemPedidoDeCompraId { get; }
    public ItemRemovidoDoPedidoDeCompraEvent(Guid pedidoCompraId, Guid itemPedidoDeCompraId) : base(pedidoCompraId)
    {
        ItemPedidoDeCompraId = itemPedidoDeCompraId;
    }
}

// PedidoDeCompraCriadoEvent.cs
using GeneralLabSolutions.Domain.Mensageria;
// ItemAdicionadoAoPedidoDeCompraEvent.cs
public class ItemAdicionadoAoPedidoDeCompraEvent : DomainEvent
{
    public Guid ItemPedidoDeCompraId { get; }
    public Guid ProdutoId { get; }
    public int Quantidade { get; }
    public decimal ValorUnitario { get; }
    public ItemAdicionadoAoPedidoDeCompraEvent(Guid pedidoCompraId, Guid itemPedidoDeCompraId, Guid produtoId, int quantidade, decimal valorUnitario) : base(pedidoCompraId)
    {
        ItemPedidoDeCompraId = itemPedidoDeCompraId;
        ProdutoId = produtoId;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
    }
}

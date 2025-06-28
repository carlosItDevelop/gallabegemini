// PedidoDeCompraCriadoEvent.cs
using GeneralLabSolutions.Domain.Mensageria;
// ItemPedidoDeCompraRecebidoEvent.cs (Pode ser disparado pelo método RegistrarRecebimentoItem na entidade ItemPedidoDeCompra, mas adicionado à lista de eventos do PedidoDeCompra)
public class ItemPedidoDeCompraRecebidoEvent : DomainEvent // O AggregateId aqui é o PedidoDeCompraId
{
    public Guid ItemPedidoDeCompraId { get; }
    public Guid ProdutoId { get; }
    public int QuantidadeRecebida { get; }
    public bool HouveDivergencia { get; }
    public string? Observacao { get; }

    public ItemPedidoDeCompraRecebidoEvent(Guid pedidoCompraId, Guid itemPedidoDeCompraId, Guid produtoId, int quantidadeRecebida, bool houveDivergencia, string? observacao)
        : base(pedidoCompraId)
    {
        ItemPedidoDeCompraId = itemPedidoDeCompraId;
        ProdutoId = produtoId;
        QuantidadeRecebida = quantidadeRecebida;
        HouveDivergencia = houveDivergencia;
        Observacao = observacao;
    }
}

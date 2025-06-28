// PedidoDeCompraCriadoEvent.cs
using GeneralLabSolutions.Domain.Mensageria;

public class PedidoDeCompraCriadoEvent : DomainEvent
{
    public string NumeroPedidoCompra { get; }
    public Guid FornecedorId { get; }
    public Guid ResponsavelCompraId { get; }
    public DateTime DataEmissao { get; }
    public PedidoDeCompraCriadoEvent(Guid pedidoCompraId, string numeroPedidoCompra, Guid fornecedorId, Guid responsavelCompraId, DateTime dataEmissao) : base(pedidoCompraId)
    {
        NumeroPedidoCompra = numeroPedidoCompra;
        FornecedorId = fornecedorId;
        ResponsavelCompraId = responsavelCompraId;
        DataEmissao = dataEmissao;
    }
}

// PedidoDeCompraCriadoEvent.cs
using GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos;
using GeneralLabSolutions.Domain.Mensageria;
// PedidoDeCompraStatusAlteradoEvent.cs
public class PedidoDeCompraStatusAlteradoEvent : DomainEvent
{
    public StatusPedidoCompra NovoStatus { get; }
    public Guid? UsuarioId { get; } // Quem alterou o status, se aplicável
    public string? Motivo { get; } // Se for cancelamento, por exemplo

    public PedidoDeCompraStatusAlteradoEvent(Guid pedidoCompraId, StatusPedidoCompra novoStatus, Guid? usuarioId = null, string? motivo = null) : base(pedidoCompraId)
    {
        NovoStatus = novoStatus;
        UsuarioId = usuarioId;
        Motivo = motivo;
    }
}

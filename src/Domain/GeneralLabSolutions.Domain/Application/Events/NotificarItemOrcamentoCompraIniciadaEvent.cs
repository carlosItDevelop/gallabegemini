// PedidoDeCompraCriadoEvent.cs
using GeneralLabSolutions.Domain.Mensageria;
// Eventos para comunicação entre Agregados (Orcamento <-> PedidoDeCompra)
// Disparados por PedidoDeCompra, mas consumidos por um handler que atualiza Orcamento/ItemOrcamento
public class NotificarItemOrcamentoCompraIniciadaEvent : Event // Não herda de DomainEvent, pois não é sobre o PedidoDeCompra em si
{
    public Guid ItemOrcamentoId { get; }
    public Guid PedidoDeCompraId { get; }
    public Guid ItemPedidoDeCompraId { get; }
    public NotificarItemOrcamentoCompraIniciadaEvent(Guid itemOrcamentoId, Guid pedidoDeCompraId, Guid itemPedidoDeCompraId)
    {
        // AggregateId aqui poderia ser o ItemOrcamentoId, pois é o agregado que será afetado
        base.AggregateId = itemOrcamentoId;
        ItemOrcamentoId = itemOrcamentoId;
        PedidoDeCompraId = pedidoDeCompraId;
        ItemPedidoDeCompraId = itemPedidoDeCompraId;
    }
}

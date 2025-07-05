using GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos;

namespace VelzonModerna.ViewModels
{
    public class PedidoViewModel
    {
        public Guid ClienteId { get; set; }
        public Guid VendedorId { get; set; }
        public DateTime DataPedido { get; set; } = DateTime.Now;
        public StatusDoPedido StatusDoPedido { get; set; }
            = StatusDoPedido.Orcamento;

        public virtual ClienteViewModel? Cliente { get; set; }
        public virtual VendedorViewModel? Vendedor { get; set; }

        // Relacionamento
        public virtual ICollection<ItemPedidoViewModel> Itens { get; set; }
            = new List<ItemPedidoViewModel>();
    }
}

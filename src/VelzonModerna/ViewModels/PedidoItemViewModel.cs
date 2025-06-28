namespace VelzonModerna.ViewModels
{
    public class PedidoItemViewModel
    {
        public PedidoViewModel? Pedido { get; set; }
        public virtual ICollection<ItemPedidoViewModel>? Items { get; set; }
        public virtual ICollection<ProdutoViewModel>? Produtos { get; set; }
    }
}

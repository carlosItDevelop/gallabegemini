using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities
{
    public class ItemPedido : EntityBase
    {
        public ItemPedido(Guid pedidoId, Guid produtoId, int quantidade, decimal valorUnitario, string nomeDoProduto)
            : this()
        {
            PedidoId = pedidoId;
            ProdutoId = produtoId;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            NomeDoProduto = nomeDoProduto;
        }

        // EF
        protected ItemPedido()
        {
            Estados = new List<EstadoDoItem>();
            Historico = new List<HistoricoItem>();
        }

        public Guid PedidoId { get; set; }
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public string NomeDoProduto { get; private set; }

        // Relacionamentos
        public virtual Produto Produto { get; set; }
        public virtual Pedido Pedido { get; set; }
        public virtual ICollection<EstadoDoItem> Estados { get; private set; }
        public virtual ICollection<HistoricoItem> Historico { get; private set; }

        // Métodos
        public void AtualizarQuantidade(int quantidade)
        {
            Quantidade = quantidade;
        }
    }
}
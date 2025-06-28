using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.SharedKernel.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Pedido : EntityBase
    {
        // EF
        public Pedido()
        {
            Itens = new List<ItemPedido>();
            Historico = new List<HistoricoPedido>();
        }

        public Pedido(Guid clienteId, Guid vendedorId, DateTime dataPedido)
        {
            ClienteId = clienteId;
            VendedorId = vendedorId;
            DataPedido = dataPedido;
            Itens = new List<ItemPedido>();
            Historico = new List<HistoricoPedido>();
            StatusDoPedido = StatusDoPedido.Orcamento; // Definir status inicial
        }

        public Guid ClienteId { get; set; }
        public Guid VendedorId { get; set; }
        public DateTime DataPedido { get; set; }
        public StatusDoPedido StatusDoPedido { get; private set; }

        public virtual Cliente Cliente { get; set; }
        public virtual Vendedor Vendedor { get; set; }

        public Guid? VoucherId { get; set; }

        public virtual Voucher? Voucher { get; set; }

        // Relacionamento
        public virtual ICollection<ItemPedido> Itens { get; set; }

        // Relacionamento 1:N com HistoricoPedido
        public virtual ICollection<HistoricoPedido> Historico { get; set; }

        // Métodos para adicionar itens ao pedido
        public void AdicionarItem(ItemPedido item)
        {
            Itens.Add(item);
            CalcularValorTotal();
        }

        public void RemoverItem(ItemPedido item)
        {
            if (Itens.Contains(item))
                Itens.Remove(item);
            CalcularValorTotal();
        }

        public decimal CalcularValorTotal()
        {
            decimal total = 0;
            foreach (var item in Itens)
            {
                total += item.ValorUnitario * item.Quantidade;
            }
            return total;
        }

        // Método para atualizar o status do pedido
        public void AtualizarStatus(StatusDoPedido novoStatus)
        {
            StatusDoPedido = novoStatus;

            // Aqui você também pode adicionar a lógica para registrar o histórico de alterações na tabela HistoricoPedido, se necessário.
            // Exemplo:
            // Historico.Add(new HistoricoPedido { PedidoId = this.Id, TipoEvento = "Status Alterado", StatusAnterior = this.StatusDoPedido.ToString(), StatusNovo = novoStatus.ToString(), DataHora = DateTime.Now, UsuarioId = "..." });
        }
    }
}
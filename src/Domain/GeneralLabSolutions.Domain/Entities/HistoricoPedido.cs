using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities
{
    public class HistoricoPedido : EntityBase
    {
        public Guid PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }
        public DateTime DataHora { get; set; }
        public string TipoEvento { get; set; }
        public string? StatusAnterior { get; set; }
        public string StatusNovo { get; set; }
        public string? UsuarioId { get; set; }
        public string? DadosExtras { get; set; }

        // Construtor
        public HistoricoPedido()
        {
            DataHora = DateTime.UtcNow;
        }
    }
}
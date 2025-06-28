using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities
{
    public class HistoricoItem : EntityBase
    {
        public Guid ItemPedidoId { get; set; }
        public virtual ItemPedido ItemPedido { get; set; }
        public DateTime DataHora { get; set; }
        public string TipoEvento { get; set; }
        public string? StatusAnterior { get; set; }
        public string StatusNovo { get; set; }
        public string? UsuarioId { get; set; }
        public string? DadosExtras { get; set; }

        // Construtor
        public HistoricoItem()
        {
            DataHora = DateTime.UtcNow;
        }
    }
}
using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities
{
    public class EstadoDoItem : EntityBase
    {
        public Guid ItemPedidoId { get; private set; }
        public virtual ItemPedido ItemPedido { get; private set; }

        public Guid StatusDoItemId { get; private set; }
        public virtual StatusDoItem StatusDoItem { get; private set; }

        public DateTime DataAlteracao { get; private set; }
        public bool Ativo { get; private set; }
        public string? DadosExtras { get; private set; } // JSON para informações adicionais

        // Construtor
        public EstadoDoItem(Guid itemPedidoId, Guid statusDoItemId, string dadosExtras = null)
        {
            ItemPedidoId = itemPedidoId;
            StatusDoItemId = statusDoItemId;
            DataAlteracao = DateTime.UtcNow;
            Ativo = true;
            DadosExtras = dadosExtras;
        }

        public void DesativarEstado()
        {
            Ativo = false;
        }

        // EF
        protected EstadoDoItem() { }
    }
}
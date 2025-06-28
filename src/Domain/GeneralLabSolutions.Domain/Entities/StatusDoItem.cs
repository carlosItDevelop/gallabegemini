using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities
{
    public class StatusDoItem : EntityBase
    {
        public StatusDoItem()
        {
            Incompatibilidades = new List<StatusDoItemIncompativel>();
        }

        public string Descricao { get; set; }
        public bool Ativo { get; set; }

        // Relacionamento com StatusDoItemIncompativel (1:N)
        public virtual ICollection<StatusDoItemIncompativel> Incompatibilidades { get; set; }
    }
}
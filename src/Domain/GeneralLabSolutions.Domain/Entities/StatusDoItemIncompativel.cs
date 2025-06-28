using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities
{
    public class StatusDoItemIncompativel : EntityBase
    {
        public Guid StatusDoItemId { get; set; }
        public virtual StatusDoItem StatusDoItem { get; set; }

        public Guid StatusDoItemIncompativelId { get; set; }
        public virtual StatusDoItem StatusDoItemIncompativelNavigation { get; set; }
    }
}
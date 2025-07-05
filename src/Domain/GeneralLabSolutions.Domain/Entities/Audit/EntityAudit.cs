using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities.Audit
{

    public interface IAuditable : IAuditableAdd, IAuditableUpd
    {
    }

    public interface IAuditableAdd
    {
        DateTime? DataInclusao { get; set; }
        string UsuarioInclusao { get; set; }
    }

    public interface IAuditableUpd
    {
        DateTime? DataUltimaModificacao { get; set; }
        string UsuarioUltimaModificacao { get; set; }
    }


    public abstract class EntityAudit : EntityBase, IAuditable
    {
        public EntityAudit() { }

        public DateTime? DataInclusao { get; set; }
        public string UsuarioInclusao { get; set; }
        public DateTime? DataUltimaModificacao { get; set; }
        public string UsuarioUltimaModificacao { get; set; }
    }
}

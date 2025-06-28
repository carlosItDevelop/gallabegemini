using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities
{
    public class CategoriaProduto : EntityBase
    {

        /// <summary>
        /// Construtor próprio para 
        /// o EntityFramework Core.
        /// </summary>
        public CategoriaProduto() { }

        public CategoriaProduto(string? descricao)
        {
            Descricao = descricao;
        }

        public string? Descricao { get; private set; }

        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();

    }
}

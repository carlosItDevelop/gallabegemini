using System.ComponentModel.DataAnnotations;

namespace VelzonModerna.ViewModels
{
    public class CategoriaProdutoViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(200, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 2)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        // Ainda NÃO Removemos a propriedade Produtos 
        public virtual ICollection<ProdutoViewModel>? Produtos { get; set; } = new List<ProdutoViewModel>();
    }
}
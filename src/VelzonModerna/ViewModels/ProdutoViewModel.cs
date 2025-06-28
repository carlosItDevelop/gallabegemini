using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GeneralLabSolutions.Domain.Enums;

namespace VelzonModerna.ViewModels
{
    public class ProdutoViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(20, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 2)]
        [Display(Name = "Código do Produto")]
        public string? Codigo { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(600, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 2)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [StringLength(15, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 2)] // Adicionado validação
        public string? Ncm { get; set; }

        [DataType(DataType.Currency, ErrorMessage = "O campo {0} está inválido.")]
        [DisplayName("Valor Unitário")]
        public decimal ValorUnitario { get; set; }

        [Display(Name = "Status do Produto")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public StatusDoProduto StatusDoProduto { get; set; } = StatusDoProduto.Dropshipping;

        [DataType(DataType.Date, ErrorMessage = "O campo {0} está inválido.")] // Alterado para DataType.Date
        public DateTime DataDeValidade { get; set; }

        [DisplayName("Imagem do Produto")]
        public IFormFile? ImagemUpload { get; set; }

        [Display(Name = "Id da Categoria")]
        public Guid CategoriaId { get; set; }

        [Display(Name = "Id do Fornecedor")]
        public Guid FornecedorId { get; set; }

        // Removemos as propriedades Imagem, CategoriaProduto e Fornecedor 
    }
}
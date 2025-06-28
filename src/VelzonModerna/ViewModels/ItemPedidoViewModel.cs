using System.ComponentModel.DataAnnotations;

namespace VelzonModerna.ViewModels
{
    public class ItemPedidoViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Pedido")]
        public Guid PedidoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Produto")]
        public Guid ProdutoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DataType(DataType.Currency, ErrorMessage = "O campo {0} está inválido.")]
        [Display(Name = "Valor Unitário")]
        public decimal ValorUnitario { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(600, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 2)]
        [Display(Name = "Nome do Produto")]
        public string NomeDoProduto { get; set; }

        // Removemos as propriedades Produto e Pedido
    }
}
// Poderíamos adicionar esta propriedade a ItemPedidoDeCompraInputViewModel
// ou criar uma ViewModel separada para edição.
// Se adicionarmos a ItemPedidoDeCompraInputViewModel:
using System.ComponentModel.DataAnnotations;

public class ItemPedidoDeCompraInputViewModel
{
    public Guid? Id { get; set; } // Usado para identificar o item existente ao editar

    [Required(ErrorMessage = "O Produto é obrigatório.")]
    [Display(Name = "Produto")]
    public Guid ProdutoId { get; set; } // Para edição, este campo pode ser readonly ou não editável.

    // ... outras propriedades como antes ...
    [Display(Name = "Nome do Produto (Somente Leitura)")]
    public string NomeProdutoDisplay { get; set; } // Para exibir o nome do produto no form de edição
}
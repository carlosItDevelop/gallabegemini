using System.ComponentModel.DataAnnotations;
using GeneralLabSolutions.Domain.Enums;

namespace VelzonModerna.ViewModels
{
    public class FornecedorViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(200, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 2)]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(14, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 11)]
        public string? Documento { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Status do Fornecedor")]
        public StatusDoFornecedor StatusDoFornecedor { get; set; } = StatusDoFornecedor.Ativo;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Pessoa Física / Jurídica")]
        public TipoDePessoa TipoDePessoa { get; set; }

        // Email e Endereço são propriedades comuns, mas não estão implementadas aqui.
        public string? Email { get; set; }

        // Já refatorado e pronto
        public List<DadosBancariosViewModel> DadosBancarios { get; set; } = new List<DadosBancariosViewModel>();

        // Coleção para Telefones (confirmando o tipo)
        // Usar List<> é geralmente mais prático que ICollection<> em ViewModels
        public List<TelefoneViewModel> Telefones { get; set; } = new List<TelefoneViewModel>();

        public List<ContatoViewModel> Contatos { get; set; } = new List<ContatoViewModel>();

        public List<EnderecoViewModel> Enderecos { get; set; } = new List<EnderecoViewModel>();

        // Removemos a propriedade Produtos
    }
}
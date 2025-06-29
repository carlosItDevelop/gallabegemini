using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;

namespace VelzonModerna.ViewModels
{
    public class ClienteViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(200, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(254, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(14, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 11)]
        public string Documento { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Status do Cliente")]
        public StatusDoCliente StatusDoCliente { get; set; } = StatusDoCliente.Ativo;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Tipo de Cliente")]
        public TipoDeCliente TipoDeCliente { get; set; } = TipoDeCliente.Comum;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Tipo de Pessoa")]
        public TipoDePessoa TipoDePessoa { get; set; }


        public Guid PessoaId { get; set; }


        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Telefone Principal")]
        public string TelefonePrincipal { get; set; }

        [Display(Name = "Contato principal ou Representante")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string? ContatoRepresentante { get; set; } // Mantido como nullable string


        [Display(Name = "Inscrição Estadual")]
        [StringLength(80, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string? InscricaoEstadual { get; set; } = string.Empty;

        [Display(Name = "Observação")]
        [StringLength(4000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string? Observacao { get; set; }



        // Já refatorado e pronto
        public List<DadosBancariosViewModel> DadosBancarios { get; set; } = new List<DadosBancariosViewModel>();

        // Coleção para Telefones (confirmando o tipo)
        // Usar List<> é geralmente mais prático que ICollection<> em ViewModels
        public List<TelefoneViewModel> Telefones { get; set; } = new List<TelefoneViewModel>();

        public List<ContatoViewModel> Contatos { get; set; } = new List<ContatoViewModel>();

        public List<EnderecoViewModel> Enderecos { get; set; } = new List<EnderecoViewModel>();


        #region: Campos auditáveis

        // ToDo: Talvez seja desnecessário, pois estes dados de Audit geralmente são gerenciados pelo EF Core

        [DisplayName(displayName: "Data de Cadastro")]
        public DateTime DataCadastro { get; set; }

        [DisplayName(displayName: "Data de Atualização")]
        public DateTime? DataAtualizacao { get; set; }

        [DisplayName(displayName: "Usuário de Cadastro")]
        public string? UsuarioCadastro { get; set; }

        [DisplayName(displayName: "Usuário de Atualização")]
        public string? UsuarioUltimaModificacao { get; set; }

        [DisplayName(displayName: "Usuário de Atualização")]
        public string? UsuarioAtualizacao { get; set; }

        //[DisplayName("Ativo")]
        //public bool Ativo { get; set; }

        //[DisplayName("Excluído")]
        //public bool Excluido { get; set; }

        #endregion: Campos auditáveis


    }
}
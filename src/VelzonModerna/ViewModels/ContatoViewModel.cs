using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using GeneralLabSolutions.Domain.Enums;

namespace VelzonModerna.ViewModels
{
    public class ContatoViewModel
    {
        [Key]
        [Display(Name = "ID do Contato")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "ID da Pessoa")] // Cliente, Fornecedor, Vendedor, etc.
        public Guid PessoaId { get; set; }

        // Não é necessária a propriedade Pessoa


        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Nome do Contato")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Email Principal")]
        [DataType(DataType.EmailAddress, ErrorMessage = "O campo {0} está inválido!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Telefone Principal")]
        public string Telefone { get; set; }

        [Display(Name = "Email Alternativo")]
        public string? EmailAlternativo { get; set; } = string.Empty;

        [Display(Name = "Telefone Alternativo")]
        public string? TelefoneAlternativo { get; set; } = string.Empty;

        [Display(Name = "Observação")]
        public string? Observacao { get; set; } = string.Empty;

        [Display(Name = "Tipo de Contato")]
        public TipoDeContato TipoDeContato { get; set; } = TipoDeContato.Comercial;

        [DisplayName(displayName: "Data Inclusão")]
        public DateTime? DataInclusao { get; set; }

        [DisplayName(displayName: "Data Última Modificação")]
        public DateTime? DataUltimaModificacao { get; set; }

        [DisplayName(displayName: "Usuário Inclusão")]
        public string? UsuarioInclusao { get; set; }

        [DisplayName(displayName: "Usuário Última Modificação")]
        public string? UsuarioUltimaModificacao { get; set; }
    }
}
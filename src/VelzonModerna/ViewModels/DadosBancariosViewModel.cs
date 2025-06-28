using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using GeneralLabSolutions.Domain.Enums;

namespace VelzonModerna.ViewModels
{
    public class DadosBancariosViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public Guid PessoaId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O máximo de caracteres para o campo {0} é de {1} caracteres.")]
        public string Banco { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(20, ErrorMessage = "O máximo de caracteres para o campo {0} é de {1} caracteres.")]
        public string Agencia { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(20, ErrorMessage = "O máximo de caracteres para o campo {0} é de {1} caracteres.")]
        public string Conta { get; set; }

        public TipoDeContaBancaria TipoDeContaBancaria { get; set; } // Ex: Corrente, Poupança

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
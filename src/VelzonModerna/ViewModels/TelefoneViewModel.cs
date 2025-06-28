using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using GeneralLabSolutions.Domain.Enums;

namespace VelzonModerna.ViewModels
{
    public class TelefoneViewModel
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PessoaId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(3, ErrorMessage = "O máximo de caracteres para o campo {0} é de {1} caracteres.")] // Ajustado para 3 caracteres
        public string DDD { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(15, ErrorMessage = "O máximo de caracteres para o campo {0} é de {1} caracteres.")] // Ajustado para 15 caracteres
        [Display(Name = "Número")]
        public string Numero { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Tipo de Telefone")]
        public TipoDeTelefone TipoDeTelefone { get; set; }

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
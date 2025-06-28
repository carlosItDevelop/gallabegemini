using System.ComponentModel.DataAnnotations;
using GeneralLabSolutions.Domain.Entities;

namespace VelzonModerna.ViewModels
{
    public class DadosBancariosViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public Guid PessoaId { get; set; }

        // Pessoa não é necessário no cenário de Edit e Create
        // que estamos usando na chamada AJAX.
        //public Pessoa Pessoa { get; set; }

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

    }
}

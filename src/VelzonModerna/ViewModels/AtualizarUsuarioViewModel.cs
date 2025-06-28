using System.ComponentModel.DataAnnotations;

namespace VelzonModerna.ViewModels
{
    public class AtualizarUsuarioViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required(ErrorMessage = "O campo Nome Completo é obrigatório")]
        [Display(Name = "Nome Completo")]
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório")]
        [EmailAddress(ErrorMessage = "O Email não é válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo Data de Nascimento é obrigatório")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [Display(Name = "Ativo?")]
        public bool IsAtivo { get; set; }

        public string ImagemAtualUrl { get; set; } // Para mostrar a imagem atual na view de edição
    }
}

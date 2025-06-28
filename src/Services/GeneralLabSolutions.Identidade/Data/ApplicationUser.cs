using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GeneralLabSolutions.Identidade.Data
{
    /// <summary>
    /// Especialização do IdentityUser a fim de extender suas funcionalidades, e/ou, propriedades.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(maximumLength: 35, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres!", MinimumLength = 2)]
        public string Apelido { get; set; } = string.Empty;

        [Display(Name = "Nome Completo")]
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(maximumLength: 80, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres!", MinimumLength = 2)]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [DataType(DataType.Text)]
        [StringLength(maximumLength: 4000, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 21)]
        public string ImgProfilePath { get; set; } = "imagemPadrao.png";

        public bool IsAtivo { get; set; } = true;

        /// <summary>
        /// Ativar Usuário
        /// </summary>
        public void AtivarUsuario() 
            => IsAtivo = true;        

        /// <summary>
        /// Ativar Usuário
        /// </summary>
        public void InativarUsuario() =>
            IsAtivo = false;


    }
}

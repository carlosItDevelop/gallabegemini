using System.ComponentModel.DataAnnotations;

namespace GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade
{
    /// <summary>
    /// DTO para transporte de dados para Atualizar Senha
    /// </summary>
    public class AtualizarSenhaDto
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string UserId { get; set; } = string.Empty; // ID do usuário

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.Password)]
        public string SenhaAtual { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(30, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,30}$",
                           ErrorMessage = "A senha deve ter pelo menos um caractere maiúsculo, um minúsculo, um número e um símbolo.")]
        [DataType(DataType.Password)]
        public string NovaSenha { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("NovaSenha", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmarNovaSenha { get; set; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations;

namespace GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade
{
    /// <summary>
    /// Transporte de dados para inclusão de usuário a uma role
    /// </summary>
    public class AdicionarUsuarioRoleDto
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string UserId { get; set; } = string.Empty; // ID do usuário

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string RoleName { get; set; } = string.Empty; // Nome da role 
    }
}
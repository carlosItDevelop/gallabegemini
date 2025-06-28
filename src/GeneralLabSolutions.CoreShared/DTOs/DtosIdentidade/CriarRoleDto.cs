using System.ComponentModel.DataAnnotations;

namespace GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade
{
    /// <summary>
    /// DTO para transporte de dados para criar role
    /// </summary>
    public class CriarRoleDto
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Nome { get; set; } = string.Empty; // Nome da nova role
    }
}
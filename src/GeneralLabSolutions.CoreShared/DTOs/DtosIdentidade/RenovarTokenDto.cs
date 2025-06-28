using System.ComponentModel.DataAnnotations;

namespace GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade
{
    /// <summary>
    /// DTO para transporte de dados para renovar token
    /// </summary>
    public class RenovarTokenDto
    {
        [Required(ErrorMessage = "O RefreshToken é obrigatório")]
        public string RefreshToken { get; set; }
    }
}

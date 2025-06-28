namespace GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade
{
    /// <summary>
    /// DTO para transporte de dados do Usuário
    /// </summary>
    public class UserDto
    {

        public string? UserId { get; set; } = Guid.NewGuid().ToString();
        public string Apelido { get; set; } = string.Empty;
        public string NomeCompleto { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }

        public string ImgProfilePath { get; set; } = string.Empty;


        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmailConfirmado { get; set; } = string.Empty;
        public string UsuarioBloqueado { get; set; } = string.Empty;
        public int NumeroDeErroDeLogin { get; set; }
    }
}

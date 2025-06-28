namespace GeneralLabSolutions.CoreShared.ViewModelsIdentidade
{
    public class UserResponseDto
    {
        public string UserId { get; set; } = string.Empty;

        public string Apelido { get; set; } = string.Empty;
        public string NomeCompleto { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }

        public string ImgProfilePath { get; set; } = "imagemPadrao.png";


        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmado { get; set; }

    }
}

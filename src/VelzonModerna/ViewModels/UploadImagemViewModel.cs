using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace VelzonModerna.ViewModels
{
    public class UploadImagemViewModel
    {
        [Required]
        public string UserId { get; set; }

        public string NomeUsuario { get; set; } // Para exibição no formulário

        [Display(Name = "Nova Imagem de Perfil")]
        [Required(ErrorMessage = "Por favor, selecione uma imagem.")]
        // Você pode adicionar validações de tipo de arquivo e tamanho aqui usando atributos customizados ou no controller
        public IFormFile Imagem { get; set; }

        public string ImagemAtualUrl { get; set; } // Para mostrar a imagem atual
    }
}
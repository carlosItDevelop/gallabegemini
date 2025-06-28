using System;
using System.ComponentModel.DataAnnotations;

namespace VelzonModerna.ViewModels.CRM
{
    public class CrmTaskCommentViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O conteúdo do comentário é obrigatório.")]
        public string Content { get; set; }

        public string? UserId { get; set; } // ID do usuário que comentou

        [Required]
        public Guid CrmTaskId { get; set; }

        // Propriedade para exibição, pode ser preenchida no backend se necessário
        public DateTime CreatedAt { get; set; }
        public string? UserName { get; set; } // Nome do usuário para exibição
    }
}
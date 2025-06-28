using System;
using System.ComponentModel.DataAnnotations;

namespace VelzonModerna.ViewModels.CRM
{
    public class LeadNoteViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public Guid LeadId { get; set; }

        [Required(ErrorMessage = "O conteúdo da nota é obrigatório.")]
        public string Content { get; set; }

        [StringLength(20)]
        public string Color { get; set; } = "blue";

        public string? UserId { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace VelzonModerna.ViewModels.CRM
{
    public class ActivityViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O tipo da atividade é obrigatório.")]
        [StringLength(50)]
        public string Type { get; set; }

        [Required(ErrorMessage = "O título da atividade é obrigatório.")]
        [StringLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "A data agendada é obrigatória.")]
        public DateTime ScheduledDate { get; set; }

        // Opcional, para atrelar a um Lead
        public Guid? LeadId { get; set; }
    }
}
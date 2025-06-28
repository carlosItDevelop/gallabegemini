using System.ComponentModel.DataAnnotations;

namespace VelzonModerna.Models
{
    public class CalendarEventInputModel
    {
        public Guid? Id { get; set; } // Adicionei '?' para tornar anulável

        [Required(ErrorMessage = "Por favor, informe o nome do evento.")]
        [StringLength(150, ErrorMessage = "O nome do evento não pode exceder 150 caracteres.")]
        [Display(Name = "Nome do Evento")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor, selecione um tipo (categoria) para o evento.")]
        [Display(Name = "Tipo")]
        public string Category { get; set; } = string.Empty; // "Success", "Info", etc.

        // Flatpickr envia datas como string, vamos validar e parsear no Controller
        [Required(ErrorMessage = "Por favor, selecione a(s) data(s) do evento.")]
        [Display(Name = "Data do Evento")]
        public string EventDate { get; set; } = string.Empty; // Pode ser "YYYY-MM-DD" ou "YYYY-MM-DD to YYYY-MM-DD"

        [Display(Name = "Hora de Início")]
        public string? StartTime { get; set; } // "HH:mm" - Obrigatório se não for AllDay com data única

        [Display(Name = "Hora de Fim")]
        public string? EndTime { get; set; } // "HH:mm"

        [StringLength(250, ErrorMessage = "A localização não pode exceder 250 caracteres.")]
        [Display(Name = "Localização")]
        public string? Location { get; set; }

        [StringLength(1000, ErrorMessage = "A descrição não pode exceder 1000 caracteres.")]
        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        // Propriedades calculadas (não são preenchidas pelo form, mas podem ser úteis)
        // public DateTime ParsedStart { get; set; }
        // public DateTime? ParsedEnd { get; set; }
        // public bool IsAllDay { get; set; }
    }
}

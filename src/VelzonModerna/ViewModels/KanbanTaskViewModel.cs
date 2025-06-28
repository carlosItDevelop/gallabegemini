using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using GeneralLabSolutions.Domain.Enums;


namespace VelzonModerna.ViewModels
{
    public class KanbanTaskViewModel
    {

        [JsonPropertyName("id")]
        [Display(Name = "ID")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public Guid Id { get; set; }

        [JsonPropertyName("title")]
        [Display(Name = "Título")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(80, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 2)]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(150, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 2)]
        public string Description { get; set; }

        // Uma forma simples de guardar a 'column' atual: ToDo, Review, etc.
        [JsonPropertyName("column")]
        public Column Column { get; set; }

        [JsonPropertyName("priority")]
        public Priority Priority { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }

        // Relação Many-to-Many com Participante
        //public ICollection<ParticipanteViewModel> Participantes { get; set; }

        [JsonPropertyName("participantIds")]
        public ICollection<Guid> ParticipantIds { get; set; }

    }
}

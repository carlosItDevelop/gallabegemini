using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using GeneralLabSolutions.Domain.Entities;

namespace VelzonModerna.ViewModels
{
    public class ParticipanteViewModel
    {
        [Key]
        [Display(Name = "ID")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 2)]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(254, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 2)]
        public string? Email { get; set; }

        // Relação reversa Many-to-Many (opcional)
        [JsonPropertyName("tasks")]
        public ICollection<KanbanTaskViewModel>? Tasks { get; set; }
    }
}
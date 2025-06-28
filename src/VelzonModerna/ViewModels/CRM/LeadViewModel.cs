using GeneralLabSolutions.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VelzonModerna.ViewModels.CRM
{
    public class LeadViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Company { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo {0} está em formato inválido.")]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(255)]
        public string? Position { get; set; }

        [StringLength(100)]
        public string? Source { get; set; }

        [StringLength(255)]
        public string? Responsible { get; set; }

        public int Score { get; set; }
        public decimal Value { get; set; }
        public string? Notes { get; set; }
        public DateTime? LastContact { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LeadStatus Status { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LeadTemperature Temperature { get; set; }
    }
}
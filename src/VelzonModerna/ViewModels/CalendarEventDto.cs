
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VelzonModerna.ViewModels
{
    public class CalendarEventDto
    {
        [JsonPropertyName("id")] // Garante a serialização correta para o JS
        public Guid Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty; // Formato ISO 8601: yyyy-MM-ddTHH:mm:ss ou yyyy-MM-dd

        [JsonPropertyName("end")]
        public string? End { get; set; } // Formato ISO 8601

        [JsonPropertyName("allDay")]
        public bool AllDay { get; set; }

        [JsonPropertyName("className")]
        public string ClassName { get; set; } = string.Empty; // e.g., "bg-success-subtle"

        [JsonPropertyName("url")]
        public string? Url { get; set; } // Se houver links

        [JsonPropertyName("extendedProps")]
        public Dictionary<string, object?>? ExtendedProps { get; set; } // Para Location, Description, etc.

        // --- Método auxiliar para mapear Category para className ---
        public static string MapCategoryToClassName(string? category)
        {
            // Usa as classes CSS da template Velzon
            return category?.ToLowerInvariant() switch
            {
                "success" => "bg-success-subtle text-success",
                "danger" => "bg-danger-subtle text-danger",
                "warning" => "bg-warning-subtle text-warning",
                "info" => "bg-info-subtle text-info",
                "primary" => "bg-primary-subtle text-primary",
                "dark" => "bg-dark-subtle text-dark",
                // Adicione mais categorias se necessário
                _ => "bg-primary-subtle text-primary" // Default
            };
        }
    }

    // DTO Específico para atualização de tempo via Drag/Drop/Resize
    public class EventTimeUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Start { get; set; } = string.Empty; // ISO 8601 string

        public string? End { get; set; } // ISO 8601 string

        public bool AllDay { get; set; }
    }
}

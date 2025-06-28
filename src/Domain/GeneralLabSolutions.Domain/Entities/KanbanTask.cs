using System.Text.Json.Serialization;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Extensions;

namespace GeneralLabSolutions.Domain.Entities
{
    public partial class KanbanTask : EntityBase
    {

        // EF Core
        public KanbanTask(){}

        public KanbanTask(string title, string description, DateTime? dueDate)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Participantes = new List<Participante>();
        }


        [JsonPropertyName("title")]
        public string Title { get; private set; }

        [JsonPropertyName("description")]
        public string Description { get; private set; }

        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; private set; }

        // Uma forma simples de guardar a 'column' atual: ToDo, Review, etc.
        [JsonPropertyName("column")]
        public Column Column { get; set; }

        [JsonPropertyName("priority")]
        public Priority Priority { get; set; }


        // Relação Many-to-Many com Participante
        [JsonPropertyName("participantes")]
        public ICollection<Participante> Participantes { get; set; }


    }
}
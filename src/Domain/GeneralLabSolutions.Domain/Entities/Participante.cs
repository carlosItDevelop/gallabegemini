using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Participante : EntityBase
    {
        // EF
        public Participante() { }

        public Participante(string? name, string? email)
        {
            Name = name;
            Email = email;
            Tasks = new List<KanbanTask>();
        }

        [JsonPropertyName("name")]
        public string? Name { get; private set; }    // Nome ou identificação

        [JsonPropertyName("email")]
        public string? Email { get; private set; }   // Nome ou identificação

        // Relação reversa Many-to-Many (opcional)
        [JsonPropertyName("tasks")]
        public ICollection<KanbanTask>? Tasks { get; set; }

    }

}

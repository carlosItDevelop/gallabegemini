using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeneralLabSolutions.Domain.Entities.Base; // Supondo que você tenha uma EntityBase
using System.ComponentModel.DataAnnotations;

namespace GeneralLabSolutions.Domain.Entities
{
    public class CalendarEvent : EntityBase // Herdar de EntityBase se aplicável
    {
        [Required(ErrorMessage = "O título do evento é obrigatório.")]
        [StringLength(150, ErrorMessage = "O título não pode exceder 150 caracteres.")]
        public string Title { get; private set; }

        [Required(ErrorMessage = "A data/hora de início é obrigatória.")]
        public DateTime Start { get; private set; }

        public DateTime? End { get; private set; } // Nullable para eventos de dia inteiro ou sem fim definido

        public bool AllDay { get; private set; }

        [Required(ErrorMessage = "A categoria do evento é obrigatória.")]
        [StringLength(50, ErrorMessage = "A categoria não pode exceder 50 caracteres.")]
        public string Category { get; private set; } // Armazena "Success", "Info", "Danger", etc.

        [StringLength(250, ErrorMessage = "A localização não pode exceder 250 caracteres.")]
        public string? Location { get; private set; }

        [StringLength(1000, ErrorMessage = "A descrição não pode exceder 1000 caracteres.")]
        public string? Description { get; private set; }

        // --- Construtor para EF Core ---
        protected CalendarEvent() { }

        // --- Construtor para criação/atualização ---
        public CalendarEvent(string title, DateTime start, DateTime? end, bool allDay, string category, string? location, string? description)
        {
            UpdateDetails(title, start, end, allDay, category, location, description);
        }

        public void UpdateDetails(string title, DateTime start, DateTime? end, bool allDay, string category, string? location, string? description)
        {
            // Adicionar validações se necessário (FluentValidation ou aqui)
            Title = title;
            Start = start;
            End = end;
            AllDay = allDay;
            Category = category;
            Location = location;
            Description = description;

            // Validar se End >= Start se End não for nulo
            if (End.HasValue && Start > End.Value)
            {
                // Lançar exceção ou adicionar a uma lista de erros de domínio
                throw new ArgumentException("A data/hora final não pode ser anterior à data/hora inicial.");
            }
            if (AllDay && End.HasValue && End.Value.Date == Start.Date)
            {
                // Para eventos AllDay de um único dia, FullCalendar geralmente espera End nulo
                // Ou End sendo o início do dia seguinte. Ajustar conforme necessário ou remover End.
                // End = null; // Opção 1
                End = Start.Date.AddDays(1); // Opção 2 (comum para FullCalendar)
            } else if (AllDay && End.HasValue)
            {
                // Para eventos AllDay de múltiplos dias, FullCalendar espera que End seja o início do dia *seguinte* ao último dia
                End = End.Value.Date.AddDays(1);
            }
        }

        public void UpdateTiming(DateTime start, DateTime? end, bool allDay)
        {
            Start = start;
            End = end;
            AllDay = allDay;

            // Reaplicar lógicas de validação/ajuste
            if (End.HasValue && Start > End.Value)
            {
                throw new ArgumentException("A data/hora final não pode ser anterior à data/hora inicial.");
            }
            if (AllDay && End.HasValue && End.Value.Date == Start.Date)
            {
                End = Start.Date.AddDays(1);
            } else if (AllDay && End.HasValue)
            {
                End = End.Value.Date.AddDays(1);
            }
        }

        // Adicionar UserId se for multi-tenant
        // public string? UserId { get; private set; }
    }
}

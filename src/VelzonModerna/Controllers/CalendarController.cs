
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Data; // Seu DbContext
using Microsoft.AspNetCore.Authorization; // Se precisar de autorização
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VelzonModerna.Models;
using VelzonModerna.ViewModels;



namespace VelzonModerna.Controllers
{
    // [Authorize] // Descomente se a funcionalidade requer login
    //[Route("[controller]")] 
    public class CalendarController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CalendarController> _logger;

        // Ajuste os formatos de data/hora conforme o esperado pelo Flatpickr/FullCalendar
        private const string IsoDateFormat = "yyyy-MM-dd";
        private const string IsoDateTimeFormat = "yyyy-MM-ddTHH:mm:ss"; // FullCalendar costuma usar este
        private const string TimeFormat = "HH:mm";


        public CalendarController(AppDbContext context, ILogger<CalendarController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // --- Action Principal da View ---
        [HttpGet]
        public IActionResult Index()
        {
            // Passar as opções de categoria para a View (para o dropdown)
            ViewBag.EventCategories = GetCategoryOptions();
            return View("Calendar"); // Assume que Calendar.cshtml está em Views/Calendar/
        }

        [HttpGet]
        public async Task<IActionResult> AgendaFilter()
        {
            var listAgendaEventos = await _context.Set<CalendarEvent>()
                .AsNoTracking().ToListAsync();

            return View(listAgendaEventos);
        }



        // No CalendarController.cs
        //[HttpGet("GetEventDetails/{id:guid}")] // VERIFIQUE ESTA LINHA CUIDADOSAMENTE
        public async Task<IActionResult> GetEventDetails(Guid id) // VERIFIQUE ASSINATURA
        {
            // ... (código interno da action conforme sugerido anteriormente) ...
            if (id == Guid.Empty)
                return BadRequest("ID inválido.");
            try
            {
                var calendarEvent = await _context.CalendarEvents.FindAsync(id);
                if (calendarEvent == null)
                {
                    return NotFound(new { message = "Evento não encontrado." });
                }

                // TODO: Verificar permissão de acesso se multi-tenant

                // Mapear para DTO (ou um DTO específico para detalhes se necessário)
                var eventDto = new CalendarEventDto
                {
                    Id = calendarEvent.Id,
                    Title = calendarEvent.Title,
                    Start = calendarEvent.AllDay ? calendarEvent.Start.ToString(IsoDateFormat) : calendarEvent.Start.ToString(IsoDateTimeFormat),
                    End = calendarEvent.End.HasValue ? (calendarEvent.AllDay ? calendarEvent.End.Value.ToString(IsoDateFormat) : calendarEvent.End.Value.ToString(IsoDateTimeFormat)) : null,
                    AllDay = calendarEvent.AllDay,
                    ClassName = CalendarEventDto.MapCategoryToClassName(calendarEvent.Category),
                    ExtendedProps = new Dictionary<string, object?>
             {
                 { "location", calendarEvent.Location },
                 { "description", calendarEvent.Description },
                 { "category", calendarEvent.Category }
             }
                };
                return Ok(eventDto); // Retorna apenas o DTO do evento solicitado
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar detalhes do evento {EventId}", id);
                return StatusCode(500, new { message = "Erro interno ao buscar detalhes do evento." });
            }
        }



        // --- Action para buscar eventos para FullCalendar ---
        [HttpGet]
        public async Task<IActionResult> GetEvents(string start, string end)
        {
            // Validação básica das datas de entrada
            if (!DateTime.TryParse(start, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var startDate) ||
                !DateTime.TryParse(end, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var endDate))
            {
                _logger.LogWarning("Falha ao parsear datas de início/fim: start={start}, end={end}", start, end);
                return BadRequest("Formato de data inválido.");
            }

            try
            {
                // TODO: Adicionar filtro por UserId se for multi-tenant
                var events = await _context.CalendarEvents
                    .Where(e => e.Start < endDate && (e.End == null || e.End > startDate))
                    // .Where(e => e.UserId == GetCurrentUserId()) // Exemplo multi-tenant
                    .ToListAsync();

                // Mapear para DTO
                var eventDtos = events.Select(ev => new CalendarEventDto
                {
                    Id = ev.Id,
                    Title = ev.Title,
                    // FullCalendar espera ISO 8601. Para AllDay, apenas a data.
                    Start = ev.AllDay ? ev.Start.ToString(IsoDateFormat) : ev.Start.ToString(IsoDateTimeFormat),
                    End = ev.End.HasValue ? (ev.AllDay ? ev.End.Value.ToString(IsoDateFormat) : ev.End.Value.ToString(IsoDateTimeFormat)) : null,
                    AllDay = ev.AllDay,
                    ClassName = CalendarEventDto.MapCategoryToClassName(ev.Category),
                    ExtendedProps = new Dictionary<string, object?>
                    {
                        { "location", ev.Location },
                        { "description", ev.Description },
                        { "category", ev.Category } // Pode ser útil ter a string original no JS
                    }
                    // Url = ev.SomeUrl // Se tiver URL
                }).ToList();

                return Ok(eventDtos); // Retorna JSON
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar eventos do calendário.");
                return StatusCode(500, "Erro interno ao buscar eventos.");
            }
        }

        // --- Action para Salvar (Criar ou Atualizar) Evento ---
        [HttpPost]
        [ValidateAntiForgeryToken] // Essencial para segurança!
        public async Task<IActionResult> Save([FromBody] CalendarEventInputModel model) // Recebe JSON do Ajax
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna erros de validação
            }

            // --- Parsear Datas e Horas ---
            if (!TryParseEventDates(model, out var parsedStart, out var parsedEnd, out var isAllDay))
            {
                ModelState.AddModelError("EventDate", "Formato inválido para data ou hora do evento.");
                return BadRequest(ModelState);
            }

            try
            {
                CalendarEvent? calendarEvent;

                // *** LÓGICA CORRIGIDA PARA Guid? ***
                // --- ATUALIZAR (Tem valor e não é Empty - Guid.Empty pode ser um ID válido em alguns cenários, mas geralmente não como PK gerada) ---
                // Ou simplesmente: if (model.Id.HasValue) se Guid.Empty nunca for usado como ID real.
                if (model.Id.HasValue && model.Id.Value != Guid.Empty) 
                {
                    calendarEvent = await _context.CalendarEvents.FindAsync(model.Id.Value); // <<< Usa .Value
                    if (calendarEvent == null)
                    {
                        return NotFound(new { success = false, message = "Evento não encontrado." });
                    }
                    // ... (lógica de UpdateDetails)
                    calendarEvent.UpdateDetails(
                        model.Title, parsedStart, parsedEnd, isAllDay, model.Category, model.Location, model.Description);
                    _context.CalendarEvents.Update(calendarEvent);
                    _logger.LogInformation("Evento atualizado: {EventId}", calendarEvent.Id);
                } else // --- CRIAR (Id é null ou Guid.Empty) ---
                {
                    calendarEvent = new CalendarEvent(
                        model.Title, parsedStart, parsedEnd, isAllDay, model.Category, model.Location, model.Description);
                    // O Id será gerado pelo EF Core/Banco de dados ao adicionar
                    await _context.CalendarEvents.AddAsync(calendarEvent);
                    _logger.LogInformation("Novo evento criado.");
                }

                await _context.SaveChangesAsync(); // Salva no banco de dados

                // Retorna o ID real (seja ele o existente ou o recém-criado)
                return Ok(new { success = true, eventId = calendarEvent.Id });
            } catch (ArgumentException argEx) // Captura erro de data fim < data início
            {
                _logger.LogWarning(argEx, "Erro de argumento ao salvar evento.");
                return BadRequest(new { success = false, message = argEx.Message });
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar evento no calendário.");
                return StatusCode(500, new { success = false, message = "Erro interno ao salvar o evento." });
            }
        }

        // --- Action para Deletar Evento ---
        [HttpPost] // Usar POST para delete com AntiForgeryToken é mais simples que configurar DELETE
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { success = false, message = "ID inválido." });

            try
            {
                var calendarEvent = await _context.CalendarEvents.FindAsync(id);
                if (calendarEvent == null)
                {
                    return NotFound(new { success = false, message = "Evento não encontrado." });
                }
                // TODO: Verificar permissão de exclusão se multi-tenant
                // if (calendarEvent.UserId != GetCurrentUserId()) return Forbid();

                _context.CalendarEvents.Remove(calendarEvent);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Evento deletado: {EventId}", id);
                return Ok(new { success = true });
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar evento {EventId}", id);
                return StatusCode(500, new { success = false, message = "Erro interno ao deletar o evento." });
            }
        }

        // --- Action para Atualizar Tempo (Drag/Drop/Resize) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEventTime([FromBody] EventTimeUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Parse das datas ISO 8601 recebidas (Start)
            if (!DateTime.TryParse(model.Start, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var parsedStart))
            {
                ModelState.AddModelError("Start", "Formato inválido para data/hora de início.");
                return BadRequest(ModelState);
            }

            // --- LÓGICA CORRIGIDA PARA parsedEnd ---
            DateTime? parsedEnd = null;
            if (!string.IsNullOrEmpty(model.End)) // Só tenta parsear se End não for vazio
            {
                if (DateTime.TryParse(model.End, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var tempEnd)) // Tenta parsear
                {
                    parsedEnd = tempEnd; // Atribui a parsedEnd SE o parse for bem-sucedido
                } else // Se não for vazio E o parse falhar
                {
                    ModelState.AddModelError("End", "Formato inválido para data/hora final.");
                    return BadRequest(ModelState); // Retorna erro
                }
            }
            // Se model.End era null ou vazio, parsedEnd continua null (correto).
            // Se não era vazio e o parse falhou, já retornamos BadRequest.
            // Se não era vazio e o parse funcionou, parsedEnd tem o valor correto.
            // --- FIM DA LÓGICA CORRIGIDA ---

            try
            {
                var calendarEvent = await _context.CalendarEvents.FindAsync(model.Id);
                if (calendarEvent == null)
                {
                    return NotFound(new { success = false, message = "Evento não encontrado." });
                }
                // TODO: Verificar permissão se multi-tenant

                calendarEvent.UpdateTiming(parsedStart, parsedEnd, model.AllDay);
                _context.CalendarEvents.Update(calendarEvent);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Tempo do evento atualizado (Drag/Drop/Resize): {EventId}", model.Id);
                return Ok(new { success = true });
            } catch (ArgumentException argEx) // Captura erro de data fim < data início
            {
                _logger.LogWarning(argEx, "Erro de argumento ao atualizar tempo do evento.");
                return BadRequest(new { success = false, message = argEx.Message });
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tempo do evento {EventId}", model.Id);
                // Retornar erro para o JS poder fazer info.revert()
                return StatusCode(500, new { success = false, message = "Erro interno ao atualizar o evento." });
            }
        }

        // --- Action para buscar Próximos Eventos (para a lista lateral) ---
        [HttpGet]
        public async Task<IActionResult> GetUpcomingEvents(int count = 5) // Pega os próximos 5 por padrão
        {
            try
            {
                var now = DateTime.UtcNow; // Usar UTC se as datas no banco estiverem em UTC
                var upcomingEvents = await _context.CalendarEvents
                    .Where(e => e.Start >= now)
                    // .Where(e => e.UserId == GetCurrentUserId()) // Se multi-tenant
                    .OrderBy(e => e.Start)
                    .Take(count)
                    .ToListAsync();

                // Mapear para DTO ou um ViewModel específico para a lista
                var upcomingDtos = upcomingEvents.Select(ev => new CalendarEventDto
                {
                    Id = ev.Id,
                    Title = ev.Title,
                    Start = ev.Start.ToString(IsoDateTimeFormat), // Pode precisar de formato diferente para display
                    End = ev.End?.ToString(IsoDateTimeFormat),
                    AllDay = ev.AllDay,
                    ClassName = CalendarEventDto.MapCategoryToClassName(ev.Category), // Usado para a cor do ícone
                    ExtendedProps = new Dictionary<string, object?>
                     {
                         { "location", ev.Location },
                         { "description", ev.Description },
                         { "category", ev.Category },
                         // Formatar datas/horas para exibição amigável
                         { "displayStartDate", FormatDateForDisplay(ev.Start, ev.AllDay) },
                         { "displayEndDate", ev.End.HasValue ? FormatDateForDisplay(ev.End.Value, ev.AllDay, true) : null },
                         { "displayTimeRange", FormatTimeRangeForDisplay(ev.Start, ev.End, ev.AllDay) }
                     }
                }).ToList();


                // Retornar Partial View com os dados
                return PartialView("_UpcomingEventsListPartial", upcomingDtos);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar próximos eventos.");
                // Retornar um erro amigável ou uma partial vazia com mensagem
                return PartialView("_UpcomingEventsListPartial", new List<CalendarEventDto>());
            }
        }


        // --- Métodos Auxiliares ---

        private List<SelectListItem> GetCategoryOptions()
        {
            // Corresponde aos valores no HTML original e `MapCategoryToClassName`
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Danger", Text = "Danger" },
                new SelectListItem { Value = "Success", Text = "Success" },
                new SelectListItem { Value = "Primary", Text = "Primary" },
                new SelectListItem { Value = "Info", Text = "Info" },
                new SelectListItem { Value = "Dark", Text = "Dark" },
                new SelectListItem { Value = "Warning", Text = "Warning" }
            };
        }

        private bool TryParseEventDates(CalendarEventInputModel model, out DateTime parsedStart, out DateTime? parsedEnd, out bool isAllDay)
        {
            parsedStart = default;
            parsedEnd = null;
            isAllDay = false;

            // Tenta parsear o EventDate (pode ser data única ou range "to")
            var dateParts = model.EventDate.Split(" to ");
            if (!DateTime.TryParseExact(dateParts [0].Trim(), IsoDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDatePart))
            {
                return false; // Falha ao parsear data inicial
            }

            DateTime? endDatePart = null;
            if (dateParts.Length > 1 && DateTime.TryParseExact(dateParts [1].Trim(), IsoDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var tempEndDatePart))
            {
                endDatePart = tempEndDatePart;
            }

            // Verifica se é AllDay (sem hora ou com range de datas)
            if (string.IsNullOrWhiteSpace(model.StartTime) || endDatePart.HasValue)
            {
                isAllDay = true;
                parsedStart = startDatePart; // Hora será 00:00:00
                // Se AllDay e tiver EndDate, FullCalendar geralmente espera que o End seja o *início* do dia seguinte ao último dia do evento.
                // A lógica no construtor/UpdateDetails do CalendarEvent ajustará isso.
                parsedEnd = endDatePart;
            } else // Evento com hora específica
            {
                isAllDay = false;
                // Combina data e hora (Start)
                if (!TimeSpan.TryParseExact(model.StartTime, "h\\:m", CultureInfo.InvariantCulture, out var startTimeSpan) && // Formato 12h AM/PM
                    !TimeSpan.TryParseExact(model.StartTime, "H\\:m", CultureInfo.InvariantCulture, out startTimeSpan)) // Formato 24h
                {
                    // Tenta parsear com Flatpickr (HH:mm)
                    if (!TimeSpan.TryParseExact(model.StartTime, TimeFormat, CultureInfo.InvariantCulture, out startTimeSpan))
                    {
                        _logger.LogWarning("Falha ao parsear StartTime: {StartTime}", model.StartTime);
                        return false;
                    }
                }
                parsedStart = startDatePart.Add(startTimeSpan);

                // Combina data e hora (End), se houver EndTime
                if (!string.IsNullOrWhiteSpace(model.EndTime))
                {
                    if (!TimeSpan.TryParseExact(model.EndTime, "h\\:m", CultureInfo.InvariantCulture, out var endTimeSpan) &&
                        !TimeSpan.TryParseExact(model.EndTime, "H\\:m", CultureInfo.InvariantCulture, out endTimeSpan))
                    {
                        // Tenta parsear com Flatpickr (HH:mm)
                        if (!TimeSpan.TryParseExact(model.EndTime, TimeFormat, CultureInfo.InvariantCulture, out endTimeSpan))
                        {
                            _logger.LogWarning("Falha ao parsear EndTime: {EndTime}", model.EndTime);
                            return false;
                        }
                    }
                    // Se EndTime for fornecido, usa a mesma data de início por padrão
                    // (a lógica pode precisar ajustar se o evento cruzar a meia-noite)
                    parsedEnd = startDatePart.Add(endTimeSpan);
                } else
                {
                    // Se não houver EndTime, o evento pode ter duração indefinida ou padrão (ex: 1 hora)
                    // Aqui, definimos como nulo, mas pode ajustar conforme regra de negócio
                    parsedEnd = null;
                }
            }

            return true;
        }

        // Funções auxiliares para formatar datas/horas para a lista de Upcoming Events
        private string FormatDateForDisplay(DateTime date, bool isAllDay, bool isEndDate = false)
        {
            // Para AllDay, se for a data final, subtrai um dia para mostrar o último dia real do evento
            if (isAllDay && isEndDate)
                date = date.AddDays(-1);
            return date.ToString("dd MMM yyyy", new CultureInfo("pt-BR")); // Ex: 03 Jan 2024
        }

        private string FormatTimeRangeForDisplay(DateTime start, DateTime? end, bool isAllDay)
        {
            if (isAllDay)
                return "Dia Inteiro";
            if (!end.HasValue)
                return start.ToString("HH:mm", new CultureInfo("pt-BR")); // Ex: 10:30

            return $"{start.ToString("HH:mm", new CultureInfo("pt-BR"))} - {end.Value.ToString("HH:mm", new CultureInfo("pt-BR"))}"; // Ex: 10:30 - 11:30
        }

        // private string GetCurrentUserId() { /* Implementar lógica para pegar ID do usuário logado */ return User.FindFirstValue(ClaimTypes.NameIdentifier); }

    }
}
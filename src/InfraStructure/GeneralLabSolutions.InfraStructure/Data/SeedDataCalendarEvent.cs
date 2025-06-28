using GeneralLabSolutions.Domain.Entities;
// using Microsoft.EntityFrameworkCore; // Não necessário aqui
// using Microsoft.Extensions.DependencyInjection; // Não necessário aqui
using GeneralLabSolutions.InfraStructure.Data; // Using correto
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralLabSolutions.InfraStructure.Data
{
    public static class SeedDataCalendarEvent
    {
        public static void Initialize(AppDbContext context)
        {
            if (context == null || context.CalendarEvents == null)
            {
                throw new ArgumentNullException("Null AppDbContext or CalendarEvents DbSet passed to SeedDataCalendarEvent");
            }

            if (context.CalendarEvents.Any()) // Verifica no banco se já existe
            {
                Console.WriteLine("SeedData: CalendarEvent já existem no banco de dados.");
                return;
            }

            Console.WriteLine("Gerando SeedData para CalendarEvents...");

            var today = DateTime.UtcNow.Date; // Usar UtcNow.Date para consistência
            var random = new Random();
            var events = new List<CalendarEvent>();

            try // Adiciona um try-catch geral para o processo de criação
            {
                // Evento 1: Feriado (Exemplo Fixo)
                events.Add(new CalendarEvent("Feriado Nacional", today.AddDays(-15), null, true, "Danger", "Brasil", "Dia da Independência (Exemplo Fixo)"));

                // Evento 2: Planejamento Semanal (Exemplo Fixo - Ajustado)
                DateTime startPlan = today.AddDays(2);
                DateTime endPlan = today.AddDays(4); // Fim deve ser > Início
                events.Add(new CalendarEvent("Planejamento Semanal", startPlan, endPlan, true, "Primary", "Escritório", "Definir metas da semana"));

                // Evento 3: Reunião (Exemplo Fixo - Ajustado)
                DateTime startReuniao = today.AddDays(1).AddHours(10);
                DateTime endReuniao = startReuniao.AddHours(1); // Garante que o fim é depois do início
                events.Add(new CalendarEvent("Reunião de Equipe", startReuniao, endReuniao, false, "Info", "Sala de Conferência", "Discussão do projeto X"));

                // Evento 4: Almoço (Exemplo Fixo - Ajustado)
                DateTime startAlmoco = today.AddDays(3).AddHours(12).AddMinutes(30);
                DateTime endAlmoco = startAlmoco.AddHours(1).AddMinutes(30); // Garante que o fim é depois do início
                events.Add(new CalendarEvent("Almoço com Cliente", startAlmoco, endAlmoco, false, "Success", "Restaurante Y", "Fechar novo contrato"));

                // Evento 5: Apresentação (Exemplo Fixo - Ajustado)
                DateTime startApres = today.AddDays(8).AddHours(15);
                DateTime endApres = startApres.AddHours(1); // Garante que o fim é depois do início
                events.Add(new CalendarEvent("Apresentação Relatório", startApres, endApres, false, "Warning", "Online", "Resultados do Trimestre"));

                // Evento 6: Workshop (Aleatório - Corrigido)
                int startDayW = random.Next(5, 20);
                int durationDaysW = random.Next(0, 3); // Duração de 0 a 2 dias adicionais
                DateTime startWorkshop = today.AddDays(startDayW).AddHours(random.Next(9, 13)); // Começa entre 9 e 12
                // Garante que End seja depois de Start
                DateTime endWorkshop = startWorkshop.AddDays(durationDaysW).AddHours(random.Next(4, 8)); // Termina entre 4 e 7 horas depois no mesmo dia ou dias depois
                events.Add(new CalendarEvent("Workshop Interno", startWorkshop, endWorkshop, false, "Dark", "Auditório", "Novas Tecnologias"));

                // Evento 7: Happy Hour (Aleatório - Corrigido)
                int startDayH = random.Next(10, 25);
                DateTime startHappyHour = today.AddDays(startDayH).AddHours(18);
                // Garante que End seja depois de Start
                DateTime endHappyHour = startHappyHour.AddHours(random.Next(1, 3)); // Dura 1 ou 2 horas
                events.Add(new CalendarEvent("Happy Hour", startHappyHour, endHappyHour, false, "Success", "Bar Z", "Confraternização"));

                // Adicionar mais alguns eventos aleatórios para teste
                for (int i = 0; i < 10; i++)
                {
                    string title = $"Evento Aleatório {i + 1}";
                    string category = new [] { "Primary", "Success", "Info", "Warning", "Danger", "Dark" } [random.Next(6)];
                    bool allDay = random.Next(0, 5) == 0; // 1 em 5 chance de ser dia inteiro
                    DateTime start = today.AddDays(random.Next(-10, 30)).AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60));
                    DateTime? end = null;
                    if (!allDay)
                    {
                        // Garante que end > start para eventos não-dia-inteiro
                        end = start.AddHours(random.Next(1, 5)).AddMinutes(random.Next(0, 60));
                    }
                    // A lógica dentro do construtor/UpdateDetails de CalendarEvent deve ajustar o 'end' para eventos 'allDay' se necessário.
                    events.Add(new CalendarEvent(title, start, end, allDay, category, $"Local {i}", $"Descrição {i}"));
                }


                context.CalendarEvents.AddRange(events); // Adiciona ao contexto recebido
                Console.WriteLine("SeedData para CalendarEvents adicionado ao contexto.");

                // **NÃO HÁ SaveChanges() AQUI** - O DbInitializer fará isso no final.

            } catch (Exception ex)
            {
                Console.WriteLine($"!!!! Erro durante a geração de CalendarEvents: {ex.Message} !!!!");
                // Decide se quer parar ou continuar
                throw; // Re-lança para parar o DbInitializer e evitar SaveChanges inconsistente
            }
        }
    }
}
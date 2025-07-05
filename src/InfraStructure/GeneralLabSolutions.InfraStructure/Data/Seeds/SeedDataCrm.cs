using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.CRM;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralLabSolutions.InfraStructure.Data.Seeds
{
    public static class SeedDataCrm
    {
        // *** MÉTODO ALTERADO PARA RECEBER O CONTEXTO DIRETAMENTE ***
        public static void Initialize(AppDbContext context)
        {
            if (context.Leads.Any())
            {
                Console.WriteLine("SeedData para CRM já existe.");
                return;
            }

            Console.WriteLine("Gerando SeedData para o Módulo de CRM...");

            // --- 1. Criar e SALVAR Leads para gerar os Guids ---
            var leadJoao = new Lead("João Silva", "joao.silva@techcorp.com", "Tech Corp", "(11) 99999-1234", "CTO", "Website", "Maria Santos");
            leadJoao.UpdateTemperature(LeadTemperature.Quente);

            var leadAna = new Lead("Ana Costa", "ana.costa@inovacao.com", "Inovação Ltda", "(11) 88888-5678", "Gerente de TI", "Referral", "Carlos Oliveira");
            leadAna.UpdateStatus(LeadStatus.Contato);

            var leadPedro = new Lead("Pedro Santos", "pedro@startupxyz.com", "Startup XYZ", "(11) 77777-9012", "CEO", "Event", "Maria Santos");
            leadPedro.UpdateStatus(LeadStatus.Qualificado);
            leadPedro.UpdateTemperature(LeadTemperature.Quente);

            var leads = new List<Lead> { leadJoao, leadAna, leadPedro };
            context.Leads.AddRange(leads);

            // *** PONTO CRÍTICO: Salvamos os leads para que o EF Core atribua os Ids ***
            // Como estamos dentro de uma transação no DbInitializer, esta operação é segura.
            context.SaveChanges();
            Console.WriteLine("Leads salvos, Guids gerados.");


            // --- 2. Criar Tarefas (CrmTasks) usando os Guids gerados ---
            var tasks = new List<CrmTask>
            {
                new CrmTask(leadJoao.Id, "Follow-up com João Silva", "Verificar interesse em proposta comercial", new DateTime(2025, 1, 20), TaskPriority.Alta, "Maria Santos"),
                new CrmTask(leadJoao.Id, "Preparar demonstração", "Criar apresentação personalizada para Tech Corp", new DateTime(2025, 1, 18), TaskPriority.Media, "Carlos Oliveira"),
                new CrmTask(leadPedro.Id, "Enviar proposta comercial", "Finalizar e enviar proposta para Startup XYZ", new DateTime(2025, 1, 16), TaskPriority.Alta, "Maria Santos")
            };
            tasks [2].CompleteTask();
            context.CrmTasks.AddRange(tasks);


            // --- 3. Criar Atividades (Agenda) ---
            var activities = new List<Activity>
            {
                new Activity(leadJoao.Id, "call", "Ligação de Follow-up - João Silva", "Verificar andamento da proposta e responder dúvidas", new DateTime(2025, 6, 18, 10, 0, 0)),
                new Activity(leadJoao.Id, "meeting", "Reunião de Demonstração - Tech Corp", "Apresentar solução completa para o time de TI", new DateTime(2025, 6, 19, 14, 30, 0)),
                new Activity(leadAna.Id, "email", "Envio de Proposta - Ana Costa", "Enviar proposta comercial personalizada por email", new DateTime(2025, 6, 20, 9, 0, 0)),
                new Activity(null, "task", "Atualização do CRM", "Revisar e atualizar dados de leads no sistema", new DateTime(2025, 6, 23, 8, 0, 0))
            };
            context.Activities.AddRange(activities);

            Console.WriteLine("Dados de CRM (Tasks, Activities) adicionados ao contexto.");
            // O SaveChanges final será chamado pelo DbInitializer, comitando toda a transação.
        }
    }
}
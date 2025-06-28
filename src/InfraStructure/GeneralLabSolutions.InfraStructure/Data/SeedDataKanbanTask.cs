// Caminho: GeneralLabSolutions.InfraStructure/Data/SeedDataKanbanTask.cs
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralLabSolutions.InfraStructure.Data
{
    public static class SeedDataKanbanTask
    {
        private static readonly Random _random = new Random();

        private static T GetRandomEnum<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(_random.Next(values.Length));
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                if (context.KanbanTask.Any())
                {
                    Console.WriteLine("SeedData para KanbanTask já existe.");
                    return;
                }

                var participantes = context.Participante.ToList();
                if (!participantes.Any())
                {
                    Console.WriteLine("SeedData para KanbanTask: Nenhum participante encontrado. Execute SeedDataParticipante primeiro.");
                    return;
                }

                Console.WriteLine("Gerando SeedData para KanbanTask...");

                var kanbanTasks = new List<KanbanTask>();
                var titulosUsados = new HashSet<string>();
                var descricoesUsadas = new HashSet<string>();


                string [] titulosBase = { "Revisar Documentação", "Desenvolver Feature", "Corrigir Bug", "Planejar Sprint", "Testar Módulo", "Deploy em Staging", "Atualizar Dependências", "Refatorar Código", "Criar Relatório", "Pesquisar Solução", "Otimizar Query", "Validar Requisitos", "Escrever Testes", "Configurar CI/CD", "Analisar Performance" };
                string [] descricoesBase = {
                    "Verificar a documentação técnica do novo componente X e atualizar se necessário.",
                    "Implementar a funcionalidade de exportação de dados para CSV, conforme especificado no backlog.",
                    "Investigar e corrigir o problema de lentidão na tela de checkout reportado pelo usuário Y.",
                    "Definir as tarefas, estimativas e prioridades para a próxima iteração de desenvolvimento.",
                    "Executar testes unitários, de integração e de UI para o módulo de pagamentos.",
                    "Realizar o deploy da versão 1.2.3 no ambiente de staging para validação final.",
                    "Verificar as versões das bibliotecas do projeto e atualizar para as mais recentes estáveis.",
                    "Melhorar a estrutura e legibilidade do código da classe de serviço de Usuários.",
                    "Compilar os dados de vendas do último trimestre e gerar o relatório de performance.",
                    "Buscar e avaliar alternativas de bibliotecas para a funcionalidade de geração de gráficos.",
                    "Analisar e otimizar a consulta SQL que está causando lentidão no dashboard principal.",
                    "Conduzir uma reunião com o Product Owner para validar os requisitos da próxima grande feature.",
                    "Escrever casos de teste automatizados para cobrir os novos endpoints da API.",
                    "Configurar o pipeline de Integração Contínua e Deploy Contínuo no servidor de build.",
                    "Realizar testes de carga e identificar gargalos de performance na aplicação."
                };


                for (int i = 0; i < 20; i++) // Criar 20 tarefas de exemplo
                {
                    string titulo;
                    string descricao;

                    // Gerar Título Único
                    do
                    {
                        titulo = $"{titulosBase [_random.Next(titulosBase.Length)]} - Task {i + _random.Next(100, 999)}";
                    } while (titulosUsados.Contains(titulo));
                    titulosUsados.Add(titulo);

                    // Gerar Descrição Única
                    do
                    {
                        descricao = $"{descricoesBase [_random.Next(descricoesBase.Length)]} (Detalhe específico {_random.Next(1000, 9999)})";
                    } while (descricoesUsadas.Contains(descricao));
                    descricoesUsadas.Add(descricao);

                    DateTime? dueDate = _random.Next(0, 3) == 0 ? (DateTime?)null : DateTime.UtcNow.AddDays(_random.Next(1, 45)); // Aumentei a chance de não ter due date e o range

                    var task = new KanbanTask(titulo, descricao, dueDate)
                    {
                        Column = GetRandomEnum<Column>(),
                        Priority = GetRandomEnum<Priority>()
                    };

                    int numParticipantesParaTarefa = _random.Next(0, Math.Min(4, participantes.Count + 1));
                    var participantesDisponiveis = new List<Participante>(participantes);

                    for (int j = 0; j < numParticipantesParaTarefa && participantesDisponiveis.Any(); j++)
                    {
                        int randomIndex = _random.Next(participantesDisponiveis.Count);
                        task.Participantes.Add(participantesDisponiveis [randomIndex]);
                        participantesDisponiveis.RemoveAt(randomIndex);
                    }
                    kanbanTasks.Add(task);
                }

                context.KanbanTask.AddRange(kanbanTasks);
                try
                {
                    context.SaveChanges(); // Mantendo por enquanto
                    Console.WriteLine("SeedData para KanbanTask gerado com sucesso!");
                } catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao salvar SeedData de KanbanTask: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    // Para debug, você pode querer ver as tarefas que seriam inseridas:
                    foreach(var kt in kanbanTasks) { Console.WriteLine($"Tentando inserir: T: {kt.Title}, D: {kt.Description}"); }
                }
            }
        }
    }
}
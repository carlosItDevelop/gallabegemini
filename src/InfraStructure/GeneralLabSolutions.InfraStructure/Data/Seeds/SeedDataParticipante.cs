// Caminho: GeneralLabSolutions.InfraStructure/Data/SeedDataParticipante.cs
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralLabSolutions.InfraStructure.Data.Seeds
{
    public static class SeedDataParticipante
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                if (context.Participante.Any())
                {
                    Console.WriteLine("SeedData para Participante já existe.");
                    return;
                }

                Console.WriteLine("Gerando SeedData para Participante...");

                var random = new Random();
                var participantes = new List<Participante>();
                var nomesUsados = new HashSet<string>(); // Para rastrear nomes completos já usados

                string [] nomesBase = { "Alice", "Bob", "Charlie", "Diana", "Eduardo", "Fernanda", "Gabriel", "Helena", "Igor", "Julia", "Lucas", "Mariana", "Nicolas", "Olivia", "Pedro" };
                string [] sobrenomesBase = { "Silva", "Souza", "Oliveira", "Pereira", "Costa", "Rodrigues", "Almeida", "Nunes", "Lima", "Gomes", "Martins", "Rocha", "Ferreira", "Alves", "Dias" };
                string [] dominiosEmail = { "example.com", "company.com", "project.org", "test.net", "inova.dev", "tech.io" };

                int participantesACriar = 6; // Defina quantos participantes você quer
                int tentativasPorNome = 0;

                while (participantes.Count < participantesACriar)
                {
                    string nome = nomesBase [random.Next(nomesBase.Length)];
                    string sobrenome = sobrenomesBase [random.Next(sobrenomesBase.Length)];
                    string nomeCompletoBase = $"{nome} {sobrenome}";
                    string nomeCompletoFinal = nomeCompletoBase;

                    // Tenta gerar um nome único
                    tentativasPorNome = 0;
                    while (nomesUsados.Contains(nomeCompletoFinal) && tentativasPorNome < 3)
                    {
                        // Tenta uma nova combinação de nome/sobrenome (pode repetir a base, mas a chance diminui)
                        nome = nomesBase [random.Next(nomesBase.Length)];
                        sobrenome = sobrenomesBase [random.Next(sobrenomesBase.Length)];
                        nomeCompletoBase = $"{nome} {sobrenome}";
                        nomeCompletoFinal = nomeCompletoBase;
                        tentativasPorNome++;
                    }

                    // Se após 3 tentativas ainda houver colisão, adiciona um sufixo numérico único
                    if (nomesUsados.Contains(nomeCompletoFinal))
                    {
                        int sufixo = 1;
                        do
                        {
                            nomeCompletoFinal = $"{nomeCompletoBase} {sufixo++}";
                        } while (nomesUsados.Contains(nomeCompletoFinal));
                    }

                    nomesUsados.Add(nomeCompletoFinal);

                    string emailPart = nomeCompletoFinal.ToLower()
                                                    .Replace(" ", ".")
                                                    .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                                                    .Replace("ã", "a").Replace("õ", "o").Replace("ç", "c");
                    // Remover outros caracteres especiais se necessário
                    emailPart = new string(emailPart.Where(c => char.IsLetterOrDigit(c) || c == '.').ToArray());


                    string email = $"{emailPart}{random.Next(10, 99)}@{dominiosEmail [random.Next(dominiosEmail.Length)]}";

                    participantes.Add(new Participante(nomeCompletoFinal, email));
                }

                context.Participante.AddRange(participantes);
                try
                {
                    context.SaveChanges(); // Mantendo por enquanto
                    Console.WriteLine($"SeedData para Participante: {participantes.Count} participantes gerados com sucesso!");
                } catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao salvar SeedData de Participante: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                }
            }
        }
    }
}
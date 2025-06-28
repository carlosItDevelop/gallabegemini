using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class SeedDataStatusDoItemIncompativel
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new AppDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
        {
            if (context == null || context.StatusDoItem == null || context.StatusDoItemIncompativel == null)
            {
                throw new ArgumentNullException("Null AppDbContext");
            }

            if (!context.StatusDoItemIncompativel.Any())
            {
                // Obtém todos os status para associá-los posteriormente
                var status = context.StatusDoItem.ToList();

                // Lista de incompatibilidades (exemplo)
                var incompativeis = new List<StatusDoItemIncompativel>
                {
                    // Exemplo: "Entregue" é incompatível com "Na Alfândega" e vice-versa
                    new StatusDoItemIncompativel {
                        StatusDoItem = status.FirstOrDefault(s => s.Descricao == "Entregue ao cliente"),
                        StatusDoItemIncompativelNavigation = status.FirstOrDefault(s => s.Descricao == "Na Alfândega")
                    },
                    new StatusDoItemIncompativel {
                        StatusDoItem = status.FirstOrDefault(s => s.Descricao == "Na Alfândega"),
                        StatusDoItemIncompativelNavigation = status.FirstOrDefault(s => s.Descricao == "Entregue ao cliente")
                    },
                    // Adicione outras incompatibilidades conforme as regras de negócio
                };

                // Adicionar as instâncias ao contexto e salvar as mudanças
                context.StatusDoItemIncompativel.AddRange(incompativeis);
                context.SaveChanges();

                Console.WriteLine("SeedData para StatusDoItemIncompativel gerado com sucesso!");
            } else
            {
                Console.WriteLine("O SeedData para StatusDoItemIncompativel já foi gerado!");
            }
        }
    }
}
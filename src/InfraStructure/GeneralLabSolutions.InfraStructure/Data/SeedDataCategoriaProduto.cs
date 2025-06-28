using GeneralLabSolutions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralLabSolutions.InfraStructure.Data
{
    public static class SeedDataCategoriaProduto
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                if (context == null || context.CategoriaProduto == null)
                {
                    throw new ArgumentNullException("Null AppDbContext");
                }

                // ATENÇÃO: Categorias REAIS do sistema. Por num DropDown no Creat e Edit de Produto
                // O nome correto é "Família de Produto" (Mostrar no Display), mas o nome "Categoria" é mais comum.
                if (!context.CategoriaProduto.Any())
                {
                    context.CategoriaProduto.AddRange(
                        new CategoriaProduto("Equipamento kinematica"),
                        new CategoriaProduto("Equipamentos Koehler instrument"),
                        new CategoriaProduto("Equipamentos spex sample prep"),
                        new CategoriaProduto("Equipamentos cole Parmer"),
                        new CategoriaProduto("Equipamentos Memmert"),
                        new CategoriaProduto("Equipamentos huber"),
                        new CategoriaProduto("Equipamentos Van der heisen"),
                        new CategoriaProduto("Equipamentos endecotts"),
                        new CategoriaProduto("Equipamentos wiggens"),
                        new CategoriaProduto("Equipamentos slb"),
                        new CategoriaProduto("Equipamentos emcee Electronics"),
                        new CategoriaProduto("Equipamentos Omnitek"),
                        new CategoriaProduto("Equipamentos scion"),
                        new CategoriaProduto("Equipamentos Techcomp"),
                        new CategoriaProduto("Equipamentos steroglass"),
                        new CategoriaProduto("Equipamentos 3vtech"),
                        new CategoriaProduto("Equipamentos SCP Science"),
                        new CategoriaProduto("Equipamentos diversos"),
                        new CategoriaProduto("Acessórios Koehler Instrument"),
                        new CategoriaProduto("Acessórios spex sample prep"),
                        new CategoriaProduto("Acessórios kinematica"),
                        new CategoriaProduto("Acessórios SCP Science"),
                        new CategoriaProduto("Acessórios cole Parmer"),
                        new CategoriaProduto("Acessórios Memmert"),
                        new CategoriaProduto("Acessórios huber"),
                        new CategoriaProduto("Acessórios wiggens"),
                        new CategoriaProduto("Acessórios. SLB"),
                        new CategoriaProduto("Acessórios scion")
                    );
                    context.SaveChanges();
                    Console.WriteLine("SeedData para Categoria de Produto gerado com sucesso!");
                } else
                {
                    Console.WriteLine("O SeedData para Categoria de Produto Já foi gerado!");
                }
            }
        }
    }
}
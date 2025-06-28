// ... (outros using)
using GeneralLabSolutions.InfraStructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralLabSolutions.InfraStructure.Data
{
    public static class DbInitializer
    {
        /// <summary>
        /// Classe que unifica a criação dos SeedDatas da Solução
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // ToDo: Vou manter como está, por enquanto. Documentador; não esqueça dessa decisão!

            using (var scope = serviceProvider.CreateScope())
            {
                var scopedProvider = scope.ServiceProvider;
                var context = scopedProvider.GetRequiredService<AppDbContext>();

                // Usar uma única transação para todos os SeedData
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Chamar os SeedData na ordem correta
                        SeedDataCategoriaProduto.Initialize(scopedProvider);
                        SeedDataFornecedor.Initialize(scopedProvider);
                        SeedDataCliente.Initialize(scopedProvider);
                        SeedDataVendedor.Initialize(scopedProvider);
                        SeedDataProduto.Initialize(scopedProvider);
                        SeedDataStatusDoItem.Initialize(scopedProvider);
                        SeedDataStatusDoItemIncompativel.Initialize(scopedProvider);
                        SeedDataPedido.Initialize(scopedProvider);


                        // --- INÍCIO DA ADIÇÃO ---
                        SeedDataCrm.Initialize(context); 
                        // Adicionando nosso novo seeder de CRM
                        // --- FIM DA ADIÇÃO ---


                        SeedDataParticipante.Initialize(scopedProvider); // ANTES de KanbanTask
                        SeedDataKanbanTask.Initialize(scopedProvider);



                        // ToDo: Refatorar os SeedDatas acima, para usar o contexto ativo, salvando tudo em uma única Trasaction;

                        // (depois dos outros seeds que criam Pessoa)
                        SeedDataContato.Initialize(context);
                        SeedDataTelefone.Initialize(context);
                        SeedDataEndereco.Initialize(context);
                        SeedDataDadosBancarios.Initialize(context);
                        // ------------------------------------- //

                        SeedDataCalendarEvent.Initialize(context);

                        // Se chegou até aqui sem erros, commita a transação
                        //Salvar todas as alterações na mesma transação.
                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        // Adicionar mensagens de sucesso para cada SeedData, se necessário

                        Console.WriteLine("\n\n================================\nSeedData executado com sucesso!");
                    } catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Erro durante a execução do SeedData: {ex.Message}");
                        // Aqui você pode logar o erro, lançar a exceção novamente, etc.
                        throw;
                    }
                }
            }
        }
    }
}
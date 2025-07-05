using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralLabSolutions.InfraStructure.Data.Seeds
{
    public static class SeedDataStatusDoItem
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                if (context == null || context.StatusDoItem == null)
                {
                    throw new ArgumentNullException("Null AppDbContext");
                }

                if (!context.StatusDoItem.Any())
                {
                    context.StatusDoItem.AddRange(
                        new StatusDoItem { Descricao = "Pago", Ativo = true },
                        new StatusDoItem { Descricao = "Entregue", Ativo = true },
                        new StatusDoItem { Descricao = "Aguardando a Transportadora", Ativo = true },
                        new StatusDoItem { Descricao = "Em Revisao", Ativo = true },
                        new StatusDoItem { Descricao = "Em Transito", Ativo = true },
                        new StatusDoItem { Descricao = "Na Alfândega", Ativo = true },
                        new StatusDoItem { Descricao = "Proposta de venda", Ativo = true },
                        new StatusDoItem { Descricao = "Item retirado da proposta", Ativo = true },
                        new StatusDoItem { Descricao = "Pedido de venda", Ativo = true },
                        new StatusDoItem { Descricao = "Pedido de compra", Ativo = true },
                        new StatusDoItem { Descricao = "Indisponível para compra", Ativo = true },
                        new StatusDoItem { Descricao = "Compra confirmada", Ativo = true },
                        new StatusDoItem { Descricao = "Compra cancelada", Ativo = true },
                        new StatusDoItem { Descricao = "Em processo com fornecedor", Ativo = true },
                        new StatusDoItem { Descricao = "Cancelado com observação do motivo", Ativo = true },
                        new StatusDoItem { Descricao = "Atrasado com observação do motivo", Ativo = true },
                        new StatusDoItem { Descricao = "Enviado ao Brasil", Ativo = true },
                        new StatusDoItem { Descricao = "Recebido na empresa", Ativo = true },
                        new StatusDoItem { Descricao = "Produto OK", Ativo = true },
                        new StatusDoItem { Descricao = "Fora de especificações com reparo", Ativo = true },
                        new StatusDoItem { Descricao = "Fora de especificações definitivo, sem possibilidade de reparo", Ativo = true },
                        new StatusDoItem { Descricao = "Em reparo", Ativo = true },
                        new StatusDoItem { Descricao = "Enviado ao cliente", Ativo = true },
                        new StatusDoItem { Descricao = "Atrasado para envio ao cliente com a transportadora", Ativo = true },
                        new StatusDoItem { Descricao = "Envio cancelado", Ativo = true },
                        new StatusDoItem { Descricao = "Entregue ao cliente", Ativo = true },
                        new StatusDoItem { Descricao = "Entrega confirmada", Ativo = true }
                    );
                    context.SaveChanges();
                    Console.WriteLine("SeedData para StatusDoItem gerado com sucesso!");
                } else
                {
                    Console.WriteLine("O SeedData para StatusDoItem já foi gerado!");
                }
            }
        }
    }
}
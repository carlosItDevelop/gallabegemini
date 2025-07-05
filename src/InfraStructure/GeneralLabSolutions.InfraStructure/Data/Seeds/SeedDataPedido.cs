using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralLabSolutions.InfraStructure.Data.Seeds
{
    public static class SeedDataPedido
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                if (context == null || context.Pedido == null || context.ItemPedido == null || context.Cliente == null || context.Vendedor == null || context.Produto == null)
                {
                    throw new ArgumentNullException("Null AppDbContext");
                }

                if (!context.Pedido.Any())
                {
                    var random = new Random();
                    var clientes = context.Cliente.ToList();
                    var vendedores = context.Vendedor.ToList();
                    var produtos = context.Produto.ToList();
                    var statusItens = context.StatusDoItem.ToList();
                    var incompatibilidades = context.StatusDoItemIncompativel.ToList();

                    for (int ano = 2022; ano <= 2024; ano++)
                    {
                        for (int mes = 1; mes <= 12; mes++)
                        {
                            int numPedidos = random.Next(6, 16);

                            for (int i = 0; i < numPedidos; i++)
                            {
                                try
                                {
                                    var cliente = clientes [random.Next(clientes.Count)];
                                    var vendedor = vendedores [random.Next(vendedores.Count)];
                                    var dataPedido = new DateTime(ano, mes, random.Next(1, DateTime.DaysInMonth(ano, mes) + 1));

                                    //var pedido = new Pedido(cliente.Id, vendedor.Id, dataPedido);
                                    //pedido.AtualizarStatus(StatusDoPedido.Orcamento);

                                    var pedido = new Pedido(cliente.Id, vendedor.Id, dataPedido);
                                    pedido.AtualizarStatus(GetStatusDoPedidoByWeight(random));


                                    // Para Debugar
                                    //Console.WriteLine($"Criando Pedido para Cliente: {cliente.Nome}, Vendedor: {vendedor.Nome}, Data: {dataPedido.ToShortDateString()}");

                                    int numItens = random.Next(2, 7);
                                    for (int j = 0; j < numItens; j++)
                                    {
                                        var produto = produtos [random.Next(produtos.Count)];
                                        var quantidade = random.Next(1, 4);

                                        var itemPedido = new ItemPedido(
                                            pedidoId: pedido.Id,
                                            produtoId: produto.Id,
                                            quantidade: quantidade,
                                            valorUnitario: produto.ValorUnitario,
                                            nomeDoProduto: produto.Descricao
                                        );

                                        Console.WriteLine($"  - Item: {produto.Descricao}, Quantidade: {quantidade}");

                                        // Gerar de 1 a 3 StatusDoItem para cada ItemPedido
                                        int numStatus = random.Next(1, 4);
                                        var statusEscolhidos = GetStatusDoItemByWeight(random, statusItens, incompatibilidades, numStatus, itemPedido.Estados.ToList());


                                        if (statusEscolhidos != null && statusEscolhidos.Any())
                                        {
                                            foreach (var status in statusEscolhidos)
                                            {
                                                var estadoDoItem = new EstadoDoItem(itemPedido.Id, status.Id);
                                                itemPedido.Estados.Add(estadoDoItem);

                                                // Para Debugar
                                                //Console.WriteLine($"    - Adicionado EstadoDoItem: {status.Descricao} ao ItemPedido: {itemPedido.Id}");

                                            }
                                        } else
                                        {
                                            Console.WriteLine($"    - Nenhum status compatível encontrado para o ItemPedido {itemPedido.Id}");
                                        }

                                        pedido.AdicionarItem(itemPedido);
                                    }

                                    context.Pedido.Add(pedido);
                                } catch (Exception ex)
                                {
                                    Console.WriteLine($"Erro durante a criação do Pedido: {ex.Message}");
                                    if (ex.InnerException != null)
                                    {
                                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                                    }
                                    // Aqui você pode logar mais detalhes sobre o pedido e os itens que estavam sendo criados
                                    throw; // Relança a exceção para ser tratada no DbInitializer
                                }
                            }
                        }
                    }

                    context.SaveChanges(); // Removido SaveChanges daqui, será chamado apenas no DbInitializer
                    Console.WriteLine("SeedData para Pedido, ItemPedido e EstadoDoItem gerado com sucesso!");
                } else
                {
                    Console.WriteLine("O SeedData para Pedido, ItemPedido e EstadoDoItem já foi gerado!");
                }
            }
        }

        // Método para obter um StatusDoItem aleatório com base em pesos
        public static List<StatusDoItem> GetStatusDoItemByWeight(Random random, List<StatusDoItem> statusItens, List<StatusDoItemIncompativel> incompatibilidades, int maxStatus = 1, List<EstadoDoItem> estadosAtuais = null)
        {
            var pesos = new Dictionary<string, int>
            {
                { "Pago", 30 },
                { "Entregue", 20 },
                { "Aguardando a Transportadora", 5 },
                { "Em Revisao", 8 },
                { "Em Transito", 10 },
                { "Na Alfândega", 15 },
                // ... outros status e seus pesos
            };

            var statusEscolhidos = new List<StatusDoItem>();
            for (int i = 0; i < maxStatus; i++)
            {
                var statusPermitidos = statusItens.Where(s => PodeAdicionarStatus(s, statusItens, estadosAtuais, incompatibilidades)).ToList();
                if (!statusPermitidos.Any())
                {
                    break; // Nenhum status adicional pode ser adicionado
                }

                // Filtrar os pesos para incluir apenas os status permitidos
                var pesosFiltrados = pesos.Where(p => statusPermitidos.Any(s => s.Descricao == p.Key)).ToDictionary(p => p.Key, p => p.Value);

                int pesoTotal = pesosFiltrados.Values.Sum();
                int randomValue = random.Next(0, pesoTotal);
                int acumulado = 0;

                foreach (var status in statusPermitidos)
                {
                    if (pesosFiltrados.ContainsKey(status.Descricao))
                    {
                        acumulado += pesosFiltrados [status.Descricao];
                        if (randomValue < acumulado)
                        {
                            statusEscolhidos.Add(status);
                            break; // Próximo status
                        }
                    }
                }
            }

            return statusEscolhidos;
        }


        public static StatusDoPedido GetStatusDoPedidoByWeight(Random random)
        {
            var pesos = new Dictionary<StatusDoPedido, int>
            {
                { StatusDoPedido.Orcamento, 10 },   // Peso maior que cancelado
                { StatusDoPedido.EmProcessamento, 20 },  // Peso ainda maior
                { StatusDoPedido.Pago, 30 },        // Peso mais alto (prioritário)
                { StatusDoPedido.Enviado, 10 },     // Peso médio
                { StatusDoPedido.Entregue, 20 },    // Peso grande
                { StatusDoPedido.Cancelado, 5 }     // Peso mínimo (raridade)
            };

            // Calcular a soma total dos pesos
            int pesoTotal = pesos.Values.Sum();

            // Gerar um número aleatório no intervalo de 0 até pesoTotal - 1
            int randomValue = random.Next(0, pesoTotal);

            // Percorrer os pesos e retornar o status correspondente
            int acumulado = 0;
            foreach (var entry in pesos)
            {
                acumulado += entry.Value;
                if (randomValue < acumulado)
                {
                    return entry.Key;
                }
            }

            // Fallback (apenas como segurança, mas nunca deverá acontecer)
            return StatusDoPedido.Pago;
        }


        // Função para verificar se um status pode ser adicionado a um item
        private static bool PodeAdicionarStatus(StatusDoItem novoStatus, List<StatusDoItem> todosStatus, List<EstadoDoItem> estadosAtuais, List<StatusDoItemIncompativel> incompatibilidades)
        {
            // 1. Verificar se o novo status existe
            if (novoStatus == null)
            {
                return false; // Novo status não encontrado
            }

            // 2. Verificar Incompatibilidades
            foreach (var estadoAtual in estadosAtuais.Where(e => e.Ativo))
            {
                // Verificar se existe alguma incompatibilidade entre o estado atual e o novo status
                if (incompatibilidades.Any(i =>
                    i.StatusDoItemId == estadoAtual.StatusDoItemId && i.StatusDoItemIncompativelId == novoStatus.Id ||
                    i.StatusDoItemId == novoStatus.Id && i.StatusDoItemIncompativelId == estadoAtual.StatusDoItemId))
                {
                    return false; // Incompatibilidade encontrada
                }
            }

            // 3. Se nenhuma incompatibilidade for encontrada
            return true;
        }
    }
}
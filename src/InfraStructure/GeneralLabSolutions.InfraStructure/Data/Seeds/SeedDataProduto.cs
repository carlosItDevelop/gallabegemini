using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralLabSolutions.InfraStructure.Data.Seeds
{
    public static class SeedDataProduto
    {
        // Métodos auxiliares para gerar dados aleatórios
        private static string GerarCodigoProduto(Random random)
        {
            return $"{random.Next(100, 999)}-{random.Next(10, 99)}-{random.Next(1000, 9999)}";
        }

        private static string GerarNCM(Random random)
        {
            return $"{random.Next(1000, 9999)}.{random.Next(10, 99)}.{random.Next(10, 99)}";
        }

        private static string GerarStringNumerica(Random random, int tamanho)
        {
            return string.Join("", Enumerable.Range(0, tamanho).Select(_ => random.Next(0, 10).ToString()));
        }

        private static decimal GerarValorUnitario(Random random)
        {
            return (decimal)(random.NextDouble() * 10000); // Gera valores entre 0 e 10000
        }

        // **Modificação aqui!**
        private static string GerarDescricaoProduto(Random random, List<CategoriaProduto> categorias)
        {
            var adjetivos = new [] { "Incrível", "Moderno", "Eficiente", "Robusto", "Prático", "Inovador", "Compacto", "Leve", "Resistente", "Durável", "Confiável", "Preciso", "Versátil", "Econômico" };
            var nomes = new [] { "Dispositivo", "Equipamento", "Ferramenta", "Material", "Componente", "Acessório", "Sistema", "Instrumento", "Máquina", "Aparelho", "Kit", "Conjunto" };
            var complementos = new [] { "de Medição", "de Teste", "de Análise", "de Controle", "de Segurança", "de Precisão", "de Alto Desempenho", "de Última Geração", "de Alta Tecnologia", "de Baixa Manutenção" };
            var categoria = categorias [random.Next(categorias.Count)].Descricao;
            var adjetivo = adjetivos [random.Next(adjetivos.Length)];
            var nome = nomes [random.Next(nomes.Length)];
            var complemento = complementos [random.Next(complementos.Length)];

            // Gerar um GUID e pegar os primeiros 8 caracteres
            var guidParte = Guid.NewGuid().ToString().Substring(0, 8);

            return $"{adjetivo} {nome} {complemento} para {categoria} - {guidParte}";
        }

        // Função para retornar o status do produto baseado em pesos (reutilizada do código anterior)
        public static StatusDoProduto GetStatusDoProdutoByWeight(Random random)
        {
            // (Mesma implementação, não foi alterada)
            var pesos = new Dictionary<StatusDoProduto, int>
            {
                { StatusDoProduto.Dropshipping, 30 },
                { StatusDoProduto.Inativo, 5 },
                { StatusDoProduto.Reservado, 20 },
                { StatusDoProduto.EmEstoque, 10 },
                { StatusDoProduto.Esgotado, 5 }
            };

            int pesoTotal = pesos.Values.Sum();
            int randomValue = random.Next(0, pesoTotal);

            int acumulado = 0;
            foreach (var entry in pesos)
            {
                acumulado += entry.Value;
                if (randomValue < acumulado)
                {
                    return entry.Key;
                }
            }

            return StatusDoProduto.Dropshipping;  // Fallback (segurança)
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                if (context == null || context.Produto == null || context.CategoriaProduto == null || context.Fornecedor == null)
                {
                    throw new ArgumentNullException("Null AppDbContext");
                }

                if (!context.Produto.Any())
                {
                    var random = new Random();

                    var categorias = context.CategoriaProduto.ToList();
                    var fornecedores = context.Fornecedor.ToList();

                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var fornecedor in fornecedores)
                            {
                                int numProdutos = random.Next(5, 21);

                                for (int i = 0; i < numProdutos; i++)
                                {
                                    var produto = new Produto(
                                        GerarCodigoProduto(random),
                                        GerarDescricaoProduto(random, categorias),
                                        GerarNCM(random),
                                        GerarValorUnitario(random),
                                        categorias [random.Next(categorias.Count)].Id,
                                        fornecedor.Id // Associar ao Fornecedor correto
                                    )
                                    {
                                        StatusDoProduto = GetStatusDoProdutoByWeight(random),
                                        Imagem = "img-padrao.jpg"
                                    };

                                    context.Produto.Add(produto);
                                }
                            }

                            context.SaveChanges();
                            transaction.Commit();

                            Console.WriteLine("SeedData para Produto gerado com sucesso!");
                        } catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Erro durante a geração de SeedDataProduto: {ex.Message}");
                        }
                    }
                } else
                {
                    Console.WriteLine("O SeedData para Produto já foi gerado!");
                }
            }
        }
    }
}
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralLabSolutions.InfraStructure.Data.Seeds
{
    public static class SeedDataFornecedor
    {
        // Função para retornar o status do fornecedor baseado em pesos
        public static StatusDoFornecedor GetStatusDoFornecedorByWeight(Random random)
        {
            var pesos = new Dictionary<StatusDoFornecedor, int>
            {
                { StatusDoFornecedor.Ativo, 30 },    // Peso: 30
                { StatusDoFornecedor.Inativo, 5 }    // Peso: 5
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

            return StatusDoFornecedor.Ativo;  // Fallback (segurança)
        }

        // Função para retornar o tipo de pessoa baseado em pesos
        public static TipoDePessoa GetTipoDePessoaByWeight(Random random)
        {
            var pesos = new Dictionary<TipoDePessoa, int>
            {
                { TipoDePessoa.Fisica, 5 },   // Peso: 5
                { TipoDePessoa.Juridica, 25 } // Peso: 25
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

            return TipoDePessoa.Juridica;  // Fallback (segurança)
        }

        // Função para gerar o número de documento baseado no tipo de pessoa
        public static string GerarDocumento(TipoDePessoa tipoPessoa, Random random)
        {
            if (tipoPessoa == TipoDePessoa.Fisica)
            {
                return string.Join("", Enumerable.Range(0, 11).Select(_ => random.Next(0, 10).ToString())); // 11 dígitos para Pessoa Física
            } else
            {
                return string.Join("", Enumerable.Range(0, 14).Select(_ => random.Next(0, 10).ToString())); // 14 dígitos para Pessoa Jurídica
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                if (context == null || context.Fornecedor == null || context.Pessoa == null)
                {
                    throw new ArgumentNullException("Null AppDbContext");
                }

                if (!context.Fornecedor.Any())
                {
                    var random = new Random();

                    var nomesFornecedores = new List<string>
                    {
                        "Eletrônicos Brasil Ltda",
                        "Instrumentos Precisão Ltda",
                        "Construtora Alpha",
                        "Distribuidora de Tecnologia",
                        "Fabricação Mecânica Ltda",
                        "Comércio de Equipamentos Ltda",
                        "Material Elétrico e Hidráulico",
                        "Ferramentas Industriais Ltda",
                        "Tecnologia e Inovação S.A.",
                        "Soluções Automação Ltda",
                        "Produtos Químicos Ltda",
                        "Madeireira Central",
                        "Comércio de Lubrificantes Ltda",
                        "Distribuidora de Peças Ltda",
                        "Equipamentos Pesados S.A.",
                        "Comércio e Serviço PLS Ltda",
                        "Distribuidora de Metal Ltda",
                        "Equipamentos Médicos S.A."
                        // ... (adicione mais nomes de fornecedores, se desejar) ...
                    };

                    foreach (var nomeFornecedor in nomesFornecedores)
                    {
                        var tipoPessoa = GetTipoDePessoaByWeight(random);
                        var documento = GerarDocumento(tipoPessoa, random);
                        var email = $"{nomeFornecedor.Replace(" ", "").ToLower()}@email.com"; // Gerar email a partir do nome

                        var fornecedor = new Fornecedor(
                            nome: nomeFornecedor,
                            documento: documento,
                            tipoDePessoa: tipoPessoa,
                            email: email
                        )
                        {
                            StatusDoFornecedor = GetStatusDoFornecedorByWeight(random),
                            DataInclusao = DateTime.UtcNow.AddDays(-10),
                            UsuarioInclusao = "SeedData - Inclusão",
                            DataUltimaModificacao = DateTime.UtcNow.AddDays(-2),
                            UsuarioUltimaModificacao = "SeedData - Modificação",
                        };

                        context.Fornecedor.Add(fornecedor);
                    }

                    context.SaveChanges();

                    Console.WriteLine("SeedData para Fornecedor gerado com sucesso!");
                } else
                {
                    Console.WriteLine("O SeedData para Fornecedor já foi gerado!");
                }
            }
        }
    }
}
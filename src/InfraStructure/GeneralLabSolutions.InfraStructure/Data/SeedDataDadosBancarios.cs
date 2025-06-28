using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums; // Para TipoDeContaBancaria
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; // Mantido por padrão, embora não usado diretamente
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralLabSolutions.InfraStructure.Data
{
    public static class SeedDataDadosBancarios
    {
        // Helper para gerar tipos de conta com pesos (mantido)
        private static TipoDeContaBancaria GetTipoDeContaBancariaByWeight(Random random)
        {
            var pesos = new Dictionary<TipoDeContaBancaria, int>
            {
                { TipoDeContaBancaria.Corrente, 40 },
                { TipoDeContaBancaria.Poupanca, 20 },
                { TipoDeContaBancaria.ContaSalario, 10 },
                { TipoDeContaBancaria.ContaConjunta, 5 }, // Menos comum como conta principal de uma Pessoa única no seed
                { TipoDeContaBancaria.ContaUniversitaria, 5 },
                { TipoDeContaBancaria.ContaEmpresarial, 15 },
                { TipoDeContaBancaria.Pagamento, 5 }
            };

            int pesoTotal = pesos.Values.Sum();
            int randomValue = random.Next(0, pesoTotal);
            int acumulado = 0;
            foreach (var entry in pesos)
            {
                acumulado += entry.Value;
                if (randomValue < acumulado)
                    return entry.Key;
            }
            return TipoDeContaBancaria.Corrente; // Fallback
        }

        // Helper para gerar nomes de bancos (mantido)
        private static string GetNomeBancoAleatorio(Random random)
        {
            var bancos = new List<string> {
                "Banco do Brasil", "Caixa Econômica Federal", "Itaú Unibanco", "Bradesco",
                "Santander", "Banco Safra", "BTG Pactual", "Sicoob", "Sicredi",
                "Banco Inter", "Nubank", "C6 Bank", "Banco Original", "Banco Pan"
            };
            return bancos [random.Next(bancos.Count)];
        }

        // Método Initialize que recebe o DbContext
        public static void Initialize(AppDbContext context)
        {
            // Verificação padrão para evitar re-seed
            if (context.DadosBancarios.Any())
            {
                Console.WriteLine("SeedData: DadosBancarios já existem no banco de dados.");
                return;
            }

            // --- 1. Obter as Pessoas Existentes ---
            // Buscamos apenas as Pessoas (sem tracking, só precisamos do Id)
            var pessoas = context.Pessoa.AsNoTracking().ToList();

            // Alternativa mais explícita buscando de Cliente/Fornecedor/Vendedor:
            /*
            var pessoas = new List<Pessoa>();
            pessoas.AddRange(context.Cliente.Select(c => c.Pessoa).AsNoTracking());
            pessoas.AddRange(context.Fornecedor.Select(f => f.Pessoa).AsNoTracking());
            pessoas.AddRange(context.Vendedor.Select(v => v.Pessoa).AsNoTracking());
            // Garantir unicidade se uma Pessoa puder ser mais de um tipo (improvável com Id único)
            // pessoas = pessoas.DistinctBy(p => p.Id).ToList();
            */


            if (!pessoas.Any())
            {
                Console.WriteLine("SeedDataDadosBancarios: Nenhuma Pessoa encontrada para associar dados bancários. Verifique se os Seeds de Cliente/Fornecedor/Vendedor foram executados antes.");
                return;
            }

            Console.WriteLine($"SeedDataDadosBancarios: Encontradas {pessoas.Count} pessoas. Gerando dados bancários...");

            var random = new Random();
            var listaDadosBancarios = new List<DadosBancarios>();

            // --- 2. Gerar Dados Bancários PARA CADA Pessoa ---
            // Vamos garantir que cada pessoa tenha pelo menos uma conta e, aleatoriamente, talvez mais.
            foreach (var pessoa in pessoas)
            {
                // Determina quantas contas esta pessoa terá (ex: 1 a 3)
                int numeroDeContasParaPessoa = random.Next(1, 4);

                for (int i = 0; i < numeroDeContasParaPessoa; i++)
                {
                    var banco = GetNomeBancoAleatorio(random);
                    var agencia = random.Next(100, 9999).ToString("D4");
                    var conta = $"{random.Next(1000, 99999)}-{random.Next(0, 9)}";
                    var tipoConta = GetTipoDeContaBancariaByWeight(random);

                    // *** MUDANÇA PRINCIPAL AQUI ***
                    // Criamos a instância de DadosBancarios já associando o PessoaId
                    // usando o construtor que você definiu.
                    var dadosBancarios = new DadosBancarios(
                        banco: banco,
                        agencia: agencia,
                        conta: conta,
                        tipoDeContaBancaria: tipoConta,
                        pessoaId: pessoa.Id // Associa diretamente à Pessoa
                    );

                    // Se preferir usar o construtor vazio e o método Set:
                    /*
                    var dadosBancarios = new DadosBancarios
                    {
                        // Id e CreatedAt/UpdatedAt serão gerados por EntityBase/EF se aplicável
                    };
                    dadosBancarios.SetBanco(banco);
                    dadosBancarios.SetAgencia(agencia);
                    dadosBancarios.SetConta(conta);
                    dadosBancarios.SetTipoDeContaBancaria(tipoConta);
                    dadosBancarios.DefinePessoa(pessoa.Id); // Define a pessoa usando o método
                    */

                    listaDadosBancarios.Add(dadosBancarios);
                }
            }

            // --- 3. Adicionar Dados Bancários Gerados ao Contexto ---
            context.DadosBancarios.AddRange(listaDadosBancarios);
            Console.WriteLine($"SeedDataDadosBancarios: {listaDadosBancarios.Count} instâncias de DadosBancarios (com PessoaId definido) adicionadas ao contexto.");

            // ** NÃO CHAMAR context.SaveChanges() AQUI **
            // A persistência será feita pelo DbInitializer

            Console.WriteLine("SeedData para DadosBancarios (modelo 1:N) preparado para commit.");
        }
    }
}
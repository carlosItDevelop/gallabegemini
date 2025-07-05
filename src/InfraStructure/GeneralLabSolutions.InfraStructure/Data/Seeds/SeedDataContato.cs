using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection; // Não mais necessário aqui
using System;
using System.Collections.Generic; // Para List<>
using System.Linq; // Para Any, Select

namespace GeneralLabSolutions.InfraStructure.Data.Seeds
{
    public static class SeedDataContato
    {
        // Função para retornar o tipo de contato baseado em pesos (mantida)
        private static TipoDeContato GetTipoDeContatoByWeight(Random random)
        {
            var pesos = new Dictionary<TipoDeContato, int>
            {
                { TipoDeContato.Inativo, 5 },
                { TipoDeContato.Comercial, 35 },
                { TipoDeContato.Pessoal, 10 },
                { TipoDeContato.ProspeccaoCliente, 25 },
                { TipoDeContato.ProspeccaoVendedor, 10 },
                { TipoDeContato.ProspeccaoFornecedor, 15 }
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
            return TipoDeContato.Comercial; // Fallback
        }

        // Helper para gerar nomes aleatórios (Exemplo simples)
        private static string GetNomeAleatorio(Random random, int index)
        {
            string [] primeirosNomes = { "Ana", "Bruno", "Carla", "Daniel", "Elisa", "Fábio", "Gisele", "Hugo", "Iris", "João", "Laura", "Marcos" };
            string [] sobrenomes = { "Silva", "Souza", "Oliveira", "Pereira", "Costa", "Rodrigues", "Almeida", "Nunes", "Lima", "Gomes", "Martins", "Rocha" };
            return $"{primeirosNomes [random.Next(primeirosNomes.Length)]} {sobrenomes [random.Next(sobrenomes.Length)]} (Contato {index})";
        }


        // *** ALTERADO: Recebe AppDbContext diretamente ***
        public static void Initialize(AppDbContext context)
        {
            // Verificação para evitar re-seed
            if (context.Contato.Any())
            {
                Console.WriteLine("SeedData: Contato já existem no banco de dados.");
                return;
            }

            // --- 1. Obter as Pessoas Existentes ---
            var pessoas = context.Pessoa.AsNoTracking().ToList();

            if (!pessoas.Any())
            {
                Console.WriteLine("SeedDataContato: Nenhuma Pessoa encontrada para associar contatos. Verifique Seeds de Cliente/Fornecedor/Vendedor.");
                return;
            }

            Console.WriteLine($"SeedDataContato: Encontradas {pessoas.Count} pessoas. Gerando contatos...");

            var random = new Random();
            var listaContatos = new List<Contato>();

            // --- 2. Gerar Contatos PARA CADA Pessoa ---
            // Garante que cada pessoa tenha 1 ou 2 contatos
            int contatoIndex = 1; // Para nome único do contato
            foreach (var pessoa in pessoas)
            {
                int numeroDeContatosParaPessoa = random.Next(1, 3); // 1 ou 2 contatos por pessoa

                for (int i = 0; i < numeroDeContatosParaPessoa; i++)
                {
                    var nome = GetNomeAleatorio(random, contatoIndex++);
                    var email = $"{nome.ToLower().Replace(" ", ".").Replace("(", "").Replace(")", "")}@empresa-contato.com";
                    var telefone = $"({random.Next(11, 99)}) 9{random.Next(1000, 9999)}-{random.Next(1000, 9999)}";
                    var tipoDeContato = GetTipoDeContatoByWeight(random);

                    // *** MUDANÇA PRINCIPAL AQUI ***
                    // Criamos a instância de Contato já associando o PessoaId
                    // usando o construtor que você definiu na entidade Contato.
                    var contato = new Contato(
                        nome: nome,
                        email: email,
                        telefone: telefone,
                        tipoDeContato: tipoDeContato,
                        pessoaId: pessoa.Id // Associa diretamente à Pessoa
                    )
                    {
                        // Propriedades adicionais (string.Empty se não houver)
                        EmailAlternativo = random.Next(0, 3) == 0 ? $"alt.{email}" : string.Empty, // 1 em 3 chance de ter alternativo
                        TelefoneAlternativo = random.Next(0, 3) == 0 ? $"({random.Next(11, 99)}) 8{random.Next(1000, 9999)}-{random.Next(1000, 9999)}" : string.Empty,
                        Observacao = random.Next(0, 2) == 0 ? "Observação gerada automaticamente para este contato." : string.Empty
                    };

                    listaContatos.Add(contato);
                }
            }

            // --- 3. Adicionar Contatos Gerados ao Contexto ---
            // Adicionamos APENAS a lista de Contato.
            context.Contato.AddRange(listaContatos);
            Console.WriteLine($"SeedDataContato: {listaContatos.Count} instâncias de Contato (com PessoaId definido) adicionadas ao contexto.");

            // ** NÃO CHAMAR context.SaveChanges() AQUI **
            // A persistência será feita pelo DbInitializer

            Console.WriteLine("SeedData para Contato (modelo 1:N) preparado para commit.");
        }
    }
}
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums; // Para TipoDeTelefone
using GeneralLabSolutions.InfraStructure.Data.ORM;
using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection; // Não mais necessário aqui
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralLabSolutions.InfraStructure.Data.Seeds
{
    public static class SeedDataTelefone
    {
        // Função para retornar o tipo de telefone baseado em pesos (mantida)
        private static TipoDeTelefone GetTipoDeTelefoneByWeight(Random random)
        {
            var pesos = new Dictionary<TipoDeTelefone, int>
            {
                { TipoDeTelefone.Celular, 40 },     // Aumentei peso Celular
                { TipoDeTelefone.Residencial, 10 },
                { TipoDeTelefone.Comercial, 35 },   // Aumentei peso Comercial
                { TipoDeTelefone.Recado, 10 },
                { TipoDeTelefone.Outro, 5 }
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
            return TipoDeTelefone.Celular; // Fallback
        }

        // *** ALTERADO: Recebe AppDbContext diretamente ***
        public static void Initialize(AppDbContext context)
        {
            // Verificação para evitar re-seed
            if (context.Telefone.Any())
            {
                Console.WriteLine("SeedData: Telefone já existem no banco de dados.");
                return;
            }

            // --- 1. Obter as Pessoas Existentes ---
            // Buscamos apenas as Pessoas (sem tracking, só precisamos do Id)
            var pessoas = context.Pessoa.AsNoTracking().ToList();

            if (!pessoas.Any())
            {
                Console.WriteLine("SeedDataTelefone: Nenhuma Pessoa encontrada para associar telefones. Verifique se os Seeds de Cliente/Fornecedor/Vendedor foram executados antes.");
                return;
            }

            Console.WriteLine($"SeedDataTelefone: Encontradas {pessoas.Count} pessoas. Gerando telefones...");

            var random = new Random();
            var listaTelefones = new List<Telefone>();

            // --- 2. Gerar Telefones PARA CADA Pessoa ---
            // Garante que cada pessoa tenha 1 ou 2 telefones
            foreach (var pessoa in pessoas)
            {
                // Determina quantos telefones esta pessoa terá (ex: 1 ou 2)
                int numeroDeTelefonesParaPessoa = random.Next(1, 3);

                for (int i = 0; i < numeroDeTelefonesParaPessoa; i++)
                {
                    var ddd = random.Next(11, 99).ToString("D2"); // Garante 2 dígitos
                    // Gera número com 9 dígitos (formato celular comum)
                    var numeroParte1 = random.Next(98000, 99999).ToString();
                    var numeroParte2 = random.Next(1000, 9999).ToString();
                    var numero = $"{numeroParte1}-{numeroParte2}"; // Ex: 9xxxx-xxxx
                    var tipoTelefone = GetTipoDeTelefoneByWeight(random);

                    // *** MUDANÇA PRINCIPAL AQUI ***
                    // Criamos a instância de Telefone já associando o PessoaId
                    // usando o novo construtor definido na entidade Telefone.
                    var telefone = new Telefone(
                        ddd: ddd,
                        numero: numero,
                        tipoDeTelefone: tipoTelefone,
                        pessoaId: pessoa.Id // Associa diretamente à Pessoa
                    );

                    listaTelefones.Add(telefone);
                }
            }

            // --- 3. Adicionar Telefones Gerados ao Contexto ---
            context.Telefone.AddRange(listaTelefones);
            Console.WriteLine($"SeedDataTelefone: {listaTelefones.Count} instâncias de Telefone (com PessoaId definido) adicionadas ao contexto.");

            // ** NÃO CHAMAR context.SaveChanges() AQUI **
            // A persistência será feita pelo DbInitializer

            Console.WriteLine("SeedData para Telefone (modelo 1:N) preparado para commit.");
        }
    }
}
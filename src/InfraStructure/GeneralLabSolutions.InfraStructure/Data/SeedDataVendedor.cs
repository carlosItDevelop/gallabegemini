using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralLabSolutions.InfraStructure.Data
{
    public static class SeedDataVendedor
    {
        // Função para retornar o StatusDoVendedor baseado em pesos
        public static StatusDoVendedor GetStatusDoVendedorByWeight(Random random)
        {
            var pesos = new Dictionary<StatusDoVendedor, int>
            {
                { StatusDoVendedor.Admin, 15 },        // Peso 15
                { StatusDoVendedor.Contratado, 10 },   // Peso 10
                { StatusDoVendedor.FreeLance, 20 },    // Peso 20
                { StatusDoVendedor.Inativo, 5 }        // Peso 5
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

            return StatusDoVendedor.Contratado;  // Fallback (segurança)
        }

        // Função para retornar o TipoDePessoa baseado em pesos
        public static TipoDePessoa GetTipoDePessoaByWeight(Random random)
        {
            var pesos = new Dictionary<TipoDePessoa, int>
            {
                { TipoDePessoa.Fisica, 5 },    // Peso 5
                { TipoDePessoa.Juridica, 25 }  // Peso 25
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

            return TipoDePessoa.Fisica;  // Fallback (segurança)
        }

        // Função para gerar o número de documento baseado no tipo de pessoa
        public static string GerarDocumento(TipoDePessoa tipoPessoa, Random random)
        {
            if (tipoPessoa == TipoDePessoa.Fisica)
            {
                return string.Join("", Enumerable.Range(0, 11).Select(_ => random.Next(0, 10).ToString())); // 11 dígitos
            } else
            {
                return string.Join("", Enumerable.Range(0, 14).Select(_ => random.Next(0, 10).ToString())); // 14 dígitos
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                if (context == null || context.Vendedor == null || context.Pessoa == null)
                {
                    throw new ArgumentNullException("Null AppDbContext");
                }

                if (!context.Vendedor.Any())
                {
                    var random = new Random();

                    var vendedoresData = new List<(string Nome, string Email)>
                    {
                        ("Carlos Henrique", "carlos.henrique@empresa.com"),
                        ("Maria Clara", "maria.clara@empresa.com"),
                        ("João Silva", "joao.silva@empresa.com"),
                        ("Luciana Alves", "luciana.alves@empresa.com"),
                        ("Rafael Souza", "rafael.souza@empresa.com"),
                        ("Fernanda Lima", "fernanda.lima@empresa.com"),
                        ("Lucas Martins", "lucas.martins@empresa.com"),
                        ("Patrícia Ramos", "patricia.ramos@empresa.com"),
                        ("Ricardo Mendes", "ricardo.mendes@empresa.com"),
                        ("Juliana Costa", "juliana.costa@empresa.com"),
                        ("Carlos Santanna", "carlos.santanna@empresa.com"),
                        ("Lucas Ferreira", "lucas.ferreira@empresa.com"),
                        ("Renato O Lima", "renato.lima@empresa.com"),
                        ("Ana Furtado", "ana.furtado@empresa.com"),
                        ("Betriz de Souza", "beatriz.souza@empresa.com")
                        // Adicione mais vendedores conforme necessário...
                    };

                    foreach (var (Nome, Email) in vendedoresData)
                    {
                        var tipoPessoa = GetTipoDePessoaByWeight(random);
                        var documento = GerarDocumento(tipoPessoa, random);

                        var vendedor = new Vendedor(
                            nome: Nome,
                            documento: documento,
                            tipoDePessoa: tipoPessoa,
                            email: Email
                        )
                        {
                            StatusDoVendedor = GetStatusDoVendedorByWeight(random)
                        };

                        context.Vendedor.Add(vendedor);
                    }

                    context.SaveChanges();

                    Console.WriteLine("SeedData para Vendedor gerado com sucesso!");
                } else
                {
                    Console.WriteLine("O SeedData para Vendedor já foi gerado!");
                }
            }
        }
    }
}
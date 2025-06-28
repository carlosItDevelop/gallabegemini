using GeneralLabSolutions.Domain.Entities; // Para Endereco e Pessoa
using Microsoft.EntityFrameworkCore;    // Para AsNoTracking
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralLabSolutions.InfraStructure.Data
{
    public static class SeedDataEndereco
    {
        // Helper para gerar dados de endereço aleatórios
        private static class EnderecoHelper
        {
            private static readonly Random _random = new Random();
            private static readonly string [] _paisesIso = { "BR", "US", "PT", "DE", "FR", "GB", "JP", "CA", "AU", "IT" };
            private static readonly string [] _tiposLogradouroBr = { "Rua", "Avenida", "Praça", "Travessa", "Alameda" };
            private static readonly string [] _nomesLogradouroBr = { "das Palmeiras", "dos Girassóis", "Central", "da República", "do Comércio", "Principal", "Nova", "XV de Novembro" };
            private static readonly string [] _cidadesBr = { "São Paulo", "Rio de Janeiro", "Belo Horizonte", "Porto Alegre", "Salvador", "Curitiba", "Fortaleza", "Recife" };
            private static readonly string [] _estadosBr = { "SP", "RJ", "MG", "RS", "BA", "PR", "CE", "PE" };

            private static readonly string [] _streetNamesUs = { "Main St", "Oak Ave", "Pine Ln", "Maple Dr", "Elm Rd", "Washington Blvd", "Park Way", "Sunset Strip" };
            private static readonly string [] _citiesUs = { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego" };
            private static readonly string [] _statesUs = { "NY", "CA", "IL", "TX", "AZ", "PA", "TX", "CA" };


            public static Endereco GerarEnderecoParaPessoa(Guid pessoaId)
            {
                string pais = _paisesIso [_random.Next(_paisesIso.Length)];
                string linha1, cidade, estado, cep;
                Endereco.TipoDeEnderecoEnum tipo = (Endereco.TipoDeEnderecoEnum)_random.Next(1, Enum.GetValues(typeof(Endereco.TipoDeEnderecoEnum)).Length + 1);

                if (pais == "BR")
                {
                    linha1 = $"{_tiposLogradouroBr [_random.Next(_tiposLogradouroBr.Length)]} {_nomesLogradouroBr [_random.Next(_nomesLogradouroBr.Length)]}, {_random.Next(1, 2000)}";
                    cidade = _cidadesBr [_random.Next(_cidadesBr.Length)];
                    estado = _estadosBr [_random.Next(_estadosBr.Length)];
                    cep = $"{_random.Next(10000, 99999)}-{_random.Next(100, 999)}";
                } else if (pais == "US")
                {
                    linha1 = $"{_random.Next(1, 9999)} {_streetNamesUs [_random.Next(_streetNamesUs.Length)]}";
                    cidade = _citiesUs [_random.Next(_citiesUs.Length)];
                    estado = _statesUs [_random.Next(_statesUs.Length)];
                    cep = _random.Next(10000, 99999).ToString();
                } else // Outros países (formato genérico)
                {
                    linha1 = $"Street Address Line 1 {_random.Next(1, 100)}, {_random.Next(1, 200)}";
                    cidade = $"City {_random.Next(1, 50)}";
                    estado = $"State/Province {_random.Next(1, 10)}";
                    cep = _random.Next(1000, 99999).ToString();
                }

                string linha2 = _random.Next(0, 3) == 0 ? $"Apt {_random.Next(1, 100)}" : string.Empty; // 1 em 3 chance de ter linha 2
                string infoAdicional = _random.Next(0, 4) == 0 ? "Additional delivery instructions here." : string.Empty; // 1 em 4 chance

                return new Endereco(
                    pessoaId: pessoaId,
                    paisCodigoIso: pais,
                    linhaEndereco1: linha1,
                    cidade: cidade,
                    codigoPostal: cep,
                    tipoDeEndereco: tipo,
                    linhaEndereco2: linha2,
                    estadoOuProvincia: estado,
                    informacoesAdicionais: infoAdicional
                );
            }
        }

        public static void Initialize(AppDbContext context)
        {
            if (context.Endereco.Any())
            {
                Console.WriteLine("SeedData: Endereco já existem no banco de dados.");
                return;
            }

            var pessoas = context.Pessoa.AsNoTracking().ToList();
            if (!pessoas.Any())
            {
                Console.WriteLine("SeedDataEndereco: Nenhuma Pessoa encontrada para associar endereços.");
                return;
            }

            Console.WriteLine($"SeedDataEndereco: Encontradas {pessoas.Count} pessoas. Gerando endereços...");

            var listaEnderecos = new List<Endereco>();

            foreach (var pessoa in pessoas)
            {
                // Adicionar 1 ou 2 endereços para cada pessoa
                int numeroDeEnderecos = new Random().Next(1, 3);
                for (int i = 0; i < numeroDeEnderecos; i++)
                {
                    listaEnderecos.Add(EnderecoHelper.GerarEnderecoParaPessoa(pessoa.Id));
                }
            }

            context.Endereco.AddRange(listaEnderecos);
            Console.WriteLine($"SeedDataEndereco: {listaEnderecos.Count} instâncias de Endereco adicionadas ao contexto.");

            // NÃO CHAMAR context.SaveChanges() AQUI
            Console.WriteLine("SeedData para Endereco (modelo 1:N) preparado para commit.");
        }
    }
}
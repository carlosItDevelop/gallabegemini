using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralLabSolutions.InfraStructure.Data.Seeds
{
    public static class SeedDataCliente
    {
        // Função para retornar o tipo de cliente baseado em pesos
        public static TipoDeCliente GetTipoDeClienteByWeight(Random random)
        {
            var pesos = new Dictionary<TipoDeCliente, int>
            {
                { TipoDeCliente.Especial, 9 },
                { TipoDeCliente.Comum, 32 },
                { TipoDeCliente.Inadimplente, 4 }
            };
            return GetRandomEnumByWeight(pesos, random, TipoDeCliente.Comum);
        }

        // Função para retornar o status do cliente baseado em pesos (ATUALIZADA)
        public static StatusDoCliente GetStatusDoClienteByWeight(Random random)
        {
            var pesos = new Dictionary<StatusDoCliente, int>
            {
                { StatusDoCliente.Ativo, 30 },
                { StatusDoCliente.Inativo, 8 },
                { StatusDoCliente.Bloqueado, 5 }
            };
            return GetRandomEnumByWeight(pesos, random, StatusDoCliente.Ativo);
        }

        // Função para retornar o tipo de pessoa baseado em pesos
        public static TipoDePessoa GetTipoDePessoaByWeight(Random random)
        {
            var pesos = new Dictionary<TipoDePessoa, int>
            {
                { TipoDePessoa.Fisica, 5 },
                { TipoDePessoa.Juridica, 25 }
            };
            return GetRandomEnumByWeight(pesos, random, TipoDePessoa.Fisica);
        }

        // Função para retornar o regime tributário baseado em pesos
        public static RegimeTributario GetRegimeTributarioByWeight(Random random)
        {
            var pesos = new Dictionary<RegimeTributario, int>
            {
                { RegimeTributario.SimplesNacional, 8 },
                { RegimeTributario.LucroPresumido, 12 },
                { RegimeTributario.LucroReal, 20 },
                { RegimeTributario.Outro, 2 }
            };
            return GetRandomEnumByWeight(pesos, random, RegimeTributario.SimplesNacional);
        }

        // Função genérica para obter um enum aleatório baseado em pesos
        private static TEnum GetRandomEnumByWeight<TEnum>(Dictionary<TEnum, int> weightedEnums, Random random, TEnum fallback) where TEnum : Enum
        {
            int pesoTotal = weightedEnums.Values.Sum();
            if (pesoTotal == 0)
                return fallback; // Evita divisão por zero se todos os pesos forem 0

            int randomValue = random.Next(0, pesoTotal);
            int acumulado = 0;
            foreach (var entry in weightedEnums)
            {
                acumulado += entry.Value;
                if (randomValue < acumulado)
                {
                    return entry.Key;
                }
            }
            return fallback; // Fallback (segurança)
        }


        public static string GerarDocumento(TipoDePessoa tipoPessoa, Random random)
        {
            if (tipoPessoa == TipoDePessoa.Fisica)
            {
                return string.Join("", Enumerable.Range(0, 11).Select(_ => random.Next(0, 10).ToString()));
            } else
            {
                return string.Join("", Enumerable.Range(0, 14).Select(_ => random.Next(0, 10).ToString()));
            }
        }

        public static string GerarEmail(string nomeCliente, Random random)
        {
            var nomeTratado = nomeCliente.ToLower().Replace(" ", ".").Replace(",", "").Replace("Ltda", "").Replace("Inc", "").Replace("SA", "").Replace("Ass", "");
            var dominios = new List<string> { "empresa.com", "cliente.com", "mail.com", "exemplo.com", "negocios.com", "provedor.net" };
            var dominioAleatorio = dominios [random.Next(dominios.Count)];
            return $"{nomeTratado.Split('.').First()}{random.Next(1, 99)}@{dominioAleatorio}"; // Adiciona um número para mais unicidade
        }
        
        public static string GerarTelefonePrincipal(Random random)
        {
            return $"({random.Next(11, 99)}) 9{random.Next(1000, 9999)}-{random.Next(1000, 9999)}";
        }

        // NOVA Função para gerar contato/representante (pode ser null/vazio)
        public static string? GerarContatoRepresentante(string nomeCliente, Random random)
        {
            if (random.Next(0, 3) == 0) // 1 em 3 chance de não ter representante
            {
                return null;
            }
            var nomesRepresentante = new List<string> { "Sr. Silva", "Sra. Oliveira", "Gerente " + nomeCliente.Split(' ') [0], "Contato Vendas", "Financeiro" };
            return nomesRepresentante [random.Next(nomesRepresentante.Count)];
        }


        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                if (context == null || context.Cliente == null || context.Pessoa == null)
                {
                    throw new ArgumentNullException("Null AppDbContext or required DbSets");
                }

                if (context.Cliente.Any())
                {
                    Console.WriteLine("O SeedData para Cliente já foi gerado!");
                    return;
                }

                Console.WriteLine("Gerando SeedData para Cliente...");
                var random = new Random();

                var clientesData = new List<string> // Simplificado para apenas nomes, email será gerado
                {
                    "Ana Paula Silva", "Empresa X Soluções Ltda", "Carlos Eduardo Moreira", "Construtora ABC Obras",
                    "Arnaldo Santiago Filho", "Clarice Borges de Almeida", "ABC Med Diagnósticos Inc", "Start Cor Tintas SA",
                    "Medical Group Assistência Ltda", "Ricardo Amaral Consultoria", "Empório Leonardo & Associados",
                    "Cooperchip CPES Tecnologia Ltda", "Ana Beatriz Souza e Silva", "Lumac Labs Pesquisas SA", "Ana Clara Vasconcelos",
                    "Mariana Costa Fotografia", "Tech Global Inovações Ltda", "Oliveira & Filhos Comércio", "Ferreira Advogados Associados", "Bar e Restaurante Sabor Local"
                };

                foreach (var nomeCliente in clientesData)
                {
                    var tipoPessoa = GetTipoDePessoaByWeight(random);
                    var documento = GerarDocumento(tipoPessoa, random);
                    var email = GerarEmail(nomeCliente, random);

                    var cliente = new Cliente(
                        nome: nomeCliente,
                        documento: documento,
                        tipoDePessoa: tipoPessoa,
                        email: email
                    )
                    {
                        TipoDeCliente = GetTipoDeClienteByWeight(random),
                        StatusDoCliente = GetStatusDoClienteByWeight(random),
                        DataInclusao = DateTime.UtcNow.AddDays(-10),
                        UsuarioInclusao = "SeedData - Inclusão",
                        DataUltimaModificacao = DateTime.UtcNow.AddDays(-2),
                        UsuarioUltimaModificacao = "SeedData - Modificação",
                        InscricaoEstadual = random.Next(0, 2) == 0 ? null : "1234567890", // 50% de chance de ser null
                        Observacao = "Observação padrão para SeedData",
                    };


                    // Usando os métodos Set para as propriedades com setters privados
                    cliente.SetTelefonePrincipal(GerarTelefonePrincipal(random));

                    var contatoRep = GerarContatoRepresentante(nomeCliente, random);
                    if (!string.IsNullOrEmpty(contatoRep))
                    {
                        cliente.SetContatoRepresentante(contatoRep);
                    }
                    // Se ContatoRepresentante for null, ele permanecerá null na entidade, o que é ok.

                    context.Cliente.Add(cliente);
                }

                try
                {
                    context.SaveChanges(); // Mantendo o SaveChanges aqui por enquanto
                    Console.WriteLine("SeedData para Cliente gerado com sucesso!");
                } catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao salvar SeedData de Cliente: {ex.Message}");
                    // Considerar logar ex.ToString() para mais detalhes em caso de falha
                }
            }
        }
    }
}
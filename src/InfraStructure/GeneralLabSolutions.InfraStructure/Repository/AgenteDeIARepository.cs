using GeneralLabSolutions.CoreShared.DTOs.DtosAgenteDeIA;
using GeneralLabSolutions.CoreShared.Interfaces;
using GeneralLabSolutions.InfraStructure.Data;
using Microsoft.EntityFrameworkCore; // Não se esqueça deste using

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class AgenteDeIARepository : IAgenteDeIARepository
    {
        private readonly AppDbContext _context;

        // Constantes para os limites podem ser definidas aqui ou passadas como parâmetros
        private const int DefaultTakeEntidadePrincipal = 10;
        private const int DefaultTakePedidosCliente = 3;
        private const int DefaultTakePedidosVendedor = 3;
        private const int DefaultTakeProdutosFornecedor = 5;

        public AgenteDeIARepository(AppDbContext context)
        {
            _context = context;
        }

        // Método para o contexto de Clientes (substitui ObterClientesComPedidos)
        // Clientes com seus pedidos (DTOs)
        public async Task<List<ClienteIAContextDto>> ObterContextoClientesParaIAAsync(int takeClientes = 10, int takePedidos = 3)
        {
            var clientes = await _context.Cliente
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Take(takeClientes)
                .Select(c => new ClienteIAContextDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Email = c.Email,
                    Documento = c.Documento,
                    Status = c.StatusDoCliente.ToString(),
                    Pedidos = c.Pedidos
                        .OrderByDescending(p => p.DataPedido)
                        .Take(takePedidos)
                        .Select(p => new PedidoResumoIADto
                        {
                            Id = p.Id,
                            Data = p.DataPedido,
                            Status = p.StatusDoPedido.ToString(),
                            Valor = p.Itens.Sum(i => i.Quantidade * i.ValorUnitario)
                        }).ToList() // Pode deixar .ToList() aqui (EF Core 5+)
                })
                .ToListAsync();
            return clientes;
        }

        public async Task<List<FornecedorIAContextDto>> ObterContextoFornecedoresParaIAAsync(int takeFornecedores = 10, int takeProdutos = 5)
        {
            var fornecedores = await _context.Fornecedor
                .AsNoTracking()
                .OrderBy(f => f.Id)
                .Take(takeFornecedores)
                .Select(f => new FornecedorIAContextDto
                {
                    Id = f.Id,
                    Nome = f.Nome,
                    Email = f.Email,
                    Documento = f.Documento,
                    Status = f.StatusDoFornecedor.ToString(),
                    Produtos = f.Produtos
                        .OrderByDescending(p => p.Id)
                        .Take(takeProdutos)
                        .Select(p => new ProdutoResumoIADto
                        {
                            Id = p.Id,
                            Codigo = p.Codigo,
                            NomeProduto = p.Descricao,
                            Categoria = p.CategoriaProduto.Descricao,
                            Status = p.StatusDoProduto.ToString(),
                            ValorUnitario = p.ValorUnitario
                        }).ToList()
                })
                .ToListAsync();
            return fornecedores;
        }


        public async Task<List<VendedorIAContextDto>> ObterContextoVendedoresParaIAAsync(int takeVendedores = 10, int takePedidos = 3)
        {
            var vendedores = await _context.Vendedor
                .AsNoTracking()
                .OrderBy(v => v.Id)
                .Take(takeVendedores)
                .Select(v => new VendedorIAContextDto
                {
                    Id = v.Id,
                    Nome = v.Nome,
                    Email = v.Email,
                    Documento = v.Documento,
                    Status = v.StatusDoVendedor.ToString(),
                    Pedidos = v.Pedidos
                        .OrderByDescending(p => p.DataPedido)
                        .Take(takePedidos)
                        .Select(p => new PedidoResumoIADto
                        {
                            Id = p.Id,
                            Data = p.DataPedido,
                            Status = p.StatusDoPedido.ToString(),
                            Valor = p.Itens.Sum(i => i.Quantidade * i.ValorUnitario)
                        }).ToList()
                })
                .ToListAsync();

            return vendedores;
        }


    }
}
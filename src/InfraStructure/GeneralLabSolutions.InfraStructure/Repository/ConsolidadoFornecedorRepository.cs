using GeneralLabSolutions.CoreShared.DTOs.DtosConsolidados;
using GeneralLabSolutions.CoreShared.Interfaces;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class ConsolidadoFornecedorRepository : GenericRepository<Fornecedor, Guid>, IConsolidadoFornecedorRepositoryDomain, IConsolidadoFornecedorRepository
    {
        private readonly IGenericRepository<Fornecedor, Guid> _queryRepository;

        public ConsolidadoFornecedorRepository(AppDbContext context,
                    IGenericRepository<Fornecedor, Guid> queryRepository) 
                    : base(context)
        {
            _queryRepository = queryRepository;
        }


        public async Task<FornecedorConsolidadoDto?> ObterConsolidadoFornecedorPorIdAsync(Guid fornecedorId)
        {
            // Começando pelos Itens de Pedido relacionados ao Fornecedor
            var itensDoFornecedor = await _context.ItemPedido
                .Include(i => i.Produto)
                    .Include(i => i.Pedido) // <- Incluindo o Pedido aqui!
                .Where(i => i.Produto.FornecedorId == fornecedorId)
                .AsSplitQuery() // Usando AsSplitQuery para evitar problemas de performance
                .ToListAsync();

            if (itensDoFornecedor.Count == 0)
                return null; // Ou retornar um DTO vazio, se preferir

            // Obtendo os produtos do fornecedor a partir dos Itens
            var produtosDoFornecedor = itensDoFornecedor
                .Select(i => i.Produto)
                .Distinct()
                .ToList();

            // Calculando o ValorTotalGerado a partir dos Itens
            var valorTotalGerado = itensDoFornecedor.Sum(i => i.Quantidade * i.ValorUnitario);

            // Obtendo o nome do Fornecedor diretamente do banco de dados
            var fornecedorNome = (await _queryRepository.GetByIdAsync(fornecedorId))?.Nome ?? string.Empty;

            return new FornecedorConsolidadoDto
            {
                FornecedorId = fornecedorId,
                Nome = fornecedorNome,
                TotalDeProdutos = produtosDoFornecedor.Count,
                ValorTotalGerado = valorTotalGerado,
                ProdutosMaisVendidos = ObterProdutosMaisVendidos(fornecedorId),
                CategoriasMaisPopulares = ObterCategoriasMaisPopulares(fornecedorId),
                HistoricoDeVendas = itensDoFornecedor
            .Select(i => i.Pedido)
            .Distinct()
            .Select(p => new VendaHistoricoDto
            {
                Id = p.Id,
                DataVenda = p.DataPedido,
                ValorTotal = p.CalcularValorTotal(),
                Status = p.StatusDoPedido.ToString()
            }).ToList()
            };
        }


        private List<ProdutoHistoricoDto> ObterProdutosMaisVendidos(Guid fornecedorId)
        {
            // Começando pelos Itens de Pedido, filtrados pelo fornecedor
            var itens = _context.ItemPedido
                .Include(i => i.Produto) // Incluindo o Produto relacionado
                    .ThenInclude(p => p.CategoriaProduto) // E a Categoria do Produto
                .Where(i => i.Produto.FornecedorId == fornecedorId)
                .AsSplitQuery() // Usando AsSplitQuery para evitar problemas de performance
                .ToList();

            // Agrupando os itens por Produto
            return itens
                .GroupBy(i => i.Produto)
                .Select(g => new ProdutoHistoricoDto
                {
                    ProdutoId = g.Key.Id,
                    Nome = g.Key.Descricao ?? string.Empty,
                    QuantidadeVendida = g.Sum(i => i.Quantidade),
                    ValorTotalVendido = g.Sum(i => i.Quantidade * i.ValorUnitario)
                })
                .OrderByDescending(dto => dto.QuantidadeVendida)
                .Take(5)
                .ToList();
        }

        private List<CategoriaProdutoDto> ObterCategoriasMaisPopulares(Guid fornecedorId)
        {
            var itens = _context.ItemPedido
                .Include(i => i.Produto)
                    .ThenInclude(p => p.CategoriaProduto)
                .AsSplitQuery()
                .Where(i => i.Produto.FornecedorId == fornecedorId)
                .ToList();

            return itens
                .GroupBy(i => i.Produto.CategoriaProduto)
                .Where(g => g.Key is not null) // Só categorias não-nulas
                .Select(g => new
                {
                    Categoria = g.Key!,
                    QuantidadeTotal = g.Sum(x => x.Quantidade)
                })
                .OrderByDescending(x => x.QuantidadeTotal)
                .Take(5)
                .Select(x => new CategoriaProdutoDto
                {
                    Id = x.Categoria.Id,
                    Nome = x.Categoria.Descricao ?? string.Empty
                })
                .ToList();
        }


        public async Task<IEnumerable<Fornecedor>> ObterTodosFornecedoresAsync()
        {
            return await _queryRepository.GetAllAsync();
        }
    }
}
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Extensions.Helpers.Generics;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class ProdutoRepository : GenericRepository<Produto, Guid>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context){}

        public async Task<IEnumerable<Produto>> GetAllProductWithIncludesAsync()
        {
            var listaProdutos = await _context.Produto
                .Include(x=>x.Fornecedor)
                .Include(x=>x.CategoriaProduto)
                .AsSplitQuery() // Usando AsSplitQuery para evitar problemas de performance
                .AsNoTracking()
                .ToListAsync();

            return await Task.FromResult(listaProdutos);
        }


        public async Task<PagedResult<Produto>> ObterTodosPaginado(int pageIndex, int pageSize, string? query = null)
        {
            var source = _context.Produto.AsQueryable();

            // Aplicar o filtro com base na descrição
            if (!string.IsNullOrWhiteSpace(query))
            {
                source = source.Where(x => x.Descricao != null && x.Descricao.Contains(query));
            }

            // Ordenar por descrição (ou outra propriedade caso necessário)
            source = source.OrderBy(x => x.Descricao);

            // Total de registros encontrados
            var count = await source.CountAsync();

            // Aplicar paginação
            var data = await source.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            // Total de páginas
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PagedResult<Produto>
            {
                List = data,
                TotalResults = count,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                Query = query,
                HasPrevious = pageIndex > 1,
                HasNext = pageIndex < totalPages
            };
        }


    }
}

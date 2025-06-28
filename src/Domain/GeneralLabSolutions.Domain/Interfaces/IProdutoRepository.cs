using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Extensions.Helpers.Generics;
using GeneralLabSolutions.Domain.Interfaces;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IProdutoRepository : IGenericRepository<Produto, Guid>
	{
		Task<IEnumerable<Produto>> GetAllProductWithIncludesAsync();
        Task<PagedResult<Produto>> ObterTodosPaginado(int pageIndex, int pageSize, string? query = null);

    }
}

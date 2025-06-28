using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Extensions.Helpers.Generics;

namespace GeneralLabSolutions.Domain.Interfaces;

public interface ICategoriaRepository : IGenericRepository<CategoriaProduto, Guid>
{
    public Task<bool> TemCategoria(Guid id);
    Task<CategoriaProduto> ObterCategoriaComProdutosEFornecedor(Guid categorId);

}
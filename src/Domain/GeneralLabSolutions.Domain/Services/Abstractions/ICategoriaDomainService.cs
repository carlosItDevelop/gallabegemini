using GeneralLabSolutions.Domain.Entities;

namespace GeneralLabSolutions.Domain.Services.Abstractions
{
    public interface ICategoriaDomainService
    {
        public Task AddCategoriaAsync(CategoriaProduto model);
        public Task UpdateCategoriaAsync(CategoriaProduto model);
        public Task DeleteCategoriaProdutoAsync(CategoriaProduto model);

    }

}

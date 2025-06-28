using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;

namespace GeneralLabSolutions.Domain.Services.Concreted;

public class CategoriaDomainService : BaseService, ICategoriaDomainService
{

    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriaDomainService(INotificador notificador, ICategoriaRepository categoriaRepository) 
        : base(notificador)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task AddCategoriaAsync(CategoriaProduto model)
    {
        await _categoriaRepository.AddAsync(model);
    }


    public async Task UpdateCategoriaAsync(CategoriaProduto model)
    {
        await _categoriaRepository.UpdateAsync(model);
    }

    public async Task DeleteCategoriaProdutoAsync(CategoriaProduto model)
    {
        //regras de negócio
        await _categoriaRepository.DeleteAsync(model);
    }

}
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class CategoriaRepository : GenericRepository<CategoriaProduto, Guid>, ICategoriaRepository
    {


        public CategoriaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> TemCategoria(Guid id)
        {
            return _context.Set<Fornecedor>().Any(x => x.Id == id);
        }

        public async Task<CategoriaProduto> ObterCategoriaComProdutosEFornecedor(Guid categorId)
        {
            var model = await _context.CategoriaProduto
                .Include(p => p.Produtos!)
                    .ThenInclude (f => f.Fornecedor)
                    .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == categorId);

            return model;
        }

    }
}

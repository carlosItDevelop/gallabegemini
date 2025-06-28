using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IVendedorRepository : IGenericRepository<Vendedor, Guid>
    {
    }
}
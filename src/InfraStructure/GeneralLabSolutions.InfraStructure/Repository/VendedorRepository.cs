using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data;
using GeneralLabSolutions.InfraStructure.Repository.Base;

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class VendedorRepository : GenericRepository<Vendedor, Guid>, IVendedorRepository
    {
        public VendedorRepository(AppDbContext context) : base(context)
        {
        }
    }
}
// Em GeneralLabSolutions.Domain.Interfaces

using GeneralLabSolutions.Domain.Entities;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IConsolidadoVendedorRepositoryDomain
    {
        Task<IEnumerable<Vendedor>> GetAllVendedoresAsync();
    }
}

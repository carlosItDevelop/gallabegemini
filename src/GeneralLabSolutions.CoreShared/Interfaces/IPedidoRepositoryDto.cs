using GeneralLabSolutions.CoreShared.DTOs.DtosGraficos;
using GeneralLabSolutions.CoreShared.DTOs.DtosViewComponents;
using GeneralLabSolutions.SharedKernel.Enums;

namespace GeneralLabSolutions.CoreShared.Interfaces
{
    public interface IPedidoRepositoryDto
    {
        Task<PedidoResumoDto> GetQuantidadeEValorTotalPorStatusAsync(StatusDoPedido status);
        Task<List<TopVendedoresDto>> GetTop10VendedoresAsync();

        /* 3º GRÁFICO DAQUI PR BAIXO - Trimestral */

        Task<List<ClientePeriodoDto>> GetTop4ClientesPorPeriodoQuantidadeAsync(int ano, int mesesPorPeriodo);
        Task<List<ClientePeriodoDto>> GetTop4ClientesPorPeriodoValorAsync(int ano, int mesesPorPeriodo);
    }
}

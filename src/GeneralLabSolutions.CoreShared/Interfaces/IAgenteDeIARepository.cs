using GeneralLabSolutions.CoreShared.DTOs.DtosAgenteDeIA;

namespace GeneralLabSolutions.CoreShared.Interfaces
{
    public interface IAgenteDeIARepository
    {
        Task<List<ClienteIAContextDto>> ObterContextoClientesParaIAAsync(int takeClientes = 10, int takePedidos = 3);
        Task<List<VendedorIAContextDto>> ObterContextoVendedoresParaIAAsync(int takeVendedores = 10, int takePedidos = 3);
        Task<List<FornecedorIAContextDto>> ObterContextoFornecedoresParaIAAsync(int takeFornecedores = 10, int takeProdutos = 5);

    }
}

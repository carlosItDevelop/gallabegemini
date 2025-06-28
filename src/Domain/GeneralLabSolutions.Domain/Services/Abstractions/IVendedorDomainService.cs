using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Services.Abstractions
{
    public interface IVendedorDomainService
    {
        Task<Vendedor> AdicionarVendedor(Vendedor vendedor);
        Task<Vendedor> AtualizarVendedor(Vendedor vendedor);
        Task ExcluirVendedor(Guid id);
        Task<Vendedor?> ObterVendedorPorId(Guid id);
        Task<IEnumerable<Vendedor>> ObterTodosVendedores();

        // Métodos para gerenciar agregados
        Task AdicionarDadosBancarios(Guid vendedorId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta);
        Task AtualizarDadosBancarios(Guid vendedorId, Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta);
        Task RemoverDadosBancarios(Guid vendedorId, Guid dadosBancariosId);

        Task AdicionarTelefone(Guid vendedorId, string ddd, string numero, TipoDeTelefone tipoTelefone);
        Task AtualizarTelefone(Guid vendedorId, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone);
        Task RemoverTelefone(Guid vendedorId, Guid telefoneId);

        Task AdicionarContato(Guid vendedorId, string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlt = "", string telAlt = "", string obs = "");
        Task AtualizarContato(Guid vendedorId, Guid contatoId, string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlt = "", string telAlt = "", string obs = "");
        Task RemoverContato(Guid vendedorId, Guid contatoId);

        Task AdicionarEndereco(Guid vendedorId, string pais, string linha1, string cidade, string cep, Endereco.TipoDeEnderecoEnum tipo, string? linha2, string? estado, string? info);
        Task AtualizarEndereco(Guid vendedorId, Guid enderecoId, string paisCodigoIso, string linhaEndereco1, string cidade, string codigoPostal, Endereco.TipoDeEnderecoEnum tipoDeEndereco, string? linhaEndereco2 = null, string? estadoOuProvincia = null, string? informacoesAdicionais = null);
        Task RemoverEndereco(Guid vendedorId, Guid enderecoId);
    }
}
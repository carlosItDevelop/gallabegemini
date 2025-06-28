using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Services.Abstractions
{
    public interface IFornecedorDomainService
    {
        // Operações no agregado Fornecedor
        Task AddFornecedorAsync(Fornecedor fornecedor);
        Task UpdateFornecedorAsync(Fornecedor fornecedor);
        Task DeleteFornecedorAsync(Fornecedor fornecedor);

        // Operações em entidades filhas do agregado

        // --- DADOS BANCÁRIOS ---
        Task AdicionarDadosBancariosAsync(Guid fornecedorId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta);
        Task AtualizarDadosBancariosAsync(Guid fornecedorId, Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta);
        Task RemoverDadosBancariosAsync(Guid fornecedorId, Guid dadosBancariosId);

        // --- TELEFONES ---
        Task AdicionarTelefoneAsync(Guid fornecedorId, string ddd, string numero, TipoDeTelefone tipoTelefone);
        Task AtualizarTelefoneAsync(Guid fornecedorId, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone);
        Task RemoverTelefoneAsync(Guid fornecedorId, Guid telefoneId);

        // --- CONTATOS ---
        Task AdicionarContatoAsync(Guid fornecedorId, string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlternativo = "", string telefoneAlternativo = "", string observacao = "");
        Task AtualizarContatoAsync(Guid fornecedorId, Guid contatoId, string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlternativo = "", string telefoneAlternativo = "", string observacao = "");
        Task RemoverContatoAsync(Guid fornecedorId, Guid contatoId);

        // --- ENDEREÇOS ---
        Task AdicionarEnderecoAsync(Guid fornecedorId, string paisCodigoIso, string linhaEndereco1, string cidade, string codigoPostal, Endereco.TipoDeEnderecoEnum tipoDeEndereco, string? linhaEndereco2 = null, string? estadoOuProvincia = null, string? informacoesAdicionais = null);
        Task AtualizarEnderecoAsync(Guid fornecedorId, Guid enderecoId, string paisCodigoIso, string linhaEndereco1, string cidade, string codigoPostal, Endereco.TipoDeEnderecoEnum tipoDeEndereco, string? linhaEndereco2 = null, string? estadoOuProvincia = null, string? informacoesAdicionais = null);
        Task RemoverEnderecoAsync(Guid fornecedorId, Guid enderecoId);
    }
}
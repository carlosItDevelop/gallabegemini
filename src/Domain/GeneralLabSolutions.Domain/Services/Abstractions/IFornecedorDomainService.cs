// Local do Arquivo: GeneralLabSolutions.Domain/Services/Interfaces/IFornecedorDomainService.cs

using GeneralLabSolutions.Domain.Entities;

namespace GeneralLabSolutions.Domain.Services.Interfaces
{
    public interface IFornecedorDomainService
    {
        // Operações no agregado Fornecedor
        Task AddFornecedorAsync(Fornecedor fornecedor);
        Task UpdateFornecedorAsync(Fornecedor fornecedor);
        Task DeleteFornecedorAsync(Fornecedor fornecedor);

        // Operações em entidades filhas do agregado

        // --- DADOS BANCÁRIOS ---
        Task AdicionarDadosBancarios(Guid fornecedorId, DadosBancarios dadosBancarios);
        Task AtualizarDadosBancarios(Guid fornecedorId, DadosBancarios dadosBancarios);
        Task RemoverDadosBancarios(Guid fornecedorId, Guid dadosBancariosId);

        // --- TELEFONES ---
        Task AdicionarTelefone(Guid fornecedorId, Telefone telefone);
        Task AtualizarTelefone(Guid fornecedorId, Telefone telefone);
        Task RemoverTelefone(Guid fornecedorId, Guid telefoneId);

        // --- CONTATOS ---
        Task AdicionarContato(Guid fornecedorId, Contato contato);
        Task AtualizarContato(Guid fornecedorId, Contato contato);
        Task RemoverContato(Guid fornecedorId, Guid contatoId);

        // --- ENDEREÇOS ---
        Task AdicionarEndereco(Guid fornecedorId, Endereco endereco);
        Task AtualizarEndereco(Guid fornecedorId, Endereco endereco);
        Task RemoverEndereco(Guid fornecedorId, Guid enderecoId);
    }
}
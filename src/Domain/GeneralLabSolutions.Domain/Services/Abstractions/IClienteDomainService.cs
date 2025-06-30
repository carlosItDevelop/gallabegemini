using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums; // Para TipoDeContaBancaria e TipoDeTelefone
using System;
using System.Threading.Tasks;

namespace GeneralLabSolutions.Domain.Services.Abstractions
{
    public interface IClienteDomainService
    {
        // --- Métodos para Cliente (Existentes) ---
        Task AddClienteAsync(Cliente model);
        Task UpdateClienteAsync(Cliente model);
        Task DeleteClienteAsync(Cliente model);

        #region: Métodos para Dados Bancários (Existentes)

        Task AdicionarDadosBancariosAsync(Guid clienteId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta);
        Task AtualizarDadosBancariosAsync(Guid clienteId, Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta);
        Task RemoverDadosBancariosAsync(Guid clienteId, Guid dadosBancariosId);

        #endregion

        #region: NOVAS ASSINATURAS PARA TELEFONE

        Task AdicionarTelefoneAsync(Guid clienteId, string ddd, string numero, TipoDeTelefone tipoTelefone);
        Task AtualizarTelefoneAsync(Guid clienteId, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone);
        Task RemoverTelefoneAsync(Guid clienteId, Guid telefoneId);
        
        #endregion


        #region: NOVAS ASSINATURAS PARA CONTATO
        Task AdicionarContatoAsync(
            Guid clienteId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "");

        Task AtualizarContatoAsync(
            Guid clienteId,
            Guid contatoId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "");

        Task RemoverContatoAsync(Guid clienteId, Guid contatoId);

        #endregion


        #region Assinaturas Endereço
        Task AdicionarEnderecoAsync(
            Guid clienteId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco, // Usa o enum interno de Endereco
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null);

        Task AtualizarEnderecoAsync(
            Guid clienteId,
            Guid enderecoId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null);

        Task RemoverEnderecoAsync(Guid clienteId, Guid enderecoId);
        #endregion


    }
}
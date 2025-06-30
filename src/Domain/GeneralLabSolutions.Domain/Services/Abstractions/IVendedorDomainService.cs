using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Services.Abstractions
{
    public interface IVendedorDomainService
    {

        // ---- Operações principais ------------------------------------
        Task AdicionarVendedor(Vendedor model);
        Task AtualizarVendedor(Vendedor model);
        Task ExcluirVendedor(Guid id);

        // ---- Consultas ----------------------------------------------
        Task<Vendedor?> ObterVendedorPorId(Guid id);
        Task<IEnumerable<Vendedor>> ObterTodosVendedores();

        // ---- Dados Bancários ----------------------------------------
        Task AdicionarDadosBancarios(
            Guid vendedorId,
            string banco,
            string agencia,
            string conta,
            TipoDeContaBancaria tipoConta);

        Task AtualizarDadosBancariosAsync(
            Guid vendedorId,
            Guid dadosBancariosId,
            string banco,
            string agencia,
            string conta,
            TipoDeContaBancaria tipoConta);

        Task RemoverDadosBancariosAsync(
            Guid vendedorId,
            Guid dadosBancariosId);

        // ---- Telefones ----------------------------------------------
        Task AdicionarTelefoneAsync(
            Guid vendedorId,
            string ddd,
            string numero,
            TipoDeTelefone tipoTelefone);

        Task AtualizarTelefoneAsync(
            Guid vendedorId,
            Guid telefoneId,
            string ddd,
            string numero,
            TipoDeTelefone tipoTelefone);

        Task RemoverTelefoneAsync(
            Guid vendedorId,
            Guid telefoneId);

        // ---- Contatos -----------------------------------------------
        Task AdicionarContatoAsync(
            Guid vendedorId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "");

        Task AtualizarContatoAsync(
            Guid vendedorId,
            Guid contatoId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "");

        Task RemoverContatoAsync(
            Guid vendedorId,
            Guid contatoId);

        // ---- Endereços ----------------------------------------------
        Task AdicionarEnderecoAsync(
            Guid vendedorId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null);

        Task AtualizarEnderecoAsync(
            Guid vendedorId,
            Guid enderecoId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null);

        Task RemoverEnderecoAsync(
            Guid vendedorId,
            Guid enderecoId);
    }
}

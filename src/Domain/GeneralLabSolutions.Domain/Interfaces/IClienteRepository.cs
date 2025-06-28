using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Extensions.Helpers.Generics; // Se usar PagedResult
using System;
using System.Collections.Generic; // Para List<>
using System.Linq.Expressions; // Para Expression<>
using System.Threading.Tasks;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IClienteRepository : IGenericRepository<Cliente, Guid>
    {
        Task<bool> TemCliente(Guid id);
        Task<PagedResult<Cliente>> ObterTodosPaginado(int pageIndex, int pageSize, string? query = null);
        Task<IEnumerable<Cliente>> Buscar(Expression<Func<Cliente, bool>> predicate); // Adicionei este se ainda não existir

        Task<Cliente?> ObterClienteComDadosBancarios(Guid clienteId);
        // Método geral
        Task<Cliente?> ObterClienteCompleto(Guid clienteId); // Este será atualizado para incluir Contatos

        Task<List<DadosBancarios>> ObterDadosBancariosPorClienteId(Guid pessoaId); // Mantido
        Task<DadosBancarios?> ObterDadosBancariosPorId(Guid dadosBancariosId); // Mantido

        // Métodos explícitos para estado (DadosBancarios)
        Task AdicionarDadosBancariosAsync(Cliente cliente, DadosBancarios novo); // Mantido
        Task RemoverDadosBancariosAsync(Cliente cliente, DadosBancarios dadosBancarios); // Mantido


        #region: NOVAS ASSINATURAS PARA TELEFONE 
        Task<Cliente?> ObterClienteComTelefones(Guid clienteId);
        Task AdicionarTelefoneAsync(Cliente cliente, Telefone novo);
        Task RemoverTelefoneAsync(Cliente cliente, Telefone telefone);

        // Poderia adicionar ObterTelefonesPorClienteId(Guid pessoaId) e ObterTelefonePorId(Guid telefoneId) se necessário fora do agregado

        #endregion


        #region: NOVAS ASSINATURAS PARA CONTATO
        Task<Cliente?> ObterClienteComContatos(Guid clienteId);
        Task AdicionarContatoAsync(Cliente cliente, Contato novo);
        Task RemoverContatoAsync(Cliente cliente, Contato contato);
        // Task<List<Contato>> ObterContatosPorClienteId(Guid pessoaId); // Opcional, se precisar buscar fora do agregado
        // Task<Contato?> ObterContatoPorId(Guid contatoId); // Opcional, se precisar buscar fora do agregado

        #endregion


        #region Assinaturas Endereço
        Task<Cliente?> ObterClienteComEnderecos(Guid clienteId);
        Task AdicionarEnderecoAsync(Cliente cliente, Endereco novo);
        Task RemoverEnderecoAsync(Cliente cliente, Endereco endereco);
        // Task<List<Endereco>> ObterEnderecosPorClienteId(Guid pessoaId); // Opcional
        // Task<Endereco?> ObterEnderecoPorId(Guid enderecoId); // Opcional
        #endregion

    }
}
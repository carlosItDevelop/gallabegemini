using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Extensions.Helpers.Generics;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IClienteRepository : IGenericRepository<Cliente, Guid>
    {
        Task<bool> TemCliente(Guid id);
        Task<PagedResult<Cliente>> ObterTodosPaginado(int pageIndex, int pageSize, string? query = null);
        Task<IEnumerable<Cliente>> Buscar(Expression<Func<Cliente, bool>> predicate);
        Task<Cliente?> ObterClienteCompleto(Guid clienteId);


        #region: NOVAS ASSINATURAS PARA DADOS BANCÁRIOS 
        Task<Cliente?> ObterClienteComDadosBancarios(Guid clienteId);
        Task<List<DadosBancarios>> ObterDadosBancariosPorClienteId(Guid pessoaId);
        Task<DadosBancarios?> ObterDadosBancariosPorId(Guid dadosBancariosId);

        Task AdicionarDadosBancariosAsync(Cliente cliente, DadosBancarios novo);
        Task RemoverDadosBancariosAsync(Cliente cliente, DadosBancarios dadosBancarios);

        #endregion


        #region: NOVAS ASSINATURAS PARA TELEFONE 
        
        Task<Cliente?> ObterClienteComTelefones(Guid clienteId);
        Task<List<Telefone>> ObterTelefonesPorClienteId(Guid pessoaId);
        Task<Telefone?> ObterTelefonePorId(Guid telefoneId); // se necessário fora do agregado

        Task AdicionarTelefoneAsync(Cliente cliente, Telefone novo);
        Task RemoverTelefoneAsync(Cliente cliente, Telefone telefone);

        #endregion


        #region: NOVAS ASSINATURAS PARA CONTATO

        Task<Cliente?> ObterClienteComContatos(Guid clienteId);
        Task<List<Contato>> ObterContatosPorClienteId(Guid pessoaId); // se precisar buscar fora do agregado
        Task<Contato?> ObterContatoPorId(Guid contatoId); // se precisar buscar fora do agregado

        Task AdicionarContatoAsync(Cliente cliente, Contato novo);
        Task RemoverContatoAsync(Cliente cliente, Contato contato);
        
        #endregion


        #region Assinaturas Endereço
        Task<Cliente?> ObterClienteComEnderecos(Guid clienteId);
        Task<List<Endereco>> ObterEnderecosPorClienteId(Guid pessoaId);
        Task<Endereco?> ObterEnderecoPorId(Guid enderecoId);

        Task AdicionarEnderecoAsync(Cliente cliente, Endereco novo);
        Task RemoverEnderecoAsync(Cliente cliente, Endereco endereco);

        #endregion

    }
}
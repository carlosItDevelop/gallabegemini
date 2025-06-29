using System.Linq.Expressions;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Extensions.Helpers.Generics;
using GeneralLabSolutions.Domain.Interfaces;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IVendedorRepository : IGenericRepository<Vendedor, Guid>
    {
        Task<bool> TemVendedor(Guid id);
        Task<PagedResult<Vendedor>> ObterTodosPaginado(int pageIndex, int pageSize, string? query = null);
        Task<IEnumerable<Vendedor>> Buscar(Expression<Func<Vendedor, bool>> predicate);


        Task<Vendedor?> ObterVendedorComDadosBancarios(Guid vendedorId);
        // M�todo geral
        Task<Vendedor?> ObterVendedorCompleto(Guid vendedorId);

        #region: M�todos para Dados Banc�rios (Existentes)

        Task<List<DadosBancarios>> ObterDadosBancariosPorVendedorId(Guid pessoaId); // Mantido
        Task<DadosBancarios?> ObterDadosBancariosPorId(Guid dadosBancariosId); // Mantido

        // M�todos expl�citos para estado (DadosBancarios)        
        Task AdicionarDadosBancariosAsync(Vendedor vendedor, DadosBancarios novo);
        Task RemoverDadosBancariosAsync(Vendedor vendedor, DadosBancarios dadosBancarios);

        #endregion

        #region: NOVAS ASSINATURAS PARA TELEFONE 
        Task<Vendedor?> ObterVendedorComTelefones(Guid vendedorId);
        Task AdicionarTelefoneAsync(Vendedor vendedor, Telefone novo);
        Task RemoverTelefoneAsync(Vendedor forncedor, Telefone telefone);

        // Poderia adicionar ObterTelefonesPorVendedorId(Guid pessoaId) e ObterTelefonePorId(Guid telefoneId) se necess�rio fora do agregado

        #endregion

        #region: NOVAS ASSINATURAS PARA CONTATO
        Task<Vendedor?> ObterVendedorComContatos(Guid vendedorId);
        Task AdicionarContatoAsync(Vendedor vendedor, Contato novo);
        Task RemoverContatoAsync(Vendedor vendedor, Contato contato);

        #endregion

        #region: NOVAS ASSINATURAS PARA ENDERE�O
        Task<Vendedor?> ObterVendedorComEnderecos(Guid vendedorId);
        Task AdicionarEnderecoAsync(Vendedor vendedor, Endereco novo);
        Task RemoverEnderecoAsync(Vendedor vendedor, Endereco endereco);

        #endregion


    }
}
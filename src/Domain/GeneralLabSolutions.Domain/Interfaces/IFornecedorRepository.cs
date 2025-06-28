// Local do Arquivo: GeneralLabSolutions.Domain/Interfaces/IFornecedorRepository.cs

using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Extensions.Helpers.Generics;
using System.Linq.Expressions;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IFornecedorRepository : IGenericRepository<Fornecedor, Guid>
    {
        // Métodos de consulta existentes
        Task<PagedResult<Fornecedor>> ObterTodosPaginado(int pageIndex, int pageSize, string? query = null);
        Task<IEnumerable<Fornecedor>> Buscar(Expression<Func<Fornecedor, bool>> predicate);

        // --- INÍCIO DAS NOVAS ASSINATURAS ---

        // Método principal para carregar o agregado completo
        Task<Fornecedor?> ObterFornecedorCompleto(Guid fornecedorId);

        // Métodos para manipulação explícita do estado das entidades filhas
        Task AdicionarDadosBancariosAsync(Fornecedor fornecedor, DadosBancarios novo);
        Task RemoverDadosBancariosAsync(Fornecedor fornecedor, DadosBancarios dadosBancarios);

        Task AdicionarTelefoneAsync(Fornecedor fornecedor, Telefone novo);
        Task RemoverTelefoneAsync(Fornecedor fornecedor, Telefone telefone);

        Task AdicionarContatoAsync(Fornecedor fornecedor, Contato novo);
        Task RemoverContatoAsync(Fornecedor fornecedor, Contato contato);

        Task AdicionarEnderecoAsync(Fornecedor fornecedor, Endereco novo);
        Task RemoverEnderecoAsync(Fornecedor fornecedor, Endereco endereco);
    }
}
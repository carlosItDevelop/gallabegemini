using System.Linq.Expressions;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Extensions.Helpers.Generics;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class ClienteRepository : GenericRepository<Cliente, Guid>, IClienteRepository
    {
        // Propriedade pública para acesso ao Context (para debug)
        public AppDbContext Context => _context;

        public ClienteRepository(AppDbContext context) : base(context) { }


        #region: Métodos para busca em Cliente
        public async Task<bool> TemCliente(Guid id)
        {
            return await _context.Cliente.AnyAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Cliente>> Buscar(Expression<Func<Cliente, bool>> predicate)
        {
            return await _context.Cliente.AsNoTracking().Where(predicate).ToListAsync();
        }

        #endregion: Fim Métodos para busca em Cliente


        #region: ObterTodosPaginado

        public async Task<PagedResult<Cliente>> ObterTodosPaginado(int pageIndex, int pageSize, string? query = null)
        {
            IEnumerable<Cliente> data = new List<Cliente>();
            var source = _context.Cliente.AsQueryable();

            data = query != null
                ? await source.Where(x => x.Nome.Contains(query)).OrderBy(x => x.Nome).ToListAsync()
                : await source.OrderBy(x => x.Nome).ToListAsync();

            var count = data.Count();
            data = data.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PagedResult<Cliente>()
            {
                List = data,
                TotalPages = totalPages,
                TotalResults = count,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Query = query,
                HasPrevious = pageIndex > 1,
                HasNext = pageIndex < totalPages
            };

        }

        #endregion: Fim ObterTodosPaginado


        #region: Métodos para Cliente Completo

        /// <summary>
        /// Obtém o cliente completo com todas as suas coleções de Pessoa.
        /// </summary>
        public async Task<Cliente?> ObterClienteCompleto(Guid clienteId)
        {
            return await _context.Cliente
                                 .Include(c => c.Pessoa)
                                     .ThenInclude(p => p.DadosBancarios)
                                 .Include(c => c.Pessoa)
                                     .ThenInclude(p => p.Telefones)
                                 .Include(c => c.Pessoa) // Inclui Pessoa novamente para encadear
                                     .ThenInclude(p => p.Contatos)
                                 .Include(c => c.Pessoa) // Inclui Pessoa novamente para encadear
                                     .ThenInclude(p => p.Enderecos)
                                 .Include(c => c.Pedidos) // Se necessário
                                     .ThenInclude(p => p.Itens) // Se necessário
                                 .AsSplitQuery()
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == clienteId);
        }

        #endregion: Fim Métodos Cliente Completo


        #region: Métodos para Dados Bancários (Mantidos)

        // ToDo: Mudar ObterDadosBancariosPorClienteId para ObterDadosBancariosPorPessoaId.
        public async Task<List<DadosBancarios>> ObterDadosBancariosPorClienteId(Guid pessoaId)
        {
            return await _context.DadosBancarios
                .Where(x => x.PessoaId == pessoaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Cliente?> ObterClienteComDadosBancarios(Guid clienteId)
        {
            return await _context.Cliente
                                 .Include(c => c.Pessoa)
                                     .ThenInclude(p => p.DadosBancarios)
                                     .AsSplitQuery()
                                     .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == clienteId);
        }

        public async Task<DadosBancarios?> ObterDadosBancariosPorId(Guid dadosBancariosId)
        {
            // Sem AsNoTracking aqui, pois pode ser para edição
            return await _context.DadosBancarios
                .FirstOrDefaultAsync(x => x.Id == dadosBancariosId);
        }


        public async Task AdicionarDadosBancariosAsync(Cliente cliente, DadosBancarios novo)
        {
            _context.Cliente.Attach(cliente); // Garante rastreamento do pai
            _context.Entry(novo).State = EntityState.Added; // Define estado explícito
            await Task.CompletedTask;
        }

        public async Task RemoverDadosBancariosAsync(Cliente cliente, DadosBancarios dadosBancarios)
        {
            _context.Cliente.Attach(cliente); // Garante rastreamento do pai
            _context.Entry(dadosBancarios).State = EntityState.Deleted; // Define estado explícito
            await Task.CompletedTask;
        }
        #endregion: Fim Métodos Dados Bancários


        #region: NOVAS IMPLEMENTAÇÕES PARA TELEFONE

        /// <summary>
        /// Obtém um cliente incluindo seus dados de Pessoa e a coleção de Telefones.
        /// </summary>
        public async Task<Cliente?> ObterClienteComTelefones(Guid clienteId)
        {
            return await _context.Cliente
                                 .Include(c => c.Pessoa)
                                     .ThenInclude(p => p.Telefones) // Inclui a nova coleção
                                     .AsSplitQuery()
                                 .FirstOrDefaultAsync(c => c.Id == clienteId);
        }


        public async Task<List<Telefone>> ObterTelefonesPorClienteId(Guid pessoaId)
        {
            return await _context.Telefone
                .Where(x => x.PessoaId == pessoaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Telefone?> ObterTelefonePorId(Guid telefoneId)
        {
            // Se necessário fora do agregado, mas geralmente não é recomendado
            return await _context.Telefone
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == telefoneId);
        }


        /// <summary>
        /// Define explicitamente o estado de um novo Telefone como 'Added'.
        /// </summary>
        public async Task AdicionarTelefoneAsync(Cliente cliente, Telefone novo)
        {
            // Garantir que o Cliente (pai) esteja sendo rastreado
            _context.Cliente.Attach(cliente);
            // Definir o estado do novo telefone como Adicionado
            _context.Entry(novo).State = EntityState.Added;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Define explicitamente o estado de um Telefone como 'Deleted'.
        /// </summary>
        public async Task RemoverTelefoneAsync(Cliente cliente, Telefone telefone)
        {
            // Garantir que o Cliente (pai) esteja sendo rastreado
            _context.Cliente.Attach(cliente);
            // Definir o estado do telefone como Excluído
            _context.Entry(telefone).State = EntityState.Deleted;
            await Task.CompletedTask;
        }

        #endregion: FIM NOVAS IMPLEMENTAÇÕES PARA TELEFONE


        #region Métodos Contatos

        /// <summary>
        /// Obtém um cliente incluindo seus dados de Pessoa e a coleção de Contatos.
        /// </summary>
        public async Task<Cliente?> ObterClienteComContatos(Guid clienteId)
        {
            return await _context.Cliente
                                 .Include(c => c.Pessoa)
                                     .ThenInclude(p => p.Contatos) // Inclui a nova coleção
                                     .AsSplitQuery()
                                 .FirstOrDefaultAsync(c => c.Id == clienteId);
        }

        public async Task<List<Contato>> ObterContatosPorClienteId(Guid pessoaId)
        {
            return await _context.Contato
                .Where(x => x.PessoaId == pessoaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Contato?> ObterContatoPorId(Guid contatoId)
        {
            // Se necessário fora do agregado, mas geralmente não é recomendado
            return await _context.Contato
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == contatoId);
        }

        /// <summary>
        /// Define explicitamente o estado de um novo Contato como 'Added'.
        /// </summary>
        public async Task AdicionarContatoAsync(Cliente cliente, Contato novo)
        {
            _context.Cliente.Attach(cliente); // Garante que o pai esteja rastreado
            _context.Entry(novo).State = EntityState.Added; // Define o estado como Adicionado
            await Task.CompletedTask;
        }

        /// <summary>
        /// Define explicitamente o estado de um Contato como 'Deleted'.
        /// </summary>
        public async Task RemoverContatoAsync(Cliente cliente, Contato contato)
        {
            _context.Cliente.Attach(cliente); // Garante que o pai esteja rastreado
            _context.Entry(contato).State = EntityState.Deleted; // Define o estado como Excluído
            await Task.CompletedTask;
        }
        #endregion


        #region Métodos Endereços

        public async Task<List<Endereco>> ObterEnderecosPorClienteId(Guid pessoaId)
        {
            return await _context.Endereco
                .Where(x => x.PessoaId == pessoaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Endereco?> ObterEnderecoPorId(Guid enderecoId)
        {
            // Se necessário fora do agregado, mas geralmente não é recomendado
            return await _context.Endereco
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == enderecoId);
        }

        /// <summary>
        /// Obtém um cliente incluindo seus dados de Pessoa e a coleção de Endereços.
        /// </summary>
        public async Task<Cliente?> ObterClienteComEnderecos(Guid clienteId) // Ou ObterClienteCompleto
        {
            var clienteComEnderecos = await _context.Cliente
                                         .Include(c => c.Pessoa)
                                             .ThenInclude(p => p.Enderecos)
                                             .AsSplitQuery()
                                         .FirstOrDefaultAsync(c => c.Id == clienteId);
            // <<< COLOQUE O BREAKPOINT AQUI, APÓS A LINHA ACIMA ^^^ >>>
            return clienteComEnderecos;
        }

        /// <summary>
        /// Define explicitamente o estado de um novo Endereço como 'Added'.
        /// </summary>
        public async Task AdicionarEnderecoAsync(Cliente cliente, Endereco novo)
        {
            _context.Cliente.Attach(cliente);
            _context.Entry(novo).State = EntityState.Added;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Define explicitamente o estado de um Endereço como 'Deleted'.
        /// </summary>
        public async Task RemoverEnderecoAsync(Cliente cliente, Endereco endereco)
        {
            _context.Cliente.Attach(cliente);
            _context.Entry(endereco).State = EntityState.Deleted;
            await Task.CompletedTask;
        }

        #endregion


    }
}

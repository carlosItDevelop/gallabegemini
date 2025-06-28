using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Extensions.Helpers.Generics;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using Microsoft.EntityFrameworkCore; // Necessário
using System;                       // Necessário
using System.Collections.Generic;   // Necessário para List<>
using System.Linq;                  // Necessário para Any, Where, etc.
using System.Linq.Expressions;      // Necessário para Expression<>
using System.Threading.Tasks;       // Necessário

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class ClienteRepository : GenericRepository<Cliente, Guid>, IClienteRepository
    {
        // Propriedade pública para acesso ao Context (para debug)
        public AppDbContext Context => _context;

        public ClienteRepository(AppDbContext context) : base(context) { }

        public async Task<bool> TemCliente(Guid id)
        {
            return await _context.Cliente.AnyAsync(x => x.Id == id);
        }

        // Exemplo de implementação para Buscar
        public async Task<IEnumerable<Cliente>> Buscar(Expression<Func<Cliente, bool>> predicate)
        {
            return await _context.Cliente.AsNoTracking().Where(predicate).ToListAsync();
        }

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

        #region: Métodos para Dados Bancários (Mantidos)
        public async Task<Cliente?> ObterClienteComDadosBancarios(Guid clienteId)
        {
            return await _context.Cliente
                                 .Include(c => c.Pessoa)
                                     .ThenInclude(p => p.DadosBancarios)
                                     .AsSplitQuery()
                                 .FirstOrDefaultAsync(c => c.Id == clienteId);
        }

        public async Task<List<DadosBancarios>> ObterDadosBancariosPorClienteId(Guid pessoaId)
        {
            return await _context.DadosBancarios
                .Where(x => x.PessoaId == pessoaId)
                .AsNoTracking() // Boa prática para listas de leitura
                .ToListAsync();
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


        #region: ObterClienteCompleto

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
                                 .Include(c => c.Pessoa)
                                     .ThenInclude(p => p.Contatos)
                                 .Include(c => c.Pessoa) // Inclui Pessoa novamente para encadear
                                     .ThenInclude(p => p.Enderecos) // *** ADICIONADO INCLUSÃO DE ENDEREÇOS ***
                                 .Include(c => c.Pedidos) // Se necessário
                                 .AsSplitQuery()
                                 .FirstOrDefaultAsync(c => c.Id == clienteId);
        }

        #endregion


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

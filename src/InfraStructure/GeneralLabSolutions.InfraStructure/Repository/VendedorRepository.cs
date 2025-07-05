using System.Linq.Expressions;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Extensions.Helpers.Generics;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class VendedorRepository : GenericRepository<Vendedor, Guid>, IVendedorRepository
    {
        // Propriedade pública para acessar o contexto do banco de dados (debug)
        public AppDbContext Context => _context;
        public VendedorRepository(AppDbContext context) : base(context) { }


        #region: Métodos de Consulta

        public async Task<bool> TemVendedor(Guid id)
        {
            return await _context.Set<Vendedor>().AsNoTracking().AnyAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Vendedor>> Buscar(Expression<Func<Vendedor, bool>> predicate)
        {
            return await _context.Set<Vendedor>()
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<PagedResult<Vendedor>> ObterTodosPaginado(int pageIndex, int pageSize, string? query = null)
        {
            IEnumerable<Vendedor> data = new List<Vendedor>();
            var source = _context.Vendedor.AsQueryable();

            data = query != null
                ? await source.Where(x => x.Nome.Contains(query)).OrderBy(x => x.Nome).ToListAsync()
                : await source.OrderBy(x => x.Nome).ToListAsync();

            var count = data.Count();
            data = data.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PagedResult<Vendedor>()
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

        #endregion


        #region: NOVAS IMPLEMENTAÇÕES PARA DADOS BANCÁRIOS


        public async Task<Vendedor?> ObterVendedorComDadosBancarios(Guid vendedorId)
        {
            return await _context.Vendedor
                .Include(v => v.Pessoa)
                    .ThenInclude(v => v.DadosBancarios)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(v => v.Id == vendedorId);
        }


        public async Task<List<DadosBancarios>> ObterDadosBancariosPorVendedorId(Guid pessoaId)
        {
            return await _context.DadosBancarios
                .AsNoTracking() // Boa prática para listas de leitura
                .Where(d => d.PessoaId == pessoaId)
                .ToListAsync();
        }

        public async Task<DadosBancarios?> ObterDadosBancariosPorId(Guid dadosBancariosId)
        {
            return await _context.DadosBancarios
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == dadosBancariosId);
        }

        public async Task AdicionarDadosBancariosAsync(Vendedor vendedor, DadosBancarios novo)
        {
            _context.Vendedor.Attach(vendedor); // Garantir rastreamento do pai
            _context.Entry(novo).State = EntityState.Added; // Marcar como novo
            await Task.CompletedTask; // Simular operação assíncrona
        }


        public async Task RemoverDadosBancariosAsync(Vendedor vendedor, DadosBancarios dadosBancarios)
        {
            _context.Vendedor.Attach(vendedor); // Garantir rastreamento do pai
            _context.Entry(dadosBancarios).State = EntityState.Deleted; // Marcar como removido
            await Task.CompletedTask; // Simular operação assíncrona
        }

        #endregion


        #region: NOVAS IMPLEMENTAÇÕES PARA TELEFONE

        public async Task<Vendedor?> ObterVendedorComTelefones(Guid vendedorId)
        {
            return await _context.Vendedor
                .Include(v => v.Pessoa)
                    .ThenInclude(p => p.Telefones)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(v => v.Id == vendedorId);
        }


        public async Task AdicionarTelefoneAsync(Vendedor vendedor, Telefone novo)
        {
            _context.Vendedor.Attach(vendedor); // Garantir rastreamento do pai
            _context.Entry(novo).State = EntityState.Added; // Marcar como novo
            await Task.CompletedTask; // Simular operação assíncrona
        }


        public async Task RemoverTelefoneAsync(Vendedor forncedor, Telefone telefone)
        {
            _context.Vendedor.Attach(forncedor); // Garantir rastreamento do pai
            _context.Entry(telefone).State = EntityState.Deleted; // Marcar como removido
            await Task.CompletedTask; // Simular operação assíncrona
        }

        #endregion


        #region: NOVAS IMPLEMENTAÇÕES PARA CONTATO

        public async Task<Vendedor?> ObterVendedorComContatos(Guid vendedorId)
        {
            var vendedorComContato = await _context.Vendedor
                .Include(v => v.Pessoa)
                    .ThenInclude(p => p.Contatos)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(v => v.Id == vendedorId);

            return vendedorComContato;
        }

        public async Task AdicionarContatoAsync(Vendedor vendedor, Contato novo)
        {
            _context.Vendedor.Attach(vendedor); // Garantir rastreamento do pai
            _context.Entry(novo).State = EntityState.Added; // Marcar como novo
            await Task.CompletedTask; // Simular operação assíncrona
        }

        public async Task RemoverContatoAsync(Vendedor vendedor, Contato contato)
        {
            _context.Vendedor.Attach(vendedor); // Garantir rastreamento do pai
            _context.Entry(contato).State = EntityState.Deleted; // Marcar como removido
            await Task.CompletedTask; // Simular operação assíncrona
        }

        #endregion


        #region: NOVAS IMPLEMENTAÇÕES PARA ENDEREÇO

        public async Task<Vendedor?> ObterVendedorComEnderecos(Guid vendedorId)
        {
            var vendedorComEnderecos = await _context.Vendedor
                .Include(v => v.Pessoa)
                    .ThenInclude(p => p.Enderecos)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(v => v.Id == vendedorId);

            return vendedorComEnderecos;
        }

        /// <summary>
        /// Define explicitamente o estado do endereço como adicionado.
        /// </summary>
        /// <param name="vendedor"></param>
        /// <param name="novo"></param>
        /// <returns></returns>
        public async Task AdicionarEnderecoAsync(Vendedor vendedor, Endereco novo)
        {
            _context.Vendedor.Attach(vendedor); // Garantir rastreamento do pai
            _context.Entry(novo).State = EntityState.Added; // Marcar como novo
            await Task.CompletedTask; // Simular operação assíncrona
        }

        /// <summary>
        /// Define explicitamente o estado do endereço como removido.
        /// </summary>
        /// <param name="vendedor"></param>
        /// <param name="endereco"></param>
        /// <returns></returns>
        public async Task RemoverEnderecoAsync(Vendedor vendedor, Endereco endereco)
        {
            _context.Vendedor.Attach(vendedor); // Garantir rastreamento do pai
            _context.Entry(endereco).State = EntityState.Deleted;
            await Task.CompletedTask;
        }


        #endregion


        #region: TRAZER VENDEDOR COMPLETO
        public async Task<Vendedor?> ObterVendedorCompleto(Guid vendedorId)
        {
            return await _context.Vendedor
                .Include(v => v.Pessoa)
                    .ThenInclude(p => p.Telefones)
                .Include(v => v.Pessoa)
                    .ThenInclude(p => p.Contatos)
                .Include(v => v.Pessoa)
                    .ThenInclude(p => p.Enderecos)
                .Include(v => v.Pessoa)
                    .ThenInclude(p => p.DadosBancarios)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(v => v.Id == vendedorId);
        }

        #endregion

    }
}
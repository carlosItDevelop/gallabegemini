using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Extensions.Helpers.Generics;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class FornecedorRepository : GenericRepository<Fornecedor, Guid>, IFornecedorRepository
    {
        // Propriedade pública para acesso ao Context (para debug)
        public AppDbContext Context => _context;

        public FornecedorRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Fornecedor>> Buscar(Expression<Func<Fornecedor, bool>> predicate)
        {
            return await _context.Fornecedor.AsNoTracking().Where(predicate).ToListAsync();
        }


        public async Task<PagedResult<Fornecedor>> ObterTodosPaginado(int pageIndex, int pageSize, string? query = null)
        {
            IEnumerable<Fornecedor> data = new List<Fornecedor>();
            var source = _context.Fornecedor.AsQueryable();

            data = query != null
                ? await source.Where(x => x.Nome.Contains(query)).OrderBy(x => x.Nome).ToListAsync()
                : await source.OrderBy(x => x.Nome).ToListAsync();

            var count = data.Count();
            data = data.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PagedResult<Fornecedor>()
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


        // ================================================================
        // INÍCIO DA IMPLEMENTAÇÃO DOS NOVOS MÉTODOS
        // ================================================================

        #region Métodos para Dados Bancários
        public async Task<Fornecedor?> ObterFornecedorCompleto(Guid fornecedorId)
        {
            return await _context.Fornecedor
                .Include(f => f.Pessoa)
                    .ThenInclude(p => p.DadosBancarios)
                .Include(f => f.Pessoa)
                    .ThenInclude(p => p.Telefones)
                .Include(f => f.Pessoa)
                    .ThenInclude(p => p.Contatos)
                .Include(f => f.Pessoa)
                    .ThenInclude(p => p.Enderecos)
                .Include(f => f.Produtos) // Se precisar dos produtos também
                .AsSplitQuery()
                .FirstOrDefaultAsync(f => f.Id == fornecedorId);
        }


        // ToDo: Mudar ObterDadosBancariosPorFornecedorId para ObterDadosBancariosPorPessoaId.
        public async Task<List<DadosBancarios>> ObterDadosBancariosPorFornecedorId(Guid pessoaId)
        {
            return await _context.DadosBancarios
                .Where(x => x.PessoaId == pessoaId)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<Fornecedor?> ObterFornecedorComDadosBancarios(Guid FornecedorId)
        {
            return await _context.Fornecedor
                .Include(f => f.Pessoa)
                    .ThenInclude(p => p.DadosBancarios)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(f => f.Id == FornecedorId);
        }


        public async Task<DadosBancarios?> ObterDadosBancariosPorId(Guid dadosBancariosId)
        {
            // Sem AsNoTracking aqui, pois pode ser para edição
            return await _context.DadosBancarios
                .FirstOrDefaultAsync(x => x.Id == dadosBancariosId);
        }


        public async Task AdicionarDadosBancariosAsync(Fornecedor fornecedor, DadosBancarios novo)
        {
            _context.Fornecedor.Attach(fornecedor);
            _context.Entry(novo).State = EntityState.Added;
            await Task.CompletedTask;
        }

        public async Task RemoverDadosBancariosAsync(Fornecedor fornecedor, DadosBancarios dadosBancarios)
        {
            _context.Fornecedor.Attach(fornecedor);
            _context.Entry(dadosBancarios).State = EntityState.Deleted;
            await Task.CompletedTask;
        }
        #endregion

        #region Métodos para Telefones
        public async Task AdicionarTelefoneAsync(Fornecedor fornecedor, Telefone novo)
        {
            _context.Fornecedor.Attach(fornecedor);
            _context.Entry(novo).State = EntityState.Added;
            await Task.CompletedTask;
        }

        public async Task RemoverTelefoneAsync(Fornecedor fornecedor, Telefone telefone)
        {
            _context.Fornecedor.Attach(fornecedor);
            _context.Entry(telefone).State = EntityState.Deleted;
            await Task.CompletedTask;
        }
        #endregion

        #region Métodos para Contatos
        public async Task AdicionarContatoAsync(Fornecedor fornecedor, Contato novo)
        {
            _context.Fornecedor.Attach(fornecedor);
            _context.Entry(novo).State = EntityState.Added;
            await Task.CompletedTask;
        }

        public async Task RemoverContatoAsync(Fornecedor fornecedor, Contato contato)
        {
            _context.Fornecedor.Attach(fornecedor);
            _context.Entry(contato).State = EntityState.Deleted;
            await Task.CompletedTask;
        }
        #endregion


        #region Métodos para Endereços
        public async Task AdicionarEnderecoAsync(Fornecedor fornecedor, Endereco novo)
        {
            _context.Fornecedor.Attach(fornecedor);
            _context.Entry(novo).State = EntityState.Added;
            await Task.CompletedTask;
        }

        public async Task RemoverEnderecoAsync(Fornecedor fornecedor, Endereco endereco)
        {
            _context.Fornecedor.Attach(fornecedor);
            _context.Entry(endereco).State = EntityState.Deleted;
            await Task.CompletedTask;
        }

        #endregion

    }
}

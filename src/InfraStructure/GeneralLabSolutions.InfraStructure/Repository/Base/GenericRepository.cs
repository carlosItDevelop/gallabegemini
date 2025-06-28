using System.Linq.Expressions;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository.Base
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class, new()
    {
        protected readonly AppDbContext _context;

        /// <summary>
        /// Controlador, onde injeto o contexto do banco de dados.
        /// </summary>
        /// <param name="context"></param>
		public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;


        #region: Métodos de Consulta

        public async Task<T?> GetByIdAsync(TKey id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
            {
                return await _context.Set<T>().AsNoTracking().ToListAsync();
            }
            return await _context.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().AnyAsync(predicate);
        }

        #endregion

        #region: Métodos de Ação

        public async Task AddAsync(T obj)
        {
            _context.Set<T>().Add(obj);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(T obj)
        {
            _context.Set<T>().Remove(obj);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(T obj)
        {
            _context.Set<T>().Update(obj);
            await Task.CompletedTask;
        }

        #endregion


        public void Dispose()
        {
            _context?.Dispose();
        }

    }
}

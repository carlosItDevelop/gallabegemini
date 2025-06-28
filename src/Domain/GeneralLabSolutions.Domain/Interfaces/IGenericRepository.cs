using System.Linq.Expressions;
using GeneralLabSolutions.Domain.Interfaces;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IGenericRepository<T, TKey> : IDisposable where T : class
    {

        IUnitOfWork UnitOfWork { get; }

        #region: Métodos de Consulta
        Task<T> GetByIdAsync(TKey id);

        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        #endregion

        #region: Métodos de Ação

        Task AddAsync(T obj);
        Task DeleteAsync(T obj);
        Task UpdateAsync(T obj);

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        Task<TEntity> GetByIDAsync(object id, string includeProperties = "");

        Task<IEnumerable<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> include = null);

        Task InsertAsync(TEntity entity);

        Task InsertRangeAsync(IEnumerable<TEntity> entities);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(object id);

        Task DeleteAsync(TEntity entity);
        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null);

    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Core.Repositories
{
    public interface IRepository<T> where T:class
    {
        Task<T> GetByIdAsync(int Id);

        Task<IEnumerable<T>> GetAllAsync();

        Task <IEnumerable<T>> Where(Expression<Func<T, bool>> predicate);   //Expression x=> x.id

        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);  // category.SingleOrDefaultAsync(x=>x.name='test');

        Task AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

        T Update(T entity);


    }
}

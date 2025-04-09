using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryEntity.IRepository
{
    public interface IRepository <T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        T Get(Expression<Func<T,bool>> filter, string? includeProperties = null,bool tracked = false);
        void Add(T entity);

        //___ Update Senerio is some time different so we handle update in different Repo ____
        //void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}

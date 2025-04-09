using WebdeskEA.DataAccess;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebdeskEA.Domain.RepositoryEntity
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly WebdeskEADBContext _db;
        internal DbSet<T> dbSet;
        public Repository(WebdeskEADBContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
            //_db.Categories == (dbSet)
        }
        public void Add(T entity)
        {
            //____ _db.T.Save();    --> _db don't knwo  what is  T
            dbSet.Add(entity);
        }
        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null,bool tracked =false)
        {
            IQueryable<T> query;
            if (tracked)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }
            //query.First(filter);
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault(filter);
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries).Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }
        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }

    }
}

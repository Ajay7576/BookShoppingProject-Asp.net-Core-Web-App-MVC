using BookShoppingProject_DataAccess.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject_DataAccess.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbset;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            dbset = _context.Set<T>();
        }
        public void Add(T entity)
        {
            dbset.Add(entity);
        }

        public T FirstorDefault(Expression<Func<T, bool>> filter = null, string IncludeProperties = null)
        {
            IQueryable<T> query = dbset;                   // querry bnane k liye or particular record uthane k liye

            if (filter != null)                              // filtration k liye
                query = query.Where(filter);

            if(IncludeProperties!=null)                      // Multiple table join 
            {
                foreach (var IncludeProp in IncludeProperties.Split(new char[] { ','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(IncludeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public T Get(int id)
        {
            return dbset.Find(id);              // find kre gaa pr ek he table m
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy = null, string IncludeProperties = null)
        {
            IQueryable<T> query = dbset;         // querry bnane k liye

            if (filter != null)                     // filtration krne k liye
                query = query.Where(filter);

            if(IncludeProperties!=null)                  // Multiple table join 
            {
                foreach (var IncludeProp in IncludeProperties.Split(new char[] { ','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(IncludeProp);
                }
            }
            if (OrderBy != null)                                 //  sorting k liye
                return OrderBy(query).ToList();
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbset.Remove(entity);             // single record delete krne k liye
        }

        public void Remove(int id)
        {
            var entity = Get(id);           // id sey delete k liye
            Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbset.RemoveRange(entity);         // mutiple record delete krne k liye
        }
    }
}

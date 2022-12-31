using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject_DataAccess.Data.Repository.IRepository
{
    public interface IRepository<T> where T:class
    {
        T Get(int id);            //Find k liye
        IEnumerable<T> GetAll(                         // display k liye
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy = null,
            string IncludeProperties = null
            );
        T FirstorDefault(                                    // Multiple record m find krne k liye
            Expression<Func<T, bool>> filter = null,
            string IncludeProperties = null
            );
        void Add(T entity);                                         // add k liye
        void Remove(T entity);                                  // single record k liye
        void Remove (int id);                                     // id sey delete krne k liye 
        void RemoveRange(IEnumerable<T> entity);                 // Multiple record delete krne k liye 

    }
}

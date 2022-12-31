using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject_DataAccess.Data.Repository.IRepository
{
   public interface ISP_Call:IDisposable
    {
        T Single<T>(string procedureName, DynamicParameters param = null);       //find
        T OneRecord<T>(string procedureName, DynamicParameters param = null);      //multiple m sey ek record
        void Execute(string procedureName, DynamicParameters param = null);       // save delete update
        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null);    // single table display
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null);  //multiple table display
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetOrdersFromOpencart.Model
{
    public interface IRepository
    {
        Task<IEnumerable<Good>> ReturnListGoodsAsync(int orderId);
        Task<Order> GetOrder(int id, string configuration=null);
       IEnumerable<Good> GetListGoodsBySearch(string name);
    }
}

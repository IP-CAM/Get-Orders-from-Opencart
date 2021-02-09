using GetOrdersFromOpencart.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GetOrdersFromOpencart.Components
{
    public class GetGoods : ViewComponent
    {
        private IRepository repository;

        public GetGoods(IRepository repo)
        {
            repository = repo;
        }
        public async Task<IViewComponentResult> InvokeAsync(int orderId)
        {
            IEnumerable<Good> listGoogs = new List<Good>();
            //запрос к базе данных на получение списка товаров по ID заказа
            listGoogs = await repository.ReturnListGoodsAsync(orderId);

            //возврат полученного списка товаров в представление компонента
            return View(listGoogs);
        }
    }
}

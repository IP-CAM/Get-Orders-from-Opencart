using GetOrdersFromOpencart.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetOrdersFromOpencart.Controllers
{
    public class OrderController : Controller
    {
        IRepository conn;
        Connect connecta;

        public OrderController(IRepository connection, Connect connect)
        {
            conn = connection;
            connecta = connect;
        }

        [Authorize]
        public ViewResult ListOrder()
        {
            var res = connecta.GetOrders().OrderByDescending(x => x.Order.Date_added).ToList();
            return View(res);
        }

        [Authorize]
        public ViewResult OrderDetail(int id)
        {
            var res = conn.GetOrder(id).Result;
            return View(res);
        }


        [Authorize]
        [HttpPost]
        public IActionResult OrderDetail(Order orderec)
        {
            if (ModelState.GetValidationState(nameof(orderec.Telephone)) != ModelValidationState.Valid)
            {
                ModelState.AddModelError("", "Не введен номер телефона");
                var res = conn.GetOrder(orderec.Order_id).Result;
                return View(nameof(OrderDetail), res);
            }
            else
            {

                //первый шаг - обновляю в базе данные
                


                //второй шаг - запрашиваю в базе данные и возвращаю на страницу(в другом экшине)
                var res = conn.GetOrder(orderec.Order_id).Result;

                return RedirectToAction(nameof(OrderDetail), res.Order_id);
            }
        }

        public JsonResult ValidatePhoneNumber(string Telephone)
        {
            if (Telephone.Length < 9 || Telephone.Length > 13)
            {
                return Json("В номере телефона содержится менее 9 или более 13 символов. Где-то ошибка");
            }
            else if (Telephone.Contains(' ', StringComparison.OrdinalIgnoreCase))
            {
                return Json("В номере телефона содержатся пробелы. Убери, пожалуйста");
            }
            else if (Telephone.Contains('-', StringComparison.OrdinalIgnoreCase) || Telephone.Contains('+', StringComparison.OrdinalIgnoreCase) || Telephone.Contains('(', StringComparison.OrdinalIgnoreCase) || Telephone.Contains(')', StringComparison.OrdinalIgnoreCase) || Telephone.Contains(',', StringComparison.OrdinalIgnoreCase) || Telephone.Contains('.', StringComparison.OrdinalIgnoreCase))
            {
                return Json("В номере телефона содержатся символы << -, (, ), +, . >>. Убери их, пожалуйста");
            }
            else
            {
                return Json(true);
            }
        }

    }
}

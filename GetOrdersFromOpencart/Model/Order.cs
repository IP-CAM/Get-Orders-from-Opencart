using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GetOrdersFromOpencart.Controllers;
using System.ComponentModel.DataAnnotations.Schema;

namespace GetOrdersFromOpencart.Model
{

    public class Order
    {
        public int Order_id { get; set; }
        [Display(Name ="Имя")]
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        [Remote("ValidatePhoneNumber", nameof(Order))]
        [Required(ErrorMessage = "Номер телефона должен быть обязательно")]
        public string Telephone { get; set; }
        public string Shipping_city { get; set; }
        public string Shipping_address_1 { get; set; }
        public string Comment { get; set; }


        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        public DateTime Date_added { get; set; }

        public List<GoodInOrder> ListGoods { get; set; } = new List<GoodInOrder>();

    }


    public class GoodInOrder
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int Product_id { get; set; }
        public Good Product { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal Total { get; set; }
        public string ForWho { get; set; }
        public string Large { get; set; }

    }

    public class OrderWithGoods
    {
        public Order Order { get; set; }
        public List<GoodInOrder> GoodsInOrder { get; set; }
    }
}

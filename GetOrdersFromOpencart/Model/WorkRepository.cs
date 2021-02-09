using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetOrdersFromOpencart.Model
{
    public class WorkRepository : IRepository
    {
        public IConfiguration Configuration { get; }
        public MySqlConnection Connection { get; private set; }
        private string Config { get; set; }
        public WorkRepository(IConfiguration config)
        {
            Configuration = config;
        }
        MySqlConnection Connect(string configuration = null)
        {
            if (configuration == null)
            {
                Config = "Database=" + Configuration["BaseConnect:Database"] + ";Datasource=" + Configuration["BaseConnect:Host"] + ";User=" + Configuration["BaseConnect:User"] + ";Password=" + Configuration["BaseConnect:Password"];
            }
            else
            {
                Config = configuration;
            }
            Connection = new MySqlConnection(Config);
            return Connection;
        }


        //получение данных по заказу с данными о товаре, покупателе, опциям товара
        public async Task<Order> GetOrder(int id, string configuration = null)
        {
            Order orderWithGoods = new Order();

            if (configuration == null)
            {
                configuration = Config;
            }
            using (var conn1 = Connect(configuration))
            {
                conn1.Open();
                string sql1 = $"SELECT order_id, firstname, lastname, telephone, shipping_city, shipping_address_1, comment, date_added from oc_order WHERE order_id={id};";
                MySqlCommand cmd1 = new MySqlCommand(sql1, conn1);
                using (MySqlDataReader mysql_result1 = cmd1.ExecuteReader())
                {
                    await mysql_result1.ReadAsync();

                    orderWithGoods.Order_id = (int)mysql_result1["order_id"];
                    orderWithGoods.Date_added = (DateTime)mysql_result1["date_added"];
                    orderWithGoods.Firstname = (string)mysql_result1["firstname"];
                    orderWithGoods.Lastname = (string)mysql_result1["lastname"];
                    orderWithGoods.Telephone = (string)mysql_result1["telephone"];
                    orderWithGoods.Shipping_city = (string)mysql_result1["shipping_city"];
                    orderWithGoods.Shipping_address_1 = (string)mysql_result1["shipping_address_1"];
                    orderWithGoods.Comment = (string)mysql_result1["comment"];
                }



                using (var conn2 = Connect(configuration))
                {
                    conn2.Open();
                    string sql2 = $"SELECT product_id, name, model, quantity, price, total, order_id from oc_order_product WHERE order_id={id};";
                    MySqlCommand cmd2 = new MySqlCommand(sql2, conn2);
                    using (MySqlDataReader mysql_result2 = cmd2.ExecuteReader())
                    {
                        while (mysql_result2.Read())
                        {
                            string forWho = string.Empty;
                            string large = string.Empty;


                            using (MySqlConnection conn3 = Connect(configuration))
                            {
                                conn3.Open();
                                string sql3 = $"SELECT * from oc_order_option WHERE order_id={id};";
                                MySqlCommand cmd3 = new MySqlCommand(sql3, conn3);
                                using (MySqlDataReader mysql_result3 = cmd3.ExecuteReader())
                                {
                                    while (mysql_result3.Read())
                                    {
                                        if ((string)mysql_result3["name"] == "Для кого футболка")
                                        {
                                            forWho = (string)mysql_result3["value"];
                                        }
                                        else if ((string)mysql_result3["name"] == "Размер, евро")
                                        {
                                            large = (string)mysql_result3["value"];
                                        }
                                    }
                                }
                                conn3.Close();
                            }

                            GoodInOrder good = new GoodInOrder
                            {
                                Model = (string)mysql_result2["model"],
                                Name = (string)mysql_result2["name"],
                                Quantity = (int)mysql_result2["quantity"],
                                Price = (decimal)mysql_result2["price"],
                                Total = (decimal)mysql_result2["price"] * (int)mysql_result2["quantity"],
                                ForWho = forWho,
                                Large = large
                            };
                            orderWithGoods.ListGoods.Add(good);
                        }
                    }
                    conn2.Close();
                }
                conn1.Close();
            }
            return orderWithGoods;
        }

        public async Task<IEnumerable<Good>> ReturnListGoodsAsync(int orderId)
        {
            List<Good> goods = new List<Good>();

            using (var myConnection = Connect())
            {
                myConnection.Open();
                string sql2 = $"SELECT product_id, name, model, quantity, price, total, order_id from oc_order_product WHERE order_id={orderId};";
                MySqlCommand cmd2 = new MySqlCommand(sql2, myConnection);
                using (MySqlDataReader mysql_result2 = cmd2.ExecuteReader())
                {
                    while (mysql_result2.Read())
                    {
                        string forWho = string.Empty;
                        string large = string.Empty;


                        using (MySqlConnection conn3 = Connect())
                        {
                            conn3.Open();
                            string sql3 = $"SELECT * from oc_order_option WHERE order_id={orderId};";
                            MySqlCommand cmd3 = new MySqlCommand(sql3, conn3);
                            using (MySqlDataReader mysql_result3 = cmd3.ExecuteReader())
                            {
                                while (mysql_result3.Read())
                                {
                                    if ((string)mysql_result3["name"] == "Для кого футболка")
                                    {
                                        forWho = (string)mysql_result3["value"];
                                    }
                                    else if ((string)mysql_result3["name"] == "Размер, евро")
                                    {
                                        large = (string)mysql_result3["value"];
                                    }
                                }
                            }
                        }

                        Good good = new Good
                        {
                            Articul = (string)mysql_result2["model"],
                            Name = (string)mysql_result2["name"],
                            Quantity = (int)mysql_result2["quantity"],
                            Price = (decimal)mysql_result2["price"],
                            ForWho = forWho,
                            Large = large
                        };
                        goods.Add(good);
                    }
                }
            }
            return await Task.Run(() => goods);
        }

        //получение списка товаров на основании поиска в базе
        public IEnumerable<Good> GetListGoodsBySearch(string name)
        {
            return new List<Good> { new Good { Name = "Товар #1", Articul = "SA0000" } };
        }

        public void UpdateOrder(Order order)
        {
            using (var conn = Connection)
            {
                conn.Open();
#warning тут написать скрипт обновления данных в заказе
                string sql = $"UPDATE oc_order_product SET name = {order.Firstname}";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (MySqlDataReader mysql_result = cmd.ExecuteReader())
                {

                }




                conn.Close();
            }
        }
    }
}

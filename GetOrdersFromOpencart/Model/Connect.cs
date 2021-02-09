using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GetOrdersFromOpencart.Model
{
    public class Connect
    {
        public IConfiguration Configuration { get; }
        private string Config { get; set; }

        private string dataBase;
        private string host;
        private string user;
        private string password;

        public Connect(IConfiguration config)
        {
            Configuration = config;
        }
        public Connect(string config)
        {
            Config = config;
        }


        internal MySqlConnection GetConnection()
        {
            MySqlConnection myConnection = default;
            if (!String.IsNullOrEmpty(Config))
            {
                myConnection = new MySqlConnection(Config);
            }
            else
            {
                Config = "Database=" + Configuration["BaseConnect:Database"] + ";Datasource=" + Configuration["BaseConnect:Host"] + ";User=" + Configuration["BaseConnect:User"] + ";Password=" + Configuration["BaseConnect:Password"];
                myConnection = new MySqlConnection(Config);
            }


            if (myConnection == null)
            {
                Console.WriteLine("Соединение не установлено");
            }
            return myConnection;
        }

         


        public List<OrderWithGoods> GetOrders(string connstring = null)
        {
       
            Dictionary<int, Order> listorder = GetListOrder(connstring); //получил список заказов
            List<OrderWithGoods> list = GetFinishModel(listorder); //получил конечную модель 

            return list;
        }

        public List<OrderWithGoods> GetFinishModel(Dictionary<int, Order> listorder)
        {
            List<OrderWithGoods> listFinish = new List<OrderWithGoods>();
            foreach (var orderId in listorder.Keys)
            {
                    List<GoodInOrder> listGoodInOrder = GetListGoodInOrder(orderId);
                    listFinish.Add(new OrderWithGoods { Order = listorder[orderId], GoodsInOrder = listGoodInOrder });

            }
            return listFinish;
        }

        public List<GoodInOrder> GetListGoodInOrder(int orderId, string connstring = null)
        {
            List<GoodInOrder> listGoodInOrder = new List<GoodInOrder>();


            using (MySqlConnection conn2 = GetConnection())
            {
                conn2.Open();
                string sql2 = $"SELECT product_id, name, model, quantity, price, total, order_id from oc_order_product WHERE order_id={orderId};";
                MySqlCommand cmd2 = new MySqlCommand(sql2, conn2);
                using (MySqlDataReader mysql_result2 = cmd2.ExecuteReader())
                {
                    while (mysql_result2.Read())
                    {

                        listGoodInOrder.Add(
                            new GoodInOrder()
                            {
                                OrderId = (int)mysql_result2["order_id"],
                                Product_id = (int)mysql_result2["product_id"],
                                Name = (string)mysql_result2["name"],
                                Model = (string)mysql_result2["model"],
                                Quantity = (int)mysql_result2["quantity"],
                                Price = (decimal)mysql_result2["price"],
                                Total = (decimal)mysql_result2["total"]
                            });
                    }
                }
            }
            return listGoodInOrder;
        }

        public Dictionary<int, Order> GetListOrder(string connstring = null)
        {
            Dictionary<int, Order> listorder = new Dictionary<int, Order>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string sql = $"SELECT o.order_id, o.firstname, o.lastname, o.telephone, o.shipping_city, o.shipping_address_1, o.comment, o.date_added from oc_order AS o ORDER BY  o.order_id DESC LIMIT 10 ;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                using (MySqlDataReader mysql_result = cmd.ExecuteReader())
                {
                    while (mysql_result.Read())
                    {
                        try
                        {
                            listorder.Add(
                                (int)mysql_result["order_id"],
                                new Order
                                {
                                    Order_id = (int)mysql_result["order_id"],
                                    Firstname = (string)mysql_result["firstname"],
                                    Lastname = (string)mysql_result["lastname"],
                                    Telephone = (string)mysql_result["telephone"],
                                    Shipping_city = (string)mysql_result["shipping_city"],
                                    Shipping_address_1 = (string)mysql_result["shipping_address_1"],
                                    Comment = (string)mysql_result["comment"],
                                    Date_added = (DateTime)mysql_result["date_added"]
                                });
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }
                }
                conn.Close();
            }
            return listorder;
        }

    }
}

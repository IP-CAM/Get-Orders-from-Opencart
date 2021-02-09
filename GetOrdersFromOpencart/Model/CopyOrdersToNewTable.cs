using MySql.Data.MySqlClient;
using Renci.SshNet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace GetOrdersFromOpencart.Model
{
    public class CopyOrdersToNewTable
    {

        public IConfiguration Configuration { get; }
        private string Config { get; set; }
        public string LastOrder { get; set; }
        public CopyOrdersToNewTable(IConfiguration config)
        {
            Configuration = config;
        }
        public CopyOrdersToNewTable(string config)
        {
            Config = config;
        }




        internal MySqlConnection GetConnection(string configuration = null)
        {
            if (configuration == null)
            {
                Config = "Database=" + Configuration["BaseConnect:Database"] + ";Datasource=" + Configuration["BaseConnect:Host"] + ";User=" + Configuration["BaseConnect:User"] + ";Password=" + Configuration["BaseConnect:Password"];
            }
            else
            {
                Config = configuration;
            }

            MySqlConnection myConnection = new MySqlConnection(Config);
            if (myConnection == null)
            {
                Console.WriteLine("Соединение не установлено");
            }
            return myConnection;
        }

        private static ConcurrentQueue<string> dicti = new ConcurrentQueue<string>();

        public ConcurrentQueue<string> ReturnQueue { get; } = dicti;
        public async Task CopyToNewTableAsync()
        {
            await PuskAsync();
        }

        public async Task PuskAsync()
        {
            try
            {
                Dictionary<int, Order> ordersInNewTable = await GetListOrder(); //получил список заказов в базе
                await AddOrderInNewFile(ordersInNewTable); // переместил все новые заказы в новую таблицу
                                                           // и выполнил работу телеграмм бота по отсылке 
                dicti.Enqueue(DateTime.Now.ToString() + " to base");
            }
            catch (Exception ex)
            {
                dicti.Enqueue(ex.Message);
                Debug.WriteLine($"{DateTime.Now} : {ex.Message}");
            }
        }


        public async Task SendMessageToTelegram(Order newOrder)
        {
            Connect conn = default;
            if (Configuration != null)
            {
                conn = new Connect(Configuration);
            }
            else
            {
                conn = new Connect(Config);
            }

            SendMessageFromBot bot = default;
            if (Configuration == null)
            {
                bot = new SendMessageFromBot("1060535548:AAEO0X5E2_FYnGqT9xj3LU4upkj-WVcDKBI");
            }
            else
            {
                bot = new SendMessageFromBot(Configuration);
            }



            string dataFromOrder = await GetStringForMessage(newOrder);

            List<GoodInOrder> list = conn.GetListGoodInOrder(newOrder.Order_id);

            string dataFromGoodsInOrder = default;
            foreach (var item in list)
            {
                dataFromGoodsInOrder += $"\n{item.Model} {item.Name} : {item.Price} ~ {item.Quantity}";
                bot.Send(dataFromOrder + " " + dataFromGoodsInOrder);
            }
        }
        static async Task<string> GetStringForMessage(Order newOrder)
        {
            string dataFromOrder = $"Получен новый заказ {newOrder.Order_id} в : {newOrder.Date_added}" +
               $"\n клиент - {newOrder.Firstname} {newOrder.Lastname} {newOrder.Telephone}" +
               $"\n адрес - {newOrder.Shipping_city} {newOrder.Shipping_address_1}" +
               $"\n комментарий - {newOrder.Comment}";
            return await Task.Run(() => dataFromOrder);
        }


        public async Task AddOrderInNewFile(Dictionary<int, Order> ordersInNewTable)
        {
            using (MySqlConnection conn = VernuConnect())
            {
                conn.Open();
                foreach (var key in ordersInNewTable)
                {
                    string sql = $"SELECT Id from oc_order_my WHERE orderId={key.Key};";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    using (MySqlDataReader mysql_result = cmd.ExecuteReader())
                    {
                        if (!mysql_result.Read())
                        {
                            try
                            {
                                //выполнил работу телеграмм бота по отсылке уведомлений
                                await SendMessageToTelegram(key.Value);

                                conn.Close();
                                conn.Open();
                                string sqlAdd = $"INSERT INTO oc_order_my (orderId) VALUES ({key.Key});";
                                MySqlCommand cmd2 = new MySqlCommand(sqlAdd, conn);
                                var res = cmd2.ExecuteNonQuery();
                                if (res <= 0) throw new Exception("Не удалось добавить строку");
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex.Message);
                                throw new Exception("Не удалось добавить строку");
                            }
                        }
                    }
                }

                conn.Close();
            }
        }

        public int SelectInMyOrderFromId(int key)
        {
            int result = default;
            using (MySqlConnection conn = VernuConnect())
            {
                conn.Open();
                string sql = $"SELECT Id from oc_order_my WHERE orderId={key};";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                using (MySqlDataReader mysql_result = cmd.ExecuteReader())
                {
                    if (mysql_result.FieldCount > 0)
                    {
                        mysql_result.Read();
                        result = (int)mysql_result["Id"];
                    }
                }
                conn.Close();
            }
            return result;
        }
        public async Task<Dictionary<int, Order>> GetListOrder()
        {
            Dictionary<int, Order> listorder = new Dictionary<int, Order>();

            using (MySqlConnection conn = VernuConnect())
            {
                conn.Open();
                string sql = $"SELECT o.order_id, o.firstname, o.lastname, o.telephone, o.shipping_city, o.shipping_address_1, o.comment, o.date_added from oc_order AS o;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                bool check = true;

                using (MySqlDataReader mysql_result = cmd.ExecuteReader())
                {
                    try
                    {
                        while (mysql_result.Read())
                        {
                            try
                            {
                                if (check == true)
                                {
                                    try
                                    {
                                        var rrr = mysql_result["date_added"].ToString();
                                        LastOrder = rrr;
                                        check = false;
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        throw;
                                    }
                                }
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
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
                conn.Close();
            }
            return await Task.Run(() => listorder);
        }

        public string ReturnDateLastOrder()
        {

            //не работает!!!!!!!!!!!!

            //var conne = new Connect(Configuration);
            //using (MySqlConnection conn = conne.GetConnection())
            //{
            //    conn.Open();
            //    string sql = $"SELECT o.order_id, o.firstname, o.lastname, o.telephone, o.shipping_city, o.shipping_address_1, o.comment, o.date_added from oc_order AS o;";
            //    MySqlCommand cmd = new MySqlCommand(sql, conn);



            //    using (MySqlDataReader mysql_result = cmd.ExecuteReader())
            //    {
            //        try
            //        {
            //            while (mysql_result.Read())
            //            {
            //                LastOrder = (string)mysql_result["date_added"];
            //                break;
            //            }

            //        }
            //        catch (Exception)
            //        {
            //            LastOrder = "вылетел запрос ";
            //        }

            //    }
            //    conn.Close();
            //}

            LastOrder = "19.08.2020";
            return LastOrder;
        }



        private MySqlConnection VernuConnect()
        {
            Connect conne = default;
            if (Configuration != null)
            {
                conne = new Connect(Configuration);
            }
            else
            {
                conne = new Connect(Config);
            }
            return conne.GetConnection();
        }
    }
}

using GetOrdersFromOpencart.Model;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetOrdersFromOpencart.Hubs
{
    public class OrderSaveHub : Hub<IOrderSave>
    {
        CopyOrdersToNewTable meth;
        public OrderSaveHub(CopyOrdersToNewTable cc)
        {
            meth = cc;
        }


        public Task NotifyForOrder(ForHub message)
        {
            ForHub clientsss = new ForHub { Text = $"Your message {message.Text}. Sorry, acces denied! Ha-ha", Sender = Context.Items["Name"] as string };
            return Clients.Others.SendNotify(clientsss);
        }



        public Task<string> GetLastOrder()
        {
            var result = meth.ReturnDateLastOrder();
            return  Task.FromResult(result);
        }

        public Task<string> SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Task.FromResult("Имя: Anonymous");
            }
            if (Context.Items.ContainsKey("Name"))
            {
                Context.Items["Name"] = name;
            }
            else
            {
                Context.Items.Add("Name", name);
            } 
            return Task.FromResult($"{Context.Items["Name"] as string}");
        }

        public Task<string> GetName()
        {
            if (Context.Items.ContainsKey("Name"))
                return Task.FromResult(Context.Items["Name"] as string);

            return Task.FromResult("Anonymous");
        }
    }
}

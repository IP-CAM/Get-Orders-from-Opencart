using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GetOrdersFromOpencart.Model
{
    public class TestTimer
    {
        public TestTimer()
        {
           // StartTest();
        }

        public void StartTest()
        {
            while (true)
            {
                Thread.Sleep(5000);
       
                Debug.WriteLine(DateTime.Now.ToString());

            }
        }
    }
}

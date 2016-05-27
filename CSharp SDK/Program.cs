using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_SDK
{
    class Program
    {
        static void Main(string[] args)
        {
            string Systemkey = "YOUR SYSTEM KEY";
            string SystemSecret = "YOUR SYSTEM SECRET";
            Dictionary<string, object> initOptions = new Dictionary<string, object>();
            initOptions.Add("platfromURL", "https://staging.clearblade.com");
            initOptions.Add("messageURL", "staging.clearblade.com");
            initOptions.Add("email", "");
            initOptions.Add("password", "");
            initOptions.Add("registerUser", false);
            initOptions.Add("logging", false);
            initOptions.Add("callTimeout",30000);
            initOptions.Add("allowUntrusted", false);
            ClearBlade clearblade = new ClearBlade();
            clearblade.initialize(Systemkey, SystemSecret, InitCallback);
            
            //string parameters = "{\"name\":\"Ameya\"}";
            //ClearBladeCodeService code = new ClearBladeCodeService("tempServ", parameters);
            //bool codeResult = code.executeCode("tempServ", parameters);
            //Console.WriteLine("Code result "+codeResult);

            System.Threading.Thread.Sleep(1000);
            ClearBladeMessageService message = new ClearBladeMessageService("Test Client",1,true);

            System.Threading.Thread.Sleep(1000);
            message.publish("TopicName", "Hello from C# client", 2, true);

            System.Threading.Thread.Sleep(1000);
            message.subscribe("TopicName");
            message.publish("TopicName", "Received Nicely by C# subscriber", 2, true);

            //System.Threading.Thread.Sleep(1000);
            //message.unsubscribe("weave");
            message.publish("TopicName", "Received Nicely by C# subscriber", 2, true);

            //System.Threading.Thread.Sleep(1000);
            //bool disconnect = message.disconnect();
            //Console.WriteLine("Disconnected : "+disconnect);
        }

        public static void InitCallback(bool success, string data)
        {
            if (success)
            {
                Console.WriteLine("Result is: "+success);
            }else
            {
                Console.WriteLine();
            }
        }
        
    }
}

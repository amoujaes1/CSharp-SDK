using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_SDK.Internal;

namespace CSharp_SDK
{
    class ClearBladeCodeService
    {
        private string serviceName;
        private string parameters;

        private RequestEngine request;
        
        public ClearBladeCodeService(string serviceName)
        {
            this.serviceName = serviceName;
        }

        public ClearBladeCodeService(string serviceName, string parameters)
        {
            this.serviceName = serviceName;
            this.parameters = parameters;
        }

        public bool executeCode(string serviceName)
        {
            request = new RequestEngine();
            Console.WriteLine("Service Name: "+serviceName);
            RequestProperties headers = new RequestProperties();
            headers.setMethod("POST");
            string endpoint = "/api/v/1/code/" + Util.getSystemKey() + "/" + this.serviceName + "";
            headers.setEndpoint(endpoint);


            request.setHeaders(headers);

            PlatformResponse<string> result = request.execute();
            if (result.getError())
            {
                Util.logger("CBUserTask", "User call failed: " + result.getData(), true);
                return false;
            }
            else
            {
                Console.WriteLine((string)result.getData());

                return true;
            }
        }

        public bool executeCode(string serviceName,string parameters)
        {
            request = new RequestEngine();
            Console.WriteLine("Service Name: " + serviceName);
            RequestProperties headers = new RequestProperties();
            headers.setMethod("POST");
            string endpoint = "/api/v/1/code/" + Util.getSystemKey() + "/" + this.serviceName + "";
            headers.setEndpoint(endpoint);
            headers.setBody(parameters);

            request.setHeaders(headers);

            PlatformResponse<string> result = request.executePost();
            if (result.getError())
            {
                Util.logger("CBUserTask", "User call failed: " + result.getData(), true);
                return false;
            }
            else
            {
                Console.WriteLine((string)result.getData());

                return true;
            }
        }
    }
}

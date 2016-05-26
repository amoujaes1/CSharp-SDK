using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_SDK.Internal
{
    class RequestProperties
    {
        private string authentication { set; get; }       // Holds authentication for API Call
        private static string body { set; get; }         // Holds the content of the API Call
        private string endPoint { set; get; }           // The section of the backend to send this information to, 
        private string method { set; get; }            // The http method to use
        private string qs { set; get; }               // the query string to be used
        private int timeout { set; get; }            // the time the API CALL will wait for a connection until it aborts.
        private static string uri { set; get; }     // the backend uri

        public void setAuthentication(string auth)
        {
            authentication = auth;
        }

        public void setBody(string body)
        {
            RequestProperties.body = body;
        }

        public void setEndpoint(string endpoint)
        {
            this.endPoint = endpoint;
        }

        public void setMethod(string method)
        {
            this.method = method;
        }

        public void setTimeout(int timeout)
        {
            this.timeout = timeout;
        }

        public void setQs(string qs)
        {
            this.qs = qs;
        }

        public void setUri(string uri)
        {
            RequestProperties.uri = uri;
        }

        public static string getBody()
        {
            return body;
        }
        
        public string getMethod()
        {
            return method;
        }

        public int getTimeout()
        {
            return timeout;
        }

        public static string getUri()
        {
            return uri;
        }

        public string getEndpoint()
        {
            return endPoint;
        }
    }
}

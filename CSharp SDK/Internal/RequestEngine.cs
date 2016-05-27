using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace CSharp_SDK.Internal
{
    class RequestEngine
    {
        private readonly string TAG = "RequestEngine";
	
	    //internal flag for understanding the use of SSL on the platform server
	    private int SSL = -1;

        private RequestProperties headers;

        /**
         * Constructs a RequestEngine Object with null RequestProperties object
         */
        public RequestEngine()
        {
            this.headers = null;
        }

        /**
         * Constructs a RequestEngine Object with given RequestProperties object
         * @param headers
         */
        public RequestEngine(RequestProperties headers)
        {
            this.headers = headers;
        }

        /**
        * Sets the RequestEngine Object's headers to the given RequestProperties
        * object.
        * @param headers RequestProperties to make API Call from
        */
        public void setHeaders(RequestProperties headers)
        {
            this.headers = headers;
        }

        /**
	    * Returns an ApiResponse<String> object that contains the 
	     * results of the API call. The String in ApiResponse<String>.getData()
	    * is usually converted to a more useful object.
	    * @return result stores the condition of the ApiRequest
	    */
        public PlatformResponse<string> execute()
        {
            return request();
        }

        /**
         * Returns an ApiResponse<String> object that contains the 
         * results of the API call. The String in ApiResponse<String>.getData()
         * is usually converted to a more useful object.
         * @return result stores the condition of the ApiRequest
         */
        public PlatformResponse<string> executeOnActivity()
        {
            return request();
        }

        public PlatformResponse<string> executePost()
        {
            return Postrequest();
        }

        private PlatformResponse<string> request()
        {
            if (this.headers == null)
            {
                throw new ArgumentNullException("The headers must not be null!");
            }

            //Used to determine if error occurred during call
            bool err = false;

            // The two variables hold the server Status codes.
            int responseCode = 0;
            string responseMessage = null;

            PlatformResponse<String> result = null;

            string reqUrl = RequestProperties.getUri().ToLower();
            string method = this.headers.getMethod();
            string endpoint = this.headers.getEndpoint();
            reqUrl += endpoint;
            Console.WriteLine("request url is:" + reqUrl);

            try { 
                var request = (HttpWebRequest)WebRequest.Create(reqUrl);

                var postData = "";
                Console.WriteLine(postData);

                var data = Encoding.ASCII.GetBytes(postData);
                String charset = "UTF-8";

                request.Method = method;
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                request.ContinueTimeout = this.headers.getTimeout();
                request.Accept = "application/json";

                bool isLogout = (reqUrl.Contains("/user/logout"));

                bool isAuthOrReg = (reqUrl.Contains("/user/auth") ||
                                    reqUrl.Contains("/user/anon") ||
                                    reqUrl.Contains("/user/reg"));
                string authToken = ClearBlade.getCurrentUser().getAuthToken();
                if (isAuthOrReg)
                {
                    request.Headers.Add("CLEARBLADE-SYSTEMKEY", Util.getSystemKey());
                    request.Headers.Add("CLEARBLADE-SYSTEMSECRET", Util.getSystemSecret());
                } else if (isLogout) {
                    request.Headers.Add("CLEARBLADE-SYSTEMKEY", Util.getSystemKey());
                    request.Headers.Add("CLEARBLADE-SYSTEMSECRET", Util.getSystemSecret());
                    request.Headers.Add("ClearBlade-UserToken", authToken);
                } else if (authToken != null)
                {
                    request.Headers.Add("ClearBlade-UserToken", authToken);
                }
                request.Headers.Add("Accept-Charset", charset);

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                var json = new StreamReader(response.GetResponseStream()).ReadToEnd();

                responseCode = (int)response.StatusCode;
                if (responseCode / 100 == 2)
                {  // If the response code is within 200 range success
                    result = new PlatformResponse<String>(err, json);
                    Console.WriteLine("Got 200 OK from server");
                    Util.logger(TAG, method + " " + responseCode + ":" + responseMessage, false);
                }
                else
                {   // else an Error Occurred 
                    Console.WriteLine("Got Error from server");
                    String errResp = responseCode + ":" + responseMessage + ":" + json;
                    Util.logger(TAG, errResp, true);
                    err = true;
                    result = new PlatformResponse<String>(err, errResp);
                }
                response.Close();
            }
            catch (WebException webex)
            {
                Console.WriteLine("Inside cath");
                WebResponse errResp = webex.Response;
                using (Stream respStream = errResp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream);
                    string json = reader.ReadToEnd();
                    Console.WriteLine("Response: " +json);
                }
            }
            
            
            return result;

        }

        private PlatformResponse<string> Postrequest()
        {
            if (this.headers == null)
            {
                throw new ArgumentNullException("The headers must not be null!");
            }

            //Used to determine if error occurred during call
            bool err = false;

            // The two variables hold the server Status codes.
            int responseCode = 0;
            string responseMessage = null;

            PlatformResponse<String> result = null;

            string reqUrl = RequestProperties.getUri().ToLower();
            string method = this.headers.getMethod();
            string endpoint = this.headers.getEndpoint();
            reqUrl += endpoint;
            Console.WriteLine("request url is:" + reqUrl);
            Console.WriteLine("request method is:" + method);


            try
            {
                var request = (HttpWebRequest)WebRequest.Create(reqUrl);

                var postData = RequestProperties.getBody();
                Console.WriteLine(postData);

                var data = Encoding.ASCII.GetBytes(postData);
                String charset = "UTF-8";

                request.Method = method;
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                request.ContinueTimeout = this.headers.getTimeout();
                request.Accept = "application/json";

                bool isLogout = (reqUrl.Contains("/user/logout"));
                bool isAuthOrReg = (reqUrl.Contains("/user/auth") ||
                                    reqUrl.Contains("/user/anon") ||
                                    reqUrl.Contains("/user/reg"));
                string authToken = ClearBlade.getCurrentUser().getAuthToken();
                if (isAuthOrReg)
                {
                    request.Headers.Add("CLEARBLADE-SYSTEMKEY", Util.getSystemKey());
                    request.Headers.Add("CLEARBLADE-SYSTEMSECRET", Util.getSystemSecret());
                }
                else if (isLogout)
                {
                    request.Headers.Add("CLEARBLADE-SYSTEMKEY", Util.getSystemKey());
                    request.Headers.Add("CLEARBLADE-SYSTEMSECRET", Util.getSystemSecret());
                    request.Headers.Add("ClearBlade-UserToken", authToken);
                }
                else if (authToken != null)
                {
                    request.Headers.Add("ClearBlade-UserToken", authToken);
                }
                request.Headers.Add("Accept-Charset", charset);

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                
                var response = (HttpWebResponse)request.GetResponse();
                var json = new StreamReader(response.GetResponseStream()).ReadToEnd();

                responseCode = (int)response.StatusCode;
                Console.WriteLine("Response code " + responseCode);
                if (responseCode / 100 == 2)
                {  // If the response code is within 200 range success
                    result = new PlatformResponse<String>(err, json);
                    Console.WriteLine("Got 200 OK from server");
                    Util.logger(TAG, method + " " + responseCode + ":" + responseMessage, false);
                }
                else
                {   // else an Error Occurred 
                    Console.WriteLine("Got Error from server");
                    String errResp = responseCode + ":" + responseMessage + ":" + json;
                    Util.logger(TAG, errResp, true);
                    err = true;
                    result = new PlatformResponse<String>(err, errResp);
                }
                response.Close();
            }
            catch (WebException webex)
            {
                Console.WriteLine("Inside cath");
                WebResponse errResp = webex.Response;
                using (Stream respStream = errResp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream);
                    string json = reader.ReadToEnd();
                    Console.WriteLine("Response: " + json);
                }
            }

            return result;

        }

        /**
	    * Internal function to identify if the hosted platform is running under SSL
	    * @return true if the platform uri contains https at the start
	    */
        private bool isSSL()
        {
            if(SSL == -1)
            {
                if (RequestProperties.getUri().StartsWith("https"))
                {
                    SSL = 1;
                }
                else
                {
                    SSL = 0;
                }
            }

            if(SSL == 1)
            {
                return true;
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_SDK.Internal;
using Newtonsoft.Json.Linq;

namespace CSharp_SDK
{
    class User
    {
        private static string email;                //users email address
        private static string authToken;            //auth token for user

        private RequestEngine request;			//used to make api requests
        
        public User(string e)
        {
            if(e == null)
            {
                email = "anonymous";
            }else
            {
                email = e;
            }
        }

        public string getEmail()
        {
            return email;
        }

        public void setAuthToken(string authToken)
        {
            User.authToken = authToken;
        }

        public string getAuthToken()
        {
            return authToken;
        }

        public bool authWithAnonUser(string uri, int timeout)
        {
            request = new RequestEngine();
            Console.WriteLine("Req URI: "+uri);
            RequestProperties headers = new RequestProperties();
            headers.setMethod("POST");
            headers.setEndpoint("/api/v/1/user/anon");
            headers.setUri(uri);
            headers.setTimeout(timeout);

            request.setHeaders(headers);

            PlatformResponse<string> result = request.execute();
            if (result.getError())
            {
                Util.logger("CBUserTask", "User call failed: " + result.getData(), true);
                ClearBlade.setInitError(true);
                return false;
            }
            else
            {
                Console.WriteLine((string) result.getData());
                JObject json = JObject.Parse((string)result.getData());
                setAuthToken( (string)json["user_token"]);
                Console.WriteLine("Auth Token:"+getAuthToken());
                return true;
            }
        }

        public bool authWithCurrentUser(string password, string uri)
        {
            //get auth token with current user
            request = new RequestEngine();

            string email = this.getEmail();
            string body = "{\"email\":\""+email+"\",\"password\":\""+password+"\"}";
            

            RequestProperties headers = new RequestProperties();
            headers.setMethod("POST");
            headers.setEndpoint("/api/v/1/user/auth");
            headers.setUri(uri);
            headers.setBody(body);

            request.setHeaders(headers);
            Console.WriteLine("In Auth with current user");
            PlatformResponse<string> result = request.executePost();
            if (result.getError())
            {
                Util.logger("CBUserTask", "User call failed: " + result.getData(), true);
                ClearBlade.setInitError(true);
                Console.WriteLine("Some error in result");
                return false;
            }
            else
            {
                Console.WriteLine((string)result.getData());
                JObject json = JObject.Parse((string)result.getData());
                setAuthToken((string)json["user_token"]);
                return true;
            }


        }
    }
}

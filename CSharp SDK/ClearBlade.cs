using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_SDK
{
    class ClearBlade
    {
        public delegate void InitCallback(bool result, string data);

        private readonly static string TAG = "ClearBlade";
        private readonly static string apiVersion = "0.0.1";
        private readonly static string sdkVersion = "0.0.1";
        private static int callTimeOut;                         // http Requests will be aborted after this amount of milliseconds
        private static bool logging;                            // if the user wants internal logs to show.
        private static string masterSecret;                      // App's Admin Password; has access to Everything
        private static string uri;                              // Default URL to send Api requests to
        private static string messageUrl;                       // Default URL to connect to the message broker with
        private static User user;                               // Current User of the application. Not implemented Yet
        private static bool initError = false;
        private static bool allowUntrusted = false;         // if the platform has https enabled but no publically signed certificate

        public ClearBlade()
        {

        }

        /**
	    * Returns the version of the API that is currently in use.
	    * 
	     * @return The API version
	     */
        public static String getApiVersion()
        {
            return apiVersion;
        }



        /** 
         * Returns the milliseconds that the API will wait for a connection to the backend
         * until API requests are aborted.
         * @return milliseconds 
         */
        public static int getCallTimeOut()
        {
            return callTimeOut;
        }

        /**
         * Returns the current user of the Application
         * @return Current user object
         */
        public static User getCurrentUser()
        {
            return user;
        }

        /**
         * Returns the version of the SDK  in use
         * @return SDK version
         */
        public static String getSdkVersion()
        {
            return sdkVersion;
        }

        /**
         * Returns the uri of the backend that will be used for API calls
         * @return uri of the Backend
         */
        public static String getUri()
        {
            return uri;
        }

        /**
         * Sets uri of the backend platform that will be used for API calls
         * Typically scenarios are https://platform.clearblade.com
         */
        public static void setUri(String platformURI)
        {
            uri = platformURI;
        }

        /**
         * Sets the url of the message broker that will be used in messaging applications
         * Defaults to 'tcp://platform.clearblade.com:1883'
         * @param messageURL the string that will be set as the url
         */
        public static void setMessageUrl(String messageURL)
        {
            messageUrl = messageURL;
        }

        /**
         * Gets the url of the message broker that was set upon initialization
         * @return URL of message broker
         */
        public static String getMessageUrl()
        {
            return messageUrl;
        }

        /**
         * Allows for passing requests to an untrusted server.  This method
         * is not recommended for any scenario other than development
         */
        public static void setAllowUntrusted(bool allowUntrustedCertificates)
        {
            allowUntrusted = allowUntrustedCertificates;
        }

        /**
         * Allows for passing requests to an untrusted server. 
         * @return boolean value for using untrusted backend servers
         */
        public static bool getAllowUntrusted()
        {
            return allowUntrusted;
        }


        public void initialize(String SYSTEM_KEY, String SYSTEM_SECRET, InitCallback initCallback)
        {
            if(user != null)
            {
                user = null;
            }

            if(SYSTEM_KEY == null)
            {
                throw new ArgumentNullException(" The System Key must be a non-empty String");
            }

            if (SYSTEM_SECRET == null)
            {
                throw new ArgumentNullException(" The System Secret must be a non-empty String");
            }

            Util.setSystemKey(SYSTEM_KEY);
            Util.setSystemSecret(SYSTEM_SECRET);
            masterSecret = null;
            uri = "https://platform.clearblade.com";
            messageUrl = "platform.clearblade.com";
            logging = false;
            callTimeOut = 30000;

            user = new User(null);
            bool initResult = user.authWithAnonUser(uri, callTimeOut);
            string resultData = "";
            if (initResult)
            {
                resultData = "";
            }else
            {
                resultData = "Unable to Auth!";
            }
            initCallback(initResult,resultData);  
        }

        public void initialize(String SYSTEM_KEY, String SYSTEM_SECRET, InitCallback initCallback, Dictionary<string, object> initOptions)
        {
            if (SYSTEM_KEY == null)
            {
                throw new ArgumentNullException(" The System Key must be a non-empty String");
            }

            if (SYSTEM_SECRET == null)
            {
                throw new ArgumentNullException(" The System Secret must be a non-empty String");
            }

            validateInitOptions(initOptions, initCallback);

            Util.setSystemKey(SYSTEM_KEY);
            Util.setSystemSecret(SYSTEM_SECRET);

            string platformURL = (string)initOptions["platfromURL"];
            string messageURL = (string)initOptions["messageURL"];
            bool log = (bool)initOptions["logging"];
            setLogging(log);
            int timeout = (int)initOptions["callTimeout"];

            if (platformURL != null)
            {
                uri = platformURL;
            }
            else
            {
                uri = "https://staging.clearblade.com";
            }

            if (messageURL != null)
            {
                messageUrl = messageURL;
            }
            else
            {
                messageUrl = "staging.clearblade.com";
            }

            if (timeout > 0)
            {
                setCallTimeOut(timeout);
            }
            else
            {
                setCallTimeOut(30000);
            }

            if (log)
            {
                logging = log;
            }else
            {
                logging = false;
            }

            //init registerUser
            bool registerUser = (bool)initOptions["registerUser"];
            

            //init untrusted
            bool allowUntrusted = (bool)initOptions["allowUntrusted"];
            setAllowUntrusted(allowUntrusted);

            string email = (string)initOptions["email"];
            string password = (string)initOptions["password"];

            user = new User(email);

            if (!initError && email != null && !registerUser)
            {
                //no init error, an email was given, and don't register user
                //just auth with given user info
                bool initResult= user.authWithCurrentUser(password, uri);
                string resultData = "";
                if (initResult)
                {
                    resultData = "";
                }
                else
                {
                    resultData = "Unable to Auth!";
                }
                initCallback(initResult, resultData);
            }
            else if (!initError && registerUser)
            {
                //no errors, and register new user
                //user.registerUser(password, initCallback);
            }
            else if (!initError && email == null)
            {
                //email is null, so try to auth as anon user
                user.authWithAnonUser(uri, callTimeOut);
            }
        }

        private static void validateInitOptions(Dictionary<String, Object> initOptions, InitCallback initcallback)
        {
            initError = false;

            string email = (string)initOptions["email"];
            string password = (string)initOptions["password"];
            bool shouldRegister = (bool)initOptions["registerUser"];
            if (email == null && password != null)
            {
                initError = true;
                initcallback(!initError,"Must provide both an email and password to authenticate. You only provided a password");
            }
            else if (email != null && password == null)
            {
                initError = true;
                initcallback(!initError,"Must provide both an email and password to authenticate. You only provided an email");
            }
            else if (shouldRegister && email == null)
            {
                initError = true;
                initcallback(!initError,"Cannot register anonymous user");
            }

        }

        public static bool isLogging()
        {
            return logging;
        }

        /**
	    * Sets the time in milliseconds that an http Request will wait for a 
	    * connection with the backend until it is aborted.
	    * @param timeOut milliseconds until http request is aborted
	    */
        public static void setCallTimeOut(int timeOut)
        {
            callTimeOut = timeOut;
        }

        /**
         * If value is true, internal API logs will be displayed throughout the use of the 
         * API else no internal logs displayed
         * @param value determines API logging
         */
        public static void setLogging(bool value)
        {
            logging = value;
        }

        /**
         * <p>Sets the masterSecret of the Application.
         * If masterSecret is set it will be used instead
         * of the appSecret. It gives complete access to APP resources
         * </p>
         * <strong>Never use the masterSecret in production code.</strong>
         * @param myMasterSecret - The Applications Admin secret
         */
        public static void setMasterSecret(String myMasterSecret)
        {
            masterSecret = myMasterSecret;
        }

        public static void setInitError(bool value)
        {
            initError = value;
        }
    }
}

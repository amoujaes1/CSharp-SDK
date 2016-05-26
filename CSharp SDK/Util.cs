using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_SDK
{
    class Util
    {
        private static String SYSTEM_KEY;                            // system Id
        private static String SYSTEM_SECRET;                     // system Password


        public static void setSystemKey(String SYSTEM_KEY)
        {
            Util.SYSTEM_KEY = SYSTEM_KEY;
        }

        public static void setSystemSecret(String SYSTEM_SECRET)
        {
            Util.SYSTEM_SECRET = SYSTEM_SECRET;
        }


        /**
         * Displays internal log messages when using the API
         * @protected
         * @param tag The Class calling
         * @param log The Message to display
         * @param error is the message an Error?
         */
        public static void logger(string tag, string log, bool error)
        {
            if (ClearBlade.isLogging())
            {
                if (error)
                {
                    Console.WriteLine(tag + ": " + log);
                }
                else
                {
                    //Log.v(tag, log);
                    Console.WriteLine(tag + ": " + log);
                }
            }
        }

        public static string getSystemKey()
        {
            return SYSTEM_KEY;
        }

        public static string getSystemSecret()
        {
            return SYSTEM_SECRET;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_SDK.Internal
{
    class PlatformResponse<T>
    {
        private bool error { get; }
        private T data;

        /**
	    * Constructs new ApiResponse of Type T
	    * @param error stored the condition of the API Call
	    * @param data stores data of type T
	    */
        public PlatformResponse(bool error, T data)
        {
            this.error = error;
            this.data = data;
        }

        public T getData()
        {
            return data;
        }

        public bool getError()
        {
            return error;
        }
    }
}

# CSharp-SDK
# API Reference
## Authenticating
Authentication is the very first and crucial step in using the ClearBlade C# API for your application. You will not be able to access any features of the ClearBlade platform without Authentication. 

You will need to clone the following C# repository for using the ClearBlade services:  
	https://github.com/ClearBlade/CSharp-SDK/


There are two ways to authenticate to the ClearBlade platform:

#### Without Options 
```C#
String SYSTEM_KEY = "your_systemkey";
String SYSTEM_SECRET = "your_systemsecret";


public static void InitCallback(bool success, string data)
{
	if (success)
	{
        //initialization successful
	}else
	{
		//initialization failed, given a ClearBladeException with the cause
    }
}

ClearBlade clearBlade = new ClearBlade();
clearBlade.initialize(SYSTEM_KEY, SYSTEM_SECRET, initCallback);
```

#### With Options
```C#
String SYSTEM_KEY = "your_systemkey";
String SYSTEM_SECRET = "your_systemsecret";
Dictionary<string, object> initOptions = new Dictionary<string, object>();

/**Available init options:
	 * 	email - String to register or log-in as specific user (required if password is given) Default - null
	 * 	password - password String for given user (required if email is given) Default - null
	 * 	platformURL - Custom URL for the platform Default - https://platform.clearblade.com
	 * 	messagingURL - Custom Messaging URL Default - tcp://messaging.clearblade.com:1883
	 * 	registerUser - Boolean to tell if you'd like to attempt registering the given user Default - false
	 * 	logging - Boolean to enable ClearBlade Internal API logging Default - false
	 * 	callTimeout - Int number of milliseconds for call timeouts Default - 30000 (30 seconds)
	 *  allowUntrusted - Boolean to connect to a platform server without a signed SSL certificate Default - false
*/

public static void InitCallback(bool success, string data)
{
	if (success)
	{
        //initialization successful
	}else
	{
		//initialization failed, given a ClearBladeException with the cause
    }
}

initOptions.Add("platfromURL", "https://staging.clearblade.com");
initOptions.Add("messageURL", "staging.clearblade.com");
initOptions.Add("email", "");
initOptions.Add("password", "");
initOptions.Add("registerUser", false);
initOptions.Add("logging", false);
initOptions.Add("callTimeout",30000);
initOptions.Add("allowUntrusted", false);

ClearBlade clearBlade = new ClearBlade();
clearBlade.initialize(SYSTEM_KEY, SYSTEM_SECRET, initOptions, initCallback);
```
## Code
The ClearBlade Java API allows executing a Code Service on the platform from your Java application.

**Please make sure that you have initialized and authenticated with the ClearBlade platform prior to using the Code API.**

You will need to clone the following C# repository for using the Code API:  
	https://github.com/ClearBlade/CSharp-SDK/

#### Code Service Without Parameters
A code service which does not take any parameters can be executed as follows:
```C#
string serviceName = "yourServiceName";
ClearBladeCodeService codeService = new ClearBladeCodeService(serviceName);
codeService.executeWithoutParams(serviceName);
```

#### Code Service With Parameters
A string object of parameters needs to be passed to the ```ClearBladeCodeService``` class constructor along with the service name:
```C#
string serviceName = "yourServiceName";
string parameters = "{\"param1\":\"value1\"}";
ClearBladeCodeService codeService = new ClearBladeCodeService(serviceName, parameters);
codeService.executeWithParams(serviceName, parameters);
```  
## Messaging

The Messaging API is used to initialize, connect and communicate with the ClearBlade MQTT Broker for publishing messages, subscribing, unsubscribing to and from topics and disconnect.

**Please make sure that you have initialized and authenticated with the ClearBlade platform prior to using the Messaging API. This is important because the ClearBlade MQTT Broker requires the authentication token to establish a successful connection. This authentication token can only be obtained by initializing and authenticaing with the ClearBlade platform**

You will need to clone the following C# repository for using the Messaging API:  
	https://github.com/ClearBlade/CSharp-SDK/

### Initialize and Connect  
The first step is to create a new ```ClearBladeMessageService``` object by passing the client ID and messaging QoS (optional). The ```ClearBladeMessageService``` constructor will then initialize and connect with the MQTT Broker.
```C#
string clientID = “ClearBladeJavaTest”; 
ClearBladeMessageService message = new ClearBladeMessageService(clientID); // QoS = 0 Default
```
OR
```C#
int qos = 1; // QoS can be 0,1 or 2
string clientID = “ClearBladeC#Test”;  
ClearBladeMessageService message = new ClearBladeMessageService(clientID, qos);
```
OR
```C#
int qos = 1; // QoS can be 0,1 or 2
string clientID = “ClearBladeC#Test”;  
bool cleanSession = true //or false
ClearBladeMessageService message = new ClearBladeMessageService(clientID, qos);
```

After the connection is successful, you can publish, subscribe, unsubscribe or disconnect using the ```ClearBladeMessageService``` object. 

### Publish
The publish function takes a topic and message of type ```string``` and publishes to the MQTT Broker.
```C#
string topic = "yourTopic";
string message = "yourMessage";
message.publish(topic, message);
```
OR
```C#
string topic = "yourTopic";
string message = "yourMessage";
int qos = "";
bool retain = "true" //or false
message.publish(topic, message, qos, retain);
```

### Subscribe
The subscribe function takes a topic of type ```string``` and a callback to handle the arrived messages.
```C#
string topic = "topicToSubscribe";
message.subscribe(topic);
```

### Unsubscribe
The unsubscribe function takes a topic of type ```string``.
```C#
string topic = "topicToUnsubscribe";
message.unsubscribe(topic);
```

### Disconnect
The disconnect function is used to disconnect from the MQTT Broker. **Note that this does not disconnect the user from the ClearBlade platform. User logout needs to be called separately.**
```C#
message.disconnect();
```
# Examples

Here's an example of a ClearBlade C# Client that initializes with the ClearBlade platform, connects to the MQTT broker and publishes and subscribes to messages. Then it disconnects from the MQTT Broker and logs out the user.  
```c#
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
            clearblade.initialize(Systemkey, SystemSecret, InitCallback,initOptions);
            
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
```



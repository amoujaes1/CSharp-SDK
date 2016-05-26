using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using uPLibrary.Networking.M2Mqtt;

namespace CSharp_SDK
{
    class ClearBladeMessageService
    {
        private static string messageUrl = ClearBlade.getMessageUrl();
        private string clientId;
        private int qos;
        private string authToken;
        private string systemKey;
        private bool cleanSession = false;

        private static MqttClient mqttClient;

        public ClearBladeMessageService(string clientId)
        {
            this.clientId = clientId;
            this.qos = 0;
        }

        public ClearBladeMessageService(string clientId, int qos)
        {
            this.clientId = clientId;
            this.qos = qos;
            connect();
        }

        public ClearBladeMessageService(string clientId, int qos, bool cleanSession)
        {
            this.clientId = clientId;
            this.qos = qos;
            this.cleanSession = cleanSession;
            connect();
        }

        public void connect()
        {
            User currentUser = ClearBlade.getCurrentUser();
            if (currentUser.getAuthToken() == null)
            {
                Console.WriteLine("Please authenticate!");
                return;
            }
            else
            {
                authToken = currentUser.getAuthToken();
                systemKey = Util.getSystemKey();
            }

            connectToService(authToken, systemKey, cleanSession);
        }

        private void connectToService(string username, string password, bool cleanSession)
        {
            try
            {
                mqttClient = new MqttClient(messageUrl);
                mqttClient.Connect(this.clientId, username, password, cleanSession, 10000);
                if (mqttClient.IsConnected)
                {
                    Console.WriteLine("Connected to ClearBlade MQTT Service");
                }else
                {
                    Console.WriteLine("Unable to Connect to ClearBlade MQTT Service");
                }
            }
            catch (uPLibrary.Networking.M2Mqtt.Exceptions.MqttClientException mClientex)
            {
                Console.WriteLine("MQTT Client exception: "+mClientex);
            }
            catch (uPLibrary.Networking.M2Mqtt.Exceptions.MqttConnectionException mConnectEx)
            {
                Console.WriteLine("MQTT Client exception: " + mConnectEx);
            }
            catch (uPLibrary.Networking.M2Mqtt.Exceptions.MqttTimeoutException mTimeoutEx)
            {
                Console.WriteLine("MQTT Client exception: " + mTimeoutEx);
            }
        }

        public bool disconnect()
        {
            if (!mqttClient.IsConnected)
            {
                Console.WriteLine("Disconnected from MQTT service.");
                return true;
            }

            if (mqttClient != null)
            {
                try
                {
                    mqttClient.Disconnect();
                }
                catch (uPLibrary.Networking.M2Mqtt.Exceptions.MqttConnectionException mConnectEx)
                {
                    Console.WriteLine("MQTT Client exception: " + mConnectEx);
                }

                mqttClient = null;
                Console.WriteLine("Disconnected from MQTT service.");
                return true;
            }else
            {
                Console.WriteLine("Unable to Disconnect from MQTT service.");
                return false;
            }
        }

        public void publish(string topic, string message)
        {
            try
            {
                if (mqttClient.IsConnected)
                {
                    mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message));
                    Console.WriteLine("Message Published successfully.");
                }
            }
            catch(uPLibrary.Networking.M2Mqtt.Exceptions.MqttCommunicationException mCommunicationEx)
            {
                Console.WriteLine("MQTT Client exception: " + mCommunicationEx);
            }
        }

        public void publish(string topic, string message, int qos, bool retain)
        {
            try
            {
                this.qos = qos;
                if (mqttClient.IsConnected)
                {
                    mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message), Convert.ToByte(qos), retain);
                    Console.WriteLine("Message Published successfully.");
                }

            }
            catch (uPLibrary.Networking.M2Mqtt.Exceptions.MqttCommunicationException mCommunicationEx)
            {
                Console.WriteLine("MQTT Client exception: " + mCommunicationEx);
            }
        }

        public void subscribe(string topic)
        {
            try
            {
                if (mqttClient.IsConnected)
                {
                    mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
                    mqttClient.Subscribe(new String[] { topic }, new byte[] { Convert.ToByte(qos) });
                    Console.WriteLine("Subscribed to \""+topic+"\" successfully.");
                }

            }
            catch (uPLibrary.Networking.M2Mqtt.Exceptions.MqttCommunicationException mCommunicationEx)
            {
                Console.WriteLine("MQTT Client exception: " + mCommunicationEx);
            }
        }

        private void MqttClient_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("Message : " + System.Text.Encoding.Default.GetString(e.Message));
        }

        public bool unsubscribe(string topic)
        {
            try
            {
                mqttClient.Unsubscribe(new String[] { topic });
                Console.WriteLine("Unubscribed from \"" + topic + "\" successfully.");
                return true;
            }
            catch (uPLibrary.Networking.M2Mqtt.Exceptions.MqttCommunicationException mCommunicationEx)
            {
                Console.WriteLine("MQTT Client exception: " + mCommunicationEx);
                return false;
            }
        }
    }
}

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.MQTT
{
    public class MqttConnection
    {
        #region Properties

        public readonly List<string> Topics;
        public readonly int Port;
        public readonly string Server;
        public readonly string ClientID;
        public readonly string Username;
        public readonly string Password;

        public readonly MqttClientOptions MqttClientOptions;
        public IMqttClient MqttClient;

        #endregion

        #region Events

        /// <summary>
        /// Raises, when connected to the MQTT Client.
        /// </summary>
        public static event EventHandler<object> OnConnected;

        public static void OnConnected_Event(object sender, object args)
        {
            OnConnected?.Invoke(sender, args);
        }


        /// <summary>
        /// Raises, when disconnected from the MQTT Client.
        /// </summary>
        public static event EventHandler<object> OnDisconnected;

        public static void OnDisconnected_Event(object sender, object args)
        {
            OnDisconnected?.Invoke(sender, args);
        }


        /// <summary>
        /// Raises, when the MQTT Client has error.
        /// </summary>
        public static event EventHandler<object> OnErrorAtSending;

        public static void OnErrorAtSending_Event(object sender, object args)
        {
            OnErrorAtSending?.Invoke(sender, args);
        }


        /// <summary>
        /// Raises, when the MQTT Client receive a message.
        /// </summary>
        public static event EventHandler<object> OnMessageReceived;

        public static void OnMessageReceived_Event(object sender, object args)
        {
            OnMessageReceived?.Invoke(sender, args);
        }

        #endregion

        public MqttConnection(string server, int port, string clientID, string userName, string password, List<string> topics)
        {
            Server = server;
            Port = port;
            ClientID = clientID;
            Username = userName;
            Password = password;
            Topics = topics;

            MqttClientOptions = new MqttClientOptions
            {
                ClientId = ClientID,
                CleanSession = true,
                ChannelOptions = new MqttClientTcpOptions
                {
                    Server = Server,
                    Port = Port
                },
                Credentials = new MqttClientCredentials
                {
                    Username = Username,
                    Password = Encoding.UTF8.GetBytes(Password)
                }
            };
        }

        public MqttConnection(MqttClientOptions option, List<string> topics)
        {
            MqttClientOptions = option;
            Topics = topics;
        }

        private void OnApplicationDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            OnDisconnected_Event(this, obj);
        }

        private void OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs obj)
        {
            OnMessageReceived_Event(this, obj);
        }

        /// <summary>
        /// Initialized the MQTT connection.
        /// </summary>
        /// <returns></returns>
        public async Task CreateMQTTConnection()
        {
            MqttFactory mqttFactory = new MqttFactory();
            MqttClient = mqttFactory.CreateMqttClient();

            MqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnApplicationDisconnected);
            MqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnApplicationMessageReceived);

            MqttClient.UseConnectedHandler(async e =>
            {
                foreach (var topic in Topics)
                {
                    await MqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
                }

                OnConnected_Event(this, MqttClient);
            });

            await MqttClient.ConnectAsync(MqttClientOptions);
        }

        /// <summary>
        /// Send message to a specified topic.
        /// </summary>
        /// <param name="topic">Topic name.</param>
        /// <param name="message">Message in string.</param>
        public async Task SendMessageToTopic(string topic, string message)
        {
            try
            {
                await MqttClient.PublishAsync(topic, message);
            }
            catch (Exception ex)
            {
                OnErrorAtSending_Event(this, ex);
            }
        }

        /// <summary>
        /// Send message to a specified topic.
        /// </summary>
        /// <param name="topic">Topic name.</param>
        /// <param name="message">Message in IEnumerable<byte>.</param>
        public async Task SendMessageToTopic(string topic, IEnumerable<byte> message)
        {
            try
            {
                await MqttClient.PublishAsync(topic, message);
            }
            catch (Exception ex)
            {
                OnErrorAtSending_Event(this, ex);
            }
        }

        /// <summary>
        /// Subscribe to a topic.
        /// </summary>
        /// <param name="topic">Topic name.</param>
        public async Task SubscribeToTopic(string topic)
        {
            await MqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());

            Topics.Add(topic);
        }

        /// <summary>
        /// Unsubscribe from topic(s).
        /// </summary>
        /// <param name="topics">Topics name.</param>
        public async Task UnSubscribeFromTopic(params string[] topics)
        {
            await MqttClient.UnsubscribeAsync(topics);

            foreach (var item in topics)
            {
                if (Topics.Contains(item))
                {
                    Topics.Remove(item);
                }
            }
        }
    }
}
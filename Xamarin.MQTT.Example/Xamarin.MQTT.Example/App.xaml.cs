using MQTTnet;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.MQTT.Example
{
    public partial class App : Application
    {
        public static MqttConnection MqttConnection
        {
            get; set;
        }

        string ServerName = "<ServerName>";
        int Port = 1883;
        string ClientID = "<ClientID>";
        string Username = "<MQTT_CredUsername>";
        string Password = "<MQTT_CredPassword>";
        List<string> TopicList = new List<string>()
        {
           "your","topics"
        };

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected async override void OnStart()
        {
            App.MqttConnection = new MqttConnection(ServerName, Port, ClientID, Username, Password, TopicList);
            
            await App.MqttConnection.CreateMQTTConnection();

            MqttConnection.OnConnected += MqttConnection_OnConnected;
            MqttConnection.OnDisconnected += MqttConnection_OnDisconnected;
            MqttConnection.OnErrorAtSending += MqttConnection_OnErrorAtSending;
            MqttConnection.OnMessageReceived += MqttConnection_OnMessageReceived;
        }

        private void MqttConnection_OnMessageReceived(object sender, object e)
        {
            Console.WriteLine("MQTT received a message:" + ((MqttApplicationMessageReceivedEventArgs)e).ApplicationMessage.ToString()); //ApplicationMessage.Topic, ApplicationMessage.Payload, ...
        }

        private void MqttConnection_OnErrorAtSending(object sender, object e)
        {
            Console.WriteLine("MQTT trow exception while sending: " + ((Exception)e).StackTrace);
        }

        private void MqttConnection_OnDisconnected(object sender, object e)
        {
            Console.WriteLine("MQTT Disconnected.");
        }

        private void MqttConnection_OnConnected(object sender, object e)
        {
            Console.WriteLine("MQTT Connected.");
        }

        protected override void OnSleep() { }

        protected override void OnResume() { }
    }
}
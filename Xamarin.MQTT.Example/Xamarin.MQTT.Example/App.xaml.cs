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
        }

        protected override void OnSleep() { }

        protected override void OnResume() { }
    }
}
# Xamarin.MQTT

Usage: 

1. I created two contructors of the MQTTConnection class. You can init this object with a specific MqttClientOptions and a string array of your topics, or with username, passowrd, a string array of your topics, servername, port and clientid.

https://github.com/officialdoniald/Xamarin.MQTT/blob/master/Xamarin.MQTT.Example/Xamarin.MQTT.Example/App.xaml.cs

I created in the App.cs a static variable of the MQTTConnection class, so I can reach this object anywhere.

2. You have to connect to the MQTT server:

await App.MqttConnection.CreateMQTTConnection();

3. You can subscribe to the MQTTConnection events:

  MqttConnection.OnConnected += MqttConnection_OnConnected;
  
  MqttConnection.OnDisconnected += MqttConnection_OnDisconnected;
  
  MqttConnection.OnErrorAtSending += MqttConnection_OnErrorAtSending;
  
  MqttConnection.OnMessageReceived += MqttConnection_OnMessageReceived;

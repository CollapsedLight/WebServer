using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using CollapsedLight.WebApp.Helper;

namespace CollapsedLight.WebApp.Data
{
    public class ControlerMqttHandler
    {
        public IManagedMqttClient MqttClient { get; }

        private IManagedMqttClientOptions _options;

        public ControlerMqttHandler()
        {
            _options = InitializeMQTT();
            MqttClient = Setup(_options).Result;
        }

        public ManagedMqttClientOptions InitializeMQTT()
        {
            return new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithTcpServer(Credentials.MQTT, 1883)
                    .Build())
                .Build();
        }

        public async Task<IManagedMqttClient> Setup(IManagedMqttClientOptions? options)
        {
            var mqttClient = new MqttFactory().CreateManagedMqttClient();
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("#").Build());

           
            await mqttClient.StartAsync(options);
            return  mqttClient;
        }
    }
}
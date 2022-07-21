using MQTTnet;
using System.Text;
using System.Globalization;

namespace CollapsedLight.WebApp.Helper
{
    public static class MqttHelper
    {
        public static double ParsePayloadDouble(this MqttApplicationMessage message)
        {
            var payload = message.ConvertPayloadToString();
            double.TryParse(payload, NumberStyles.Any, CultureInfo.InvariantCulture, out var val);
            return val;
        }

        public static int ParsePayloadInt(this MqttApplicationMessage message)
        {
            var payload = message.ConvertPayloadToString();
            int.TryParse(payload, NumberStyles.Any, CultureInfo.InvariantCulture, out var val);
            return val;
        }


        //public static async ValueTask SubscribeTopic(this MqttApplicationMessage message, string topic, Func<double, ValueTask> callback)
        //{
        //    if (message.Topic == topic)
        //        await callback(message.ParsePayloadDouble()).ConfigureAwait(false);
        //}
        
        public static void SubscribeToDouble(this MqttApplicationMessage message, string topic, Action<double> callback)
        {
            if (message.Topic == topic)
                callback(message.ParsePayloadDouble());
        } 
        
        public static void SubscribeToInt(this MqttApplicationMessage message, string topic, Action<int> callback)
        {
            if (message.Topic == topic)
                callback(message.ParsePayloadInt());
        }

    }

}

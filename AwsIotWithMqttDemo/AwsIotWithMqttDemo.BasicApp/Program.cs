using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace AwsIotWithMqttDemo.BasicApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var broker = "a2upl3efg7dvex-ats.iot.eu-central-1.amazonaws.com"; //<AWS-IoT-Endpoint>           
            var port = 8883;
            var clientId = "TestDeviceWeb";
            var certPass = "<enterPassword>";

            //certificates Path
            var certificatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "certs");

            var caCertPath = Path.Combine(certificatesPath, "AmazonRootCA1.pem");
            var caCert = X509Certificate.CreateFromCertFile(caCertPath);

            var deviceCertPath = Path.Combine(certificatesPath, "certificate.cert.pfx");
            var deviceCert = new X509Certificate(deviceCertPath, certPass);

            // Create a new MQTT client.
            var client = new MqttClient(broker, port, true, caCert, deviceCert, MqttSslProtocols.TLSv1_2);

            //Event Handler Wiring
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            client.MqttMsgSubscribed += Client_MqttMsgSubscribed;

            //Connect
            client.Connect(clientId);
            Console.WriteLine($"Connected to AWS IoT with client id: {clientId}.");

            //send Messages
            var message = "Hello from .NET Core";
            int i = 1;
            while (i <= 3)
            {
                client.Publish("topic_2", Encoding.UTF8.GetBytes($"{message} {i}"));
                Console.WriteLine($"Published: {message} {i}");
                i++;
                Thread.Sleep(2000);
            }
            Console.WriteLine($"Messages Sent Complete.");

            //subscribig to topic
            string topic = "topic_1";
            client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

            Console.WriteLine($"Press any key to exit!");
            Console.ReadLine();
        }

        private static void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine($"Successfully subscribed to the AWS IoT topic.");
        }

        private static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("Message received: " + Encoding.UTF8.GetString(e.Message));
        }
    }
}

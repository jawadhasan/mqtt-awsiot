using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;

namespace AwsIotWithMqttDemo.BasicApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var broker = "a2upl3efg7dvex-ats.iot.eu-central-1.amazonaws.com"; //<AWS-IoT-Endpoint>           
            var port = 8883;
            var clientId = "TestDeviceWeb";
            var certPass = "<EnterPassword>";

            //certificates Path
            var certificatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "certs");

            var caCertPath = Path.Combine(certificatesPath, "AmazonRootCA1.pem");
            var caCert = X509Certificate.CreateFromCertFile(caCertPath);

            var deviceCertPath = Path.Combine(certificatesPath, "certificate.cert.pfx");
            var deviceCert = new X509Certificate(deviceCertPath, certPass);

            // Create a new MQTT client.
            var client = new MqttClient(broker, port, true, caCert, deviceCert, MqttSslProtocols.TLSv1_2);

            //Connect
            client.Connect(clientId);
            Console.WriteLine($"Connected to AWS IoT with client id: {clientId}.");

            //send Messages
            var message = "Hello from .NET Core";
            int i = 1;
            while (i <= 10)
            {
                client.Publish("topic_2", Encoding.UTF8.GetBytes($"{message} {i}"));
                Console.WriteLine($"Published: {message} {i}");
                i++;
                Thread.Sleep(2000);
            }

            Console.WriteLine($"Messages Sent Complete. Press any key to exit!");
            Console.ReadLine();

        }
    }
}

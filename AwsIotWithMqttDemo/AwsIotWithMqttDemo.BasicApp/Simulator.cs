using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;

namespace AwsIotWithMqttDemo.BasicApp
{
    public class Simulator
    {
        private readonly Coordinates _initialCooridnates;
        private readonly Coordinates _endingCoordinates;

        //ctor
        public Simulator()
        {
            _initialCooridnates = new Coordinates
            {
                Lat = 46.6314609,
                Lon = -99.3446777
            };
            _endingCoordinates = new Coordinates
            {
                Lat = 46.6302106,
                Lon = -96.8319174
            };
        }
        public void PublishToAWSIoT(MqttClient client)
        {
            var rideId = Math.Floor(new Random().NextDouble() * 1000);
            var counter = 0;
            var driveTime = 2 * 60; // 2 hours in minutes
            var endTime = DateTime.UtcNow;

            while (counter < driveTime)
            {
                counter++;

                Console.WriteLine($"Sending message {counter}");

                //slightly different from NodeJS example
                var newLat = _initialCooridnates.Lat + (_endingCoordinates.Lat - _initialCooridnates.Lat) * (driveTime / counter);
                var newLon = _initialCooridnates.Lon + (_endingCoordinates.Lon - _initialCooridnates.Lon) * (driveTime / counter);

                var currentCoordinates = new Coordinates
                {
                    Lat = newLat,
                    Lon = newLon
                };
                Console.WriteLine($"Lat {currentCoordinates.Lat}: Lon {currentCoordinates.Lon}");

                // ts: Math.floor(new Date(endTime.getTime() - (driveTime - counter) * 60 * 1000).getTime()),                
                var nd = endTime.Ticks - (driveTime - counter) * 60 * 1000;
                var ts = new DateTime(nd).Ticks;

                var data = new TruckData
                {
                    RideId = rideId,
                    Temperature = 77.2 + 0.02 * counter,
                    Ts = ts,
                    Lat = currentCoordinates.Lat,
                    Lon = currentCoordinates.Lon
                };

                //prepare json
                var jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                //publish to AWS IoT Core
                client.Publish("truck_sensor", Encoding.UTF8.GetBytes(jsonData));

                Thread.Sleep(500);

            }
        }
    }

    public class TruckData
    {
        public double RideId { get; set; }
        public double Temperature { get; set; }
        public long Ts { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
    public class Coordinates
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}

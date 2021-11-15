using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Problem2.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PriceController : ControllerBase
    {

        //[HttpGet]
        //public void Get(Bitcoin greeting)
        //{
        //    var URL = new Uri("https://api.coindesk.com/v1/bpi/currentprice.json");

        //    using (var httpClient = new HttpClient())
        //    {
        //        var json = httpClient.GetStringAsync("https://api.coindesk.com/v1/bpi/currentprice.json");

        //        // Now parse with JSON.Net
        //    }
        //    return;
        //}
        [HttpGet]
        public HttpResponseMessage GetPriceEvery15()
        {
            while (true)
            {
                var timer = new System.Threading.Timer(
                e => GetPrice(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(15));
            }
        }

        [HttpGet]
        public string GetPrice()
        {
            WebClient client = new WebClient();

            // Add a user agent header in case the 
            // requested URI contains a query.

            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            Stream data = client.OpenRead("https://api.coindesk.com/v1/bpi/currentprice.json");
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 31091
                //HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                //Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))
            };

            Console.WriteLine(factory.HostName + ":" + factory.Port);
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "info",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string btcinfo = s;
                var body = Encoding.UTF8.GetBytes(btcinfo);

                channel.BasicPublish(exchange: "",
                                     routingKey: "info",
                                     basicProperties: null,
                                     body: body);
            }

            return "ok";
        }



        //[HttpPost]
        //public void Post([FromBody] Bitcoin greeting)
        //{
        //    var factory = new ConnectionFactory()
        //    {
        //        HostName = "localhost",
        //        Port = 31091
        //        //HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
        //        //Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))
        //    };

        //    Console.WriteLine(factory.HostName + ":" + factory.Port);
        //    using (var connection = factory.CreateConnection())
        //    using (var channel = connection.CreateModel())
        //    {
        //        channel.QueueDeclare(queue: "greetings",
        //                             durable: false,
        //                             exclusive: false,
        //                             autoDelete: false,
        //                             arguments: null);

        //        //string message = greeting.Greet;
        //        //var body = Encoding.UTF8.GetBytes(/*message*/);

        //        //channel.BasicPublish(exchange: "",
        //        //                     routingKey: "greetings",
        //        //                     basicProperties: null,
        //        //                     body: body);
        //    }
        //}
    }
}



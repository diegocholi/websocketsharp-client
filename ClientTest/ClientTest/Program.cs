using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WebSocketSharp;

namespace ClientTest
{
    class Program
    {
        public static WebSocket Ws;
        static void Main(string[] args)
        {

            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "nome", "c# Application" },
                { "mensagem", "Hello, i am c#" }
            };

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            Ws = new WebSocket("ws://localhost:9990/chat");
            Ws.OnMessage += (sender, e) =>
            {
                Console.WriteLine("Message received from "+((WebSocket) sender).Url+" Data: " + e.Data);
            };
            Ws.Connect();
            if (Ws != null)
                Ws.Send(json);

        }
    }
}

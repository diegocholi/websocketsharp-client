using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WebSocketSharp; // LIB: WebSocketSharp-netstandard

namespace ClientTest
{
    class Program
    {
        public static WebSocket Ws;
        static void Main(string[] args)
        {
            // Inicio da conexão com WS
            Ws = new WebSocket("ws://localhost:8090/groups?authorization=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwOlwvXC8xOTIuMTY4LjEuNDo4MDgwXC9hcGlcL3YxLjAuMFwvbG9naW4iLCJpYXQiOjE2NDI3ODQ1MTUsImV4cCI6MTg4Mzc0NDUxNSwibmJmIjoxNjQyNzg0NTE1LCJqdGkiOiJmVlN0cmNoamRwUjE3ZjVqIiwic3ViIjo4LCJwcnYiOiIyM2JkNWM4OTQ5ZjYwMGFkYjM5ZTcwMWM0MDA4NzJkYjdhNTk3NmY3In0.GZhIxSWxewHu0c1V0iTcyTm7duEZQv7Fv511lBRcRis");
            Ws.Connect();
            
            // Observer que recebe as mensagens
            Ws.OnMessage += (sender, e) =>
            {
                Console.WriteLine("Message received from "+((WebSocket) sender).Url+" Data: " + e.Data);
            };

            // Envio de mensagem com params
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "name", "c# Application" },
                { "message", "Hello, i am c#" },
                { "groupId", "1" },
                { "token", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwOlwvXC8xOTIuMTY4LjEuNDo4MDgwXC9hcGlcL3YxLjAuMFwvbG9naW4iLCJpYXQiOjE2NDI3ODQ1MTUsImV4cCI6MTg4Mzc0NDUxNSwibmJmIjoxNjQyNzg0NTE1LCJqdGkiOiJmVlN0cmNoamRwUjE3ZjVqIiwic3ViIjo4LCJwcnYiOiIyM2JkNWM4OTQ5ZjYwMGFkYjM5ZTcwMWM0MDA4NzJkYjdhNTk3NmY3In0.GZhIxSWxewHu0c1V0iTcyTm7duEZQv7Fv511lBRcRis"},
            };

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            if (Ws != null)
                Ws.Send(json);

        }
    }
}

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
            Ws.OnMessage += ObserverWs;

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
        
        private void ObserverWs(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Message received from "+((WebSocket) sender).Url+" Data: " + e.Data);
        }
    }
    
    // Classe aprimorada que implementa o WebSocketSharp
    class WebsocketClient
    {
        public static WebSocket Ws = null;
        private static Dictionary<string, EventHandler<MessageEventArgs>> Observers = new Dictionary<string, EventHandler<MessageEventArgs>>();
        
        // Chamar o método estático "InstanceVerify" para instânciar a classe
        public WebsocketClient(string url = "")
        {
            Ws = new WebSocket(url);
            Ws.Connect();
        }
        
        // Método que envia mensagem
        public async static void SendMessage(int groupId, string message)
        {
            try
            {
                string tokenGroups = await PersistHybridData.GetData("groups_access_token");
                Dictionary<string, string> data = new Dictionary<string, string>()
                {
                    { "name", "c# Application" },
                    { "message", message },
                    { "groupId", groupId.ToString() },
                    { "token", tokenGroups.Split("Bearer ")[1] },
                };

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);

                if (Ws != null)
                    Ws.Send(json);
            }
            catch (HttpRequestException ex)
            {
                SuccessfulAnswerUtil.retrieveSuccessfulAnswer(ex);
            }
            catch (SwaggerException ex)
            {
                SuccessfulAnswerUtil.retrieveSuccessfulAnswer(ex);
            }
        }

        // Adiciona eventos, 
        // No primeiro parâmetro passar o evento criado como no exemplo da classe Program acima: ObserverWs 
        // No segundo parâmetro a KEY que ira manter o evento na memória da aplicação: "KEY_EVENTO"
        // EXEMPLO: WebsocketClient.AddObserver(ObserverWs, "Evento_1")
        public static void AddObserver(EventHandler<MessageEventArgs> eventHandler, string keyObserver)
        {
            if (IsConnected())
            {
                Observers.Add(keyObserver, eventHandler);
                Ws.OnMessage += Observers[keyObserver];
            }
        }
        
        // Após o uso do observer por boa prática é sempre bom remove-lo da pilha de execução: WebsocketClient.RemoveObserver("Evento_1")
        public static void RemoveObserver(string keyObserver)
        {
            if (IsConnected())
            {
                Ws.OnMessage -= Observers[keyObserver];
                Observers.Remove(keyObserver);
            }
        }

        public static void Connect()
        {
            Ws.Connect();
        }

        // Método que verifica se a instância está incializada e inicia caso não esteja
        public async static void InstanceVerify()
        {
            string tokenGroups = await PersistHybridData.GetData("groups_access_token");
            if (!string.IsNullOrEmpty(tokenGroups) && !IsConnected())
            {
                new WebsocketClient(Configuration.WsGroups + "?authorization=" + tokenGroups.Split("Bearer ")[1]);
            }
        }
        
        // Verificação de conexão
        public static bool IsConnected()
        {
            if (Ws != null)
                return true;
            else return false;
        }
    }
}

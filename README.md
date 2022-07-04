# websocketsharp-client
WebSocketSharp - LIB: WebSocketSharp-netstandard

```
using MeuSebrae.Helpers;
using MeuSebrae.Shared;
using MeuSebrae.Shared.Models;
using MeuSebrae.Shared.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using WebSocketSharp;

namespace MeuSebrae.Utils
{
    class WebsocketClient
    {
        public static WebSocket Ws = null;
        private static Dictionary<string, EventHandler<MessageEventArgs>> Observers = new Dictionary<string, EventHandler<MessageEventArgs>>();

        // Chamar o método estático "InstanceVerify" para instânciar a classe
        public WebsocketClient(string url = "")
        {
            Ws = new WebSocket(url);
            Ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;  // Configuração SSL
            Ws.Connect();
        }

        // Método que envia mensagem
        public async static void SendMessage(int groupId, string message, StorageFileInfo file = null)
        {
            try
            {
                string fileJsonConvert = null;
                if (file != null)
                    fileJsonConvert = JsonConvert.SerializeObject(file, Formatting.Indented);

                string tokenGroups = await PersistHybridData.GetData("groups_access_token");
                Dictionary<string, string> data = new Dictionary<string, string>()
                {
                    { "name", ConstantsHelper.CurrentUser.Usuario.Name },
                    { "message", message },
                    { "groupId", groupId.ToString() },
                    { "file", fileJsonConvert },
                    { "token", tokenGroups.Split("Bearer ")[1] },
                };

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);

                if (IsConnected())
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
        // No primeiro parâmetro passar o evento criado
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
            if (IsConnected() && Observers.Count > 0)
            {
                Ws.OnMessage -= Observers[keyObserver];
                Observers.Remove(keyObserver);
            }
        }

        public static void ClearObservers()
        {
            if (IsConnected() && Observers.Count > 0)
            {
                foreach (KeyValuePair<string, EventHandler<MessageEventArgs>> observer in Observers)
                {
                    Ws.OnMessage -= observer.Value;
                }
                Observers.Clear();
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
            // Reconectar no websocket caso ele não estiver em um tempo de vida válido e estar instanciado
            if (Ws != null && !Ws.IsAlive) 
            {
                Connect();
                return true;
            }
            
            // Se o websocket está instanciado e ainda dentro do tempo de vida dele
            if (Ws != null && Ws.IsAlive)
                return true;
            
            // Se o websocket não estiver instanciado e fora do seu tempo de vida, retorna false
            return false;
        }
    }
}
```

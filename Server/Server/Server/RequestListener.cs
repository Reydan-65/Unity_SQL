using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;

namespace Server
{
    public class RequestListener
    {
        private PlayerList playerList;
        private HttpListener listener;
        private ResponseCollection responseCollection;

        public RequestListener(PlayerList playerList, ResponseCollection responseCollection, string prefix)
        {
            this.playerList = playerList;
            this.responseCollection = responseCollection;

            listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();
            listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
        }

        public async void StartRequestListener()
        {
            listener.Start();

            while (true)
            {
                // Получение запроса
                HttpListenerContext context = await listener.GetContextAsync();

                HttpListenerBasicIdentity identity = (HttpListenerBasicIdentity)context.User.Identity;

                PlayerInfo playerInfo = new PlayerInfo(identity.Name, identity.Password);

                if (playerList.ExistPlayer(playerInfo) == false)
                {
                    Console.WriteLine($"Request was recieved from a user {identity.Name} who is not in the database");
                    continue;
                }

                Console.WriteLine($"REQUEST: USER: {identity.Name}; METHOD: {context.Request.HttpMethod}; URL: {context.Request.RawUrl};");

                if (context.Request.HttpMethod == "GET")
                {
                    string responseMessage = responseCollection.GetResponseForGET(context.Request.RawUrl, playerInfo);
                    
                    if (responseMessage != "")
                        SendResponseAsync(context.Response, playerInfo, responseMessage);

                    await context.Response.OutputStream.FlushAsync();
                }

                if (context.Request.HttpMethod == "POST")
                {
                    string content = "";
                    StreamReader inputStream = new StreamReader(context.Request.InputStream);
                    content = inputStream.ReadToEnd();
                    Console.WriteLine($"{content}");

                    string responseMessage = responseCollection.ResponseForPOST(context.Request.RawUrl, content, playerInfo);
                    
                    if (responseMessage != "")
                        SendResponseAsync(context.Response, playerInfo, responseMessage);

                    await context.Response.OutputStream.FlushAsync();
                }
            }
        }

        public async void SendResponseAsync(HttpListenerResponse response, PlayerInfo playerInfo, string responseText)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(responseText);

            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);

            Console.WriteLine($"RESPONSE: USER: {playerInfo.Name}; METHOD: POST; CONTENT: {responseText};");
        }
    }
}

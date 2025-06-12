using System.IO;
using System.Net;
using System.Text;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

                if (context.Request.RawUrl == "/register" && context.Request.HttpMethod == "POST")
                {
                    await HandleRegistration(context);
                    continue;
                }

                PlayerInfo playerInfo = new PlayerInfo(identity.Name, identity.Password);

                if (!playerList.ExistPlayer(playerInfo))
                {
                    DebugViewer.Write("ABORT: ", ConsoleColor.Red);
                    DebugViewer.WriteLine($"Request was received from a user {playerInfo.Name} who is not in the database", ConsoleColor.DarkRed);

                    SendErrorResponse(context.Response, "User not found");
                    continue;
                }

                DebugViewer.Write($"REQUEST: ", ConsoleColor.Yellow);
                DebugViewer.WriteLine($"USER: {identity.Name}; METHOD: {context.Request.HttpMethod}; URL: {context.Request.RawUrl};", ConsoleColor.DarkYellow);
                
                await RequestMethodGET(context, playerInfo);
                await RequestMethodPOST(context, playerInfo);
            }
        }

        private async Task RequestMethodPOST(HttpListenerContext context, PlayerInfo playerInfo)
        {
            if (context.Request.HttpMethod == "POST")
            {
                string content = "";
                StreamReader inputStream = new StreamReader(context.Request.InputStream);
                content = inputStream.ReadToEnd();

                DebugViewer.WriteLine($"{content}", ConsoleColor.DarkCyan);

                string responseMessage = responseCollection.ResponseForPOST(context.Request.RawUrl, content, playerInfo);

                if (responseMessage != "")
                    SendResponseAsync(context.Response, playerInfo, responseMessage);

                await context.Response.OutputStream.FlushAsync();
            }
        }

        private async Task RequestMethodGET(HttpListenerContext context, PlayerInfo playerInfo)
        {
            if (context.Request.HttpMethod == "GET")
            {
                string responseMessage = responseCollection.GetResponseForGET(context.Request.RawUrl, playerInfo);

                if (responseMessage != "")
                    SendResponseAsync(context.Response, playerInfo, responseMessage);

                await context.Response.OutputStream.FlushAsync();
            }
        }


        public async void SendResponseAsync(HttpListenerResponse response, PlayerInfo playerInfo, string responseText)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(responseText);

            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);

            DebugViewer.Write($"RESPONSE: ", ConsoleColor.Yellow);
            DebugViewer.WriteLine($"USER: {playerInfo.Name}; METHOD: POST; CONTENT: {responseText};", ConsoleColor.DarkYellow);
        }

        private async Task HandleRegistration(HttpListenerContext context)
        {
            try
            {
                StreamReader inputStream = new StreamReader(context.Request.InputStream);
                string content = await inputStream.ReadToEndAsync();

                DebugViewer.WriteLine($"Registration attempt: {content}", ConsoleColor.Green);

                var data = JsonConvert.DeserializeObject<AuthRequest>(content);

                if (data == null || string.IsNullOrEmpty(data.login) || string.IsNullOrEmpty(data.password))
                {
                    SendErrorResponse(context.Response, "Invalid registration data");
                    return;
                }

                if (playerList.ExistPlayer(new PlayerInfo(data.login, "")))
                {
                    SendErrorResponse(context.Response, "Username already taken");
                    return;
                }

                playerList.AddNewPlayer(new PlayerInfo(data.login, data.password));
                SendResponseAsync(context.Response, new PlayerInfo(data.login, data.password), "{\"success\":true}");
            }
            catch (Exception ex)
            {
                DebugViewer.Write($"ABORT: ", ConsoleColor.Red);
                DebugViewer.WriteLine($"Registration error: {ex.Message}", ConsoleColor.DarkRed);

                SendErrorResponse(context.Response, "Registration failed");
            }
        }

        private void SendErrorResponse(HttpListenerResponse response, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes($"{{\"success\":false, \"message\":\"{message}\"}}");
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }
    }
}
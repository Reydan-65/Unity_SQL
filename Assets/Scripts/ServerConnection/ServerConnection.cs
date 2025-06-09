using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ServerConnection : MonoBehaviour
{
    [SerializeField] private string baseIP = "http://192.168.1.100:88";
    [SerializeField] private PlayerInfo playerInfo;

    public async Task<string> RequestAsync(string resource, string method, string content = "")
    {
        // Отправляем запрос
        WebRequest request = WebRequest.Create(baseIP + resource);
        request.Method = method;
        request.UseDefaultCredentials = true;
        request.PreAuthenticate = true;
        request.Credentials = new NetworkCredential(playerInfo.Name, playerInfo.GetPasswordHash());

        if (method == "POST")
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            Stream requestStream = await request.GetRequestStreamAsync();
            await requestStream.WriteAsync(bytes, 0, bytes.Length);
        }

        // Получаем ответ
        string responseText = "";

        try
        {
            WebResponse response = await request.GetResponseAsync();
            StreamReader stream = new StreamReader(response.GetResponseStream());
            responseText = await stream.ReadToEndAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return responseText;
    }
}

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class RegisterResponse
{
    public bool success { get; set; }
}

public class ServerConnection : MonoBehaviour
{
    public static ServerConnection Instance;

    [SerializeField] private string baseIP = "http://192.168.1.100:88";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async Task<string> RequestAsync(string resource, string method, string content = "",
                                           string login = "", string password = "")
    {
        // Отправляем запрос
        WebRequest request = WebRequest.Create(baseIP + resource);
        request.Method = method;
        request.UseDefaultCredentials = true;
        request.PreAuthenticate = true;

        string user = !string.IsNullOrEmpty(login) ? login : PlayerInfo.Instance?.Name;
        string pass = !string.IsNullOrEmpty(password) ? password : PlayerInfo.Instance?.Password;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass)) return "";

        PlayerInfo.Instance.Init(user, pass);

        request.Credentials = new NetworkCredential(user, pass);

        if (method == "POST" && !string.IsNullOrEmpty(content))
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

    public async Task<bool> RegisterAsync(string username, string password)
    {
        string clientHashedPassword = PlayerInfo.Instance?.GetPasswordHash(password);

        AuthRequest authRequest = new AuthRequest
        {
            Login = username,
            Password = clientHashedPassword
        };

        string content = JsonUtility.ToJson(authRequest);
        string response = await RequestAsync("/register", "POST", content);

        if (!string.IsNullOrEmpty(response))
        {
            try
            {
                var result = JsonConvert.DeserializeObject<RegisterResponse>(response);
                return result.success;
            }
            catch (Exception e)
            {
                Debug.LogError($"Deserialization error: {e}");
                return false;
            }
        }
        return false;
    }
}
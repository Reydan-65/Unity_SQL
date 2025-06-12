using System.Net;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
internal class AuthRequest
{
    public string Login;
    public string Password;
}

[System.Serializable]
internal class AuthResponse
{
    public bool success;
    public string message;
}

public class UILoginPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _loginField;
    [SerializeField] private TMP_InputField _passwordField;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _registerButton;
    [SerializeField] private Button _exitButton;

    private void Start()
    {
        _loginButton.onClick.AddListener(OnLoginClicked);
        _registerButton.onClick.AddListener(OnRegistrationClicked);
        _exitButton.onClick.AddListener(OnExitClicked);
        ShowStatus("");
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }

    private async void OnLoginClicked()
    {
        ShowStatus("");
        await TryAuthProcess(isLogin: true);
    }

    private async void OnRegistrationClicked()
    {
        string login = _loginField.text;
        string password = _passwordField.text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            _statusText.text = "Username and password required";
            return;
        }

        _registerButton.interactable = false;
        _loginButton.interactable = false;

        _statusText.text = "Registering...";

        await Task.Yield();

        bool success = await ServerConnection.Instance.RegisterAsync(login, password);

        if (success)
            _statusText.text = "Registration successful!";
        else
            _statusText.text = "Registration failed - username may be taken";

        _registerButton.interactable = true;
        _loginButton.interactable = true;
    }

    private async Task TryAuthProcess(bool isLogin)
    {
        string login = _loginField.text;
        string rawPassword = _passwordField.text;
        string hashedPassword = PlayerInfo.Instance.GetPasswordHash(rawPassword);
        string content = JsonUtility.ToJson(new AuthRequest{Login = login, Password = hashedPassword});
        string response = await ServerConnection.Instance.RequestAsync(resource: "/auth", method: "POST", content: content, login: login, password: hashedPassword);

        if (string.IsNullOrEmpty(response))
        {
            ShowStatus("Empty server response");
            return;
        }

        AuthResponse responseData;
        try
        {
            responseData = JsonUtility.FromJson<AuthResponse>(response);
        }
        catch (System.Exception e)
        {
            ShowStatus(e is WebException ? "Connection error" : "Operation failed");
            return;
        }

        if (responseData == null)
        {
            ShowStatus("Server error: invalid response format");
            return;
        }

        if (responseData.success)
        {
            ShowStatus("");
            SceneManager.LoadScene("level_01");
        }
        else
        {
            ShowStatus(responseData.message ?? "Authentication failed");
        }
    }

    private void ShowStatus(string message)
    {
        _statusText.text = message;
    }
}

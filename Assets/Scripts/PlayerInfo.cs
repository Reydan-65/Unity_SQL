using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private new string name;
    [SerializeField] private string password;

    public string Name => name;
    public string Password => password;

    public string GetPasswordHash()
    {
        StringBuilder sb = new StringBuilder();

        using (SHA256  sha256 = SHA256.Create())
        {
            byte[] hasValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            foreach (byte b in hasValue)
            {
                sb.Append($"{b:X2}");
            }
        }

        Debug.Log(sb.ToString());

        return sb.ToString();
    }
}

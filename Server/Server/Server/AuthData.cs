namespace Server
{
    [System.Serializable]
    public class AuthRequest
    {
        public string login;
        public string password;
    }

    [System.Serializable]
    public class AuthResponse
    {
        public bool success;
        public string message;
    }
}
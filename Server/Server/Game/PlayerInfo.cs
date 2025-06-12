namespace Server
{
    public class PlayerInfo
    {
        private string name;
        private string passwordHash;

        public string Name => name;
        public string PasswordHash => passwordHash;

        public PlayerInfo(string name, string passwordHash)
        {
            this.name = name;
            this.passwordHash = passwordHash;
        }
    }
}

namespace Server
{
    public class Game
    {
        private PlayerList playerList;

        public Game(PlayerList playerList)
        {
            this.playerList = playerList;
        }

        public void UpdateGame()
        {
            playerList.UpdatePlayerStats();

            System.Threading.Thread.Sleep(1000);
        }
    }
}

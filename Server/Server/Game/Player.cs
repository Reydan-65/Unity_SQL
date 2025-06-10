namespace Server
{
    public class Player
    {
        public PlayerInfo Info;
        public PlayerStats Stats;

        public Player(PlayerInfo info)
        {
            Info = info;
            Stats = new PlayerStats(10, 1);
        }

        public Player(PlayerInfo info, PlayerStats stats)
        {
            Info = info;
            Stats = stats;
        }
    }
}

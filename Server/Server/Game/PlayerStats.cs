using System;

namespace Server
{
    [Serializable]
    public class PlayerStats
    {
        public int Gold { get; set; }
        public int Level { get; set; }

        public int GoldPerClick => 1 * Level;

        public PlayerStats(int gold, int level)
        {
            Gold = gold;
            Level = level;
        }

        public void Update()
        {
            Gold += Level;
        }

        public void NextLevel()
        {
            if (Gold >= 10 * Level)
            {
                GetGold (-10 * Level);
                Level++;
            }
        }

        public void GetGold(int gold)
        {
            Gold += gold;
        }

        public void MineGold()
        {
            Gold += GoldPerClick;
        }
    }
}

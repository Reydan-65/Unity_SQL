using System;
using System.Collections.Generic;

namespace Server
{
    public class PlayerList
    {
        private List<Player> players = new List<Player>();

        public void AddNewPlayer(PlayerInfo playerInfo)
        { 
            if (ExistPlayer(playerInfo) == false)
            {
                players.Add(new Player(playerInfo));
                Console.WriteLine($"Добавили игрока: {playerInfo.Name}");
            }
        }

        public bool ExistPlayer(PlayerInfo playerInfo)
        { 
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Info.Name == playerInfo.Name && 
                    players[i].Info.PasswordHash == playerInfo.PasswordHash) return true;
            }

            return false;
        }

        public void UpdatePlayerStats()
        {
            for (int i = 0;i < players.Count;i++)
            {
                players[i].Stats.Update();
            }
        }

        public PlayerStats GetPlayerStats(PlayerInfo playerInfo)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Info.Name == playerInfo.Name)
                    return players[i].Stats;
            }

            return null;
        }
    }
}

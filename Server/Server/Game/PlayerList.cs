using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public class PlayerList
    {
        private List<Player> players = new List<Player>();
        private DBPlayerRequestCollection collection;

        public int Count => players.Count;

        public Player this[int index]
        {
            get => players[index];
            //set => players[index] = value;
        }

        public PlayerList(DBPlayerRequestCollection collection)
        {
            this.collection = collection;
        }

        public void AddPlayers(List<Player> players)
        {
            this.players.AddRange(players);
        }

        public void AddNewPlayer(PlayerInfo playerInfo)
        {
            if (ExistPlayer(playerInfo) == false)
            {
                if (collection.CreatePlayer(playerInfo))
                {
                    players.Add(new Player(playerInfo));
                    DebugViewer.Write($"SUCCESS: ", ConsoleColor.Green);
                    DebugViewer.WriteLine($"Registered new User: {playerInfo.Name}", ConsoleColor.DarkGreen);
                }
                else
                {
                    DebugViewer.Write($"ABORT: ", ConsoleColor.Red);
                    DebugViewer.WriteLine($"Failed to create new User {playerInfo.Name} in database!", ConsoleColor.DarkRed);
                }
            }
            else
            {
                DebugViewer.Write($"ABORT: ", ConsoleColor.Red);
                DebugViewer.WriteLine($"{playerInfo.Name} already exists in database!", ConsoleColor.DarkRed);
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
            for (int i = 0; i < players.Count; i++)
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

        public Player GetPlayerByName(string name)
        {
            return players.FirstOrDefault(p => p.Info.Name == name);
        }
    }
}

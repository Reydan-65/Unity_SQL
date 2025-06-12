using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Server
{
    public class DBPlayerRequestCollection
    {
        private DBConnection connection;

        public DBPlayerRequestCollection(DBConnection connection)
        {
            this.connection = connection;
        }

        public void SetPlayerStats(Player player)
        {
            int playerID = GetPlayerID(player.Info);

            string commandText = $"UPDATE player_stats SET gold={player.Stats.Gold}, level={player.Stats.Level} WHERE player_id=\"{playerID}\"";

            connection.ExecuteCommand(commandText);
        }

        private int GetPlayerID(PlayerInfo info)
        {
            string commandText = $"SELECT id FROM player_info WHERE name = \"{info.Name}\"";

            SQLiteDataReader reader = connection.ExecuteCommandWithResult(commandText);

            if (reader.HasRows == true)
            {
                reader.Read();
                return reader.GetInt32(0);
            }

            return -1;
        }

        public List<Player> GetAllPlayers()
        {
            List<Player> allPlayers = new List<Player>();

            PlayerInfo[] info = GetAllPlayerInfo();
            PlayerStats[] stats = GetAllPlayerStats();

            DebugViewer.WriteLine($"{"Login",-15} {"Password Hash",-64}", ConsoleColor.DarkCyan);

            for (int i = 0; i < info.Length; i++)
            {
                allPlayers.Add(new Player(info[i], stats[i]));
                DebugViewer.WriteLine($"{info[i].Name,-15} {info[i].PasswordHash,-64}", ConsoleColor.DarkGreen);
            }
            Console.WriteLine();

            return allPlayers;
        }

        public PlayerInfo[] GetAllPlayerInfo()
        {
            List<PlayerInfo> info = new List<PlayerInfo>();
            string commandText = $"SELECT * FROM player_info";

            SQLiteDataReader reader = connection.ExecuteCommandWithResult(commandText);

            if (reader.HasRows == true)
            {
                while (reader.Read())
                {
                    string name = reader.GetValue(1).ToString();
                    string passwordHash = reader.GetValue(2).ToString();

                    info.Add(new PlayerInfo(name, passwordHash));
                }
            }

            return info.ToArray();
        }

        public PlayerStats[] GetAllPlayerStats()
        {
            List<PlayerStats> stats = new List<PlayerStats>();
            string commandText = $"SELECT * FROM player_stats";

            SQLiteDataReader reader = connection.ExecuteCommandWithResult(commandText);

            if (reader.HasRows == true)
            {
                while (reader.Read())
                {
                    int gold = reader.GetInt32(1);
                    int level = reader.GetInt32(2);

                    stats.Add(new PlayerStats(gold, level));
                }
            }

            return stats.ToArray();
        }

        public bool CreatePlayer(PlayerInfo info)
        {
            try
            {
                string commandText = $@"INSERT INTO player_info (name, password_hash)
                                        VALUES ('{info.Name}', '{info.PasswordHash}');
                                        INSERT INTO player_stats (player_id, gold, level)
                                        VALUES (last_insert_rowid(), 0, 1);";

                connection.ExecuteCommand(commandText);
                return true;
            }
            catch (SQLiteException e)
            {
                DebugViewer.Write("ABORT: ", ConsoleColor.Red);
                DebugViewer.WriteLine(e.Message, ConsoleColor.DarkRed);
                return false;
            }
        }
    }
}

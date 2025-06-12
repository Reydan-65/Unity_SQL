using System;
using System.Threading;

namespace Server
{
    public class DBPlayerSynchronizer
    {
        private DBPlayerRequestCollection collection;
        private PlayerList playerList;
        private int timeOut;

        public DBPlayerSynchronizer(DBPlayerRequestCollection collection, PlayerList playerList, int timeOut)
        {
            this.collection = collection;
            this.playerList = playerList;
            this.timeOut = timeOut;
        }

        public void StartSynchronize()
        {
            while (true)
            {
                for (int i = 0; i < playerList.Count;  i++)
                {
                    collection.SetPlayerStats(playerList[i]); // PlayerList проиндексирован (стр. 14)
                }

                DebugViewer.WriteLine("Player stats synchronized with the database", ConsoleColor.DarkGray);

                Thread.Sleep(timeOut);
            }
        }
    }
}

using System;
using System.Threading;

namespace Server
{
    class Application
    {
        static void Main(string[] args)
        {
            // Подключение к базе данных
            DBConnection connection = new DBConnection();
            DBPlayerRequestCollection collection = new DBPlayerRequestCollection(connection);
            connection.Open();

            // Получение игроков
            PlayerList playerList = new PlayerList();

            playerList.AddPlayers(collection.GetAllPlayers());
            Console.WriteLine($"List of players loaded from database");

            // Обработка запросов
            ResponseCollection responseCollection = new ResponseCollection(playerList);
            RequestListener requestListener = new RequestListener(playerList, responseCollection, "http://192.168.1.100:88/playerStats/");
            Thread requestThread = new Thread(requestListener.StartRequestListener);
            requestThread.Start();

            // Синхронизация с базой данных
            DBPlayerSynchronizer synchronizer = new DBPlayerSynchronizer(collection, playerList, 5000);
            Thread thread = new Thread(synchronizer.StartSynchronize);
            thread.Start();

            // Игровая логика
            Game game = new Game(playerList);

            while (true)
            {
                game.UpdateGame();
            }
        }
    }
}

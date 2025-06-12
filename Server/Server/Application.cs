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
            PlayerList playerList = new PlayerList(collection);
            playerList.AddPlayers(collection.GetAllPlayers());
            DebugViewer.WriteLine($"List of players loaded from database\n", ConsoleColor.Green);

            // Обработка запросов авторизации
            ResponseCollection responseAuthCollection = new ResponseCollection(playerList);
            RequestListener requestAuthListener = new RequestListener(playerList, responseAuthCollection, "http://192.168.1.100:88/auth/");
            Thread requestAuthThread = new Thread(requestAuthListener.StartRequestListener);
            requestAuthThread.Start();

            // Обработка запросов регистрации
            ResponseCollection responseRegisterCollection = new ResponseCollection(playerList);
            RequestListener requestRegisterListener = new RequestListener(playerList, responseRegisterCollection, "http://192.168.1.100:88/register/");
            Thread requestRegisterThread = new Thread(requestRegisterListener.StartRequestListener);
            requestRegisterThread.Start();

            // Обработка запросов изменения характеристик
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
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Application
    {
        static void Main(string[] args)
        {
            // Игровая логика
            PlayerList playerList = new PlayerList();

            playerList.AddNewPlayer(new PlayerInfo("Player1", "65E84BE33532FB784C48129675F9EFF3A682B27168C0EA744B2CF58EE02337C5"));
            playerList.AddNewPlayer(new PlayerInfo("Player2", "DAAAD6E5604E8E17BD9F108D91E26AFE6281DAC8FDA0091040A7A6D7BD9B43B5"));

            Game game = new Game(playerList);

            // Обработка запросов
            ResponseCollection responseCollection = new ResponseCollection(playerList);
            RequestListener requestListener = new RequestListener(playerList, responseCollection, "http://192.168.1.100:88/playerStats/");
            Thread requestThread = new Thread(requestListener.StartRequestListener);
            requestThread.Start();

            while (true)
            {
                game.UpdateGame();
            }
        }
    }
}

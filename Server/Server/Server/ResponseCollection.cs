using Newtonsoft.Json;

namespace Server
{
    public class ResponseCollection
    {
        private PlayerList playerList;

        public ResponseCollection(PlayerList playerList)
        {
            this.playerList = playerList;
        }

        public string GetResponseForGET(string resource, PlayerInfo playerInfo)
        {
            if (resource == "/playerStats") return GetSerializedPlayerStats(playerInfo);

            return "";
        }

        public string ResponseForPOST(string resource, string content, PlayerInfo playerInfo)
        {
            if (resource == "/playerStats" && content == "UpgradeLevel") return UpgradeLevel(playerInfo);

            return "";
        }

        private string GetSerializedPlayerStats(PlayerInfo playerInfo)
        {
            PlayerStats playerStats = playerList.GetPlayerStats(playerInfo);

            return JsonConvert.SerializeObject(playerStats);
        }

        private string UpgradeLevel(PlayerInfo playerInfo)
        {
            PlayerStats playerStats = playerList.GetPlayerStats(playerInfo);
            playerStats.NextLevel();

            return JsonConvert.SerializeObject(playerStats);
        }
    }
}

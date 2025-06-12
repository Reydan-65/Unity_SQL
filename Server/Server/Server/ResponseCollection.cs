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

        public string GetResponseForGET(string resource, PlayerInfo info)
        {
            if (!playerList.ExistPlayer(info))
                return JsonResponse(false, "Not authorized");

            switch (resource)
            {
                case "/playerStats":
                    return GetSerializedPlayerStats(info);

                case "/auth":
                    return JsonResponse(true, "Authorized");

                default:
                    return JsonResponse(false, "Unknown endpoint");
            }
        }

        public string ResponseForPOST(string resource, string content, PlayerInfo info)
        {
            switch (resource)
            {
                case "/auth":
                    bool userExists = playerList.GetPlayerByName(info.Name) != null;
                    if (!userExists)
                        return JsonResponse(false, "User not found");
                    if (!playerList.ExistPlayer(info))
                        return JsonResponse(false, "Invalid password");
                    return JsonResponse(true, "Authentication successful");

                case "/register":
                    if (!playerList.ExistPlayer(info))
                    {
                        playerList.AddNewPlayer(info);
                        return JsonResponse(true, $"{info.Name} - Registration successful");
                    }
                    return JsonResponse(false, "Username already exists");

                case "/playerStats" when content.StartsWith("Mine upgraded"):
                    if (!playerList.ExistPlayer(info))
                        return JsonResponse(false, $"{info.Name} - Not authorized");
                    return UpgradeLevel(info);

                case "/playerStats" when content.StartsWith("Gold added"):
                    if (!playerList.ExistPlayer(info))
                        return JsonResponse(false, $"{info.Name} - Not authorized");
                    return UpdateGold(info);

                default:
                    return JsonResponse(false, $"Invalid login {info.Name} or password {info.PasswordHash}");
            }
        }

        private string GetSerializedPlayerStats(PlayerInfo info)
        {
            PlayerStats stats = playerList.GetPlayerStats(info);

            return JsonConvert.SerializeObject(stats);
        }

        private string UpgradeLevel(PlayerInfo info)
        {
            PlayerStats stats = playerList.GetPlayerStats(info);
            stats.NextLevel();

            return JsonConvert.SerializeObject(stats);
        }

        private string UpdateGold(PlayerInfo info)
        {
            PlayerStats stats = playerList.GetPlayerStats(info);
            stats.MineGold();

            return JsonConvert.SerializeObject(stats);
        }

        private string JsonResponse(bool success, string message, object data = null)
        {
            var response = new
            {
                success,
                message,
                data
            };

            return JsonConvert.SerializeObject(response);
        }
    }
}

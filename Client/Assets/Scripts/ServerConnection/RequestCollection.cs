using System.Threading.Tasks;
using UnityEngine;

public class RequestCollection : MonoBehaviour
{
    public async Task<PlayerInfo> UpdatePlayerStatsAsync(PlayerInfo playerInfo)
    {
        string responseMessage = await ServerConnection.Instance.RequestAsync("/playerInfo", "GET");

        PlayerInfo info = GetPlayerInfo(playerInfo, responseMessage);

        return info;
    }

    public async Task<PlayerStats> UpdatePlayerStatsAsync(PlayerStats playerStats)
    {
        string responseMessage = await ServerConnection.Instance.RequestAsync("/playerStats", "GET");

        PlayerStats stats = GetPlayerStats(playerStats, responseMessage);

        return stats;
    }

    public async Task<PlayerStats> UpdateGoldAsync(PlayerStats playerStats)
    {
        string responseMessage = await ServerConnection.Instance.RequestAsync("/playerStats", "POST", $"Gold added: +{playerStats.Level} " +
                                                                                                      $"Total gold: {playerStats.Gold + playerStats.Level}");

        PlayerStats stats = GetPlayerStats(playerStats, responseMessage);

        return stats;
    }

    public async Task<PlayerStats> UpgradeLevelAsync(PlayerStats playerStats)
    {
        string responseMessage = await ServerConnection.Instance.RequestAsync("/playerStats", "POST", $"Mine upgraded to Level: {playerStats.Level + 1}");

        PlayerStats stats = GetPlayerStats(playerStats, responseMessage);

        return stats;
    }

    private static PlayerStats GetPlayerStats(PlayerStats playerStats, string responseMessage)
    {
        PlayerStats stats = playerStats;

        if (responseMessage != "")
        {
            stats = JsonUtility.FromJson<PlayerStats>(responseMessage);
        }

        return stats;
    }

    private PlayerInfo GetPlayerInfo(PlayerInfo playerInfo, string responseMessage)
    {
        PlayerInfo info = playerInfo;

        if (responseMessage != "")
        {
            info = JsonUtility.FromJson<PlayerInfo>(responseMessage);
        }

        return info;
    }
}
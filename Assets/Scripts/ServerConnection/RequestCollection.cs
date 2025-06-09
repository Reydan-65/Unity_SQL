using System.Threading.Tasks;
using UnityEngine;

public class RequestCollection : MonoBehaviour
{
    [SerializeField] private ServerConnection connection;

    public async Task<PlayerStats> UpdatePlayerStatsAsync(PlayerStats playerStats)
    {
        string responseMessage = await connection.RequestAsync("/playerStats", "GET");

        PlayerStats stats = GetPlayerStats(playerStats, responseMessage);

        return stats;
    }

    public async Task<PlayerStats> UpgradeLevel(PlayerStats playerStats)
    {
        string responseMessage = await connection.RequestAsync("/playerStats", "POST", "UpgradeLevel");
        
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
}

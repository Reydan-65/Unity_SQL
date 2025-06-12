using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private RequestCollection requestCollection;

    [SerializeField] private PlayerStats playerStats = new PlayerStats(0, 1);
    public PlayerStats PlayerStats => playerStats;

    private void Start()
    {
        StartCoroutine(UpdateCoroutine());
    }

    IEnumerator UpdateCoroutine()
    {
        UpdatePlayerStatsAsync();

        while (true)
        {
            yield return new WaitForSeconds(1);
            UpdatePlayerStatsAsync();
        }
    }

    public async void UpgradeLevel()
    {
        playerStats = await requestCollection.UpgradeLevelAsync(playerStats);
    }

    public async void UpdateGold()
    {
        playerStats = await requestCollection.UpdateGoldAsync(playerStats);
    }

    private async void UpdatePlayerStatsAsync()
    {
        playerStats = await requestCollection.UpdatePlayerStatsAsync(playerStats);
    }
}

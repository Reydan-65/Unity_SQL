using UnityEngine;

public class GoldMine : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void OnMouseDown()
    {
        if (playerController == null) return;

        playerController.UpdateGold();
    }
}

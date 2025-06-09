using TMPro;
using UnityEngine;

public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerController controller;

    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI levelText;

    private void Update()
    {
        goldText.text = controller.PlayerStats.Gold.ToString();
        levelText.text = controller.PlayerStats.Level.ToString();
    }
}

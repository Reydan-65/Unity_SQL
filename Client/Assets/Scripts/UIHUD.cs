using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    [SerializeField] private PlayerController controller;

    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private Button upgradeLevelButton;
    [SerializeField] private TextMeshProUGUI costText;

    private Color baseColor;
    private CultureInfo cultureInfo;

    private void Start()
    {
        baseColor = upgradeLevelButton.GetComponent<Image>().color;
        cultureInfo = new CultureInfo("en-US");
    }

    private void Update()
    {
        goldText.text = FormatNumber(controller.PlayerStats.Gold);
        levelText.text = controller.PlayerStats.Level.ToString();

        UpdateCost();
    }

    private string FormatNumber(int number)
    {
        return number.ToString("N0", cultureInfo).Replace(",", " ");
    }

    private void UpdateCost()
    {
        int cost = 10 * controller.PlayerStats.Level;

        costText.text = "Cost " + " " + cost.ToString();

        if (cost > controller.PlayerStats.Gold)
        {
            upgradeLevelButton.interactable = false;
            upgradeLevelButton.GetComponent<Image>().color = Color.red;
        }
        else
        {
            upgradeLevelButton.interactable = true;
            upgradeLevelButton.GetComponent<Image>().color = baseColor;
        }
    }

    public void Logout()
    {
        upgradeLevelButton.onClick.RemoveAllListeners();
        upgradeLevelButton.interactable = false;

        PlayerInfo.Instance.Init("", "");

        if (controller != null)
            controller.StopAllCoroutines();

        SceneManager.LoadScene("login_scene");
    }
}

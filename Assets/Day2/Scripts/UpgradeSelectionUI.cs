using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeSelectionUI : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public TextMeshProUGUI upgradeCount;
    public Image icon;
    UpgradeSelection upgradeSelection;
    Button button;
    public Action<UpgradeSelection> OnSelectUpgrdaeSelection;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => OnSelectUpgrdaeSelection?.Invoke(upgradeSelection));

    }

    public void SetUpgradeSelectionData(UpgradeSelection upgradeSelection)
    {
        this.upgradeSelection = upgradeSelection;

        UpdateUI();
    }

    public void UpdateUI()
    {
        title.text = upgradeSelection.title;
        description.text = upgradeSelection.description;


        var currentUpgradeCount = GameManager.Instance.upgradeSystem.GetCurrentUpgradeCount(upgradeSelection);

        upgradeCount.text = $"{currentUpgradeCount} / {upgradeSelection.upgradeMaxCount}";
        icon.sprite = upgradeSelection.icon;

    }


}

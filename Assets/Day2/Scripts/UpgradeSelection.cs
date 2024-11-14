using UnityEngine;

public class UpgradeSelection : MonoBehaviour
{
    public UpgradeOption option;
    public string title;
    public string description;
    public Sprite icon;
    public int upgradeMaxCount;

    public void UpgradeEffect()
    {
        Debug.Log($"{title}가 업그레이드 되었습니다.");
    }
}

using UnityEngine;

public class UpgradeSelection : MonoBehaviour
{
    public string title;
    public string description;
    public Sprite icon;
    public int upgradeMaxCount;

    public virtual void UpgradeEffect()
    {
        Debug.Log($"{title}가 업그레이드 되었습니다.");
    }
}

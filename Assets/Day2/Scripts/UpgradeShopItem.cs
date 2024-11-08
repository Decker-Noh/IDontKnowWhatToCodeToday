using UnityEngine;

public class UpgradeShopItem : Item
{
    public override void Effect()
    {
        base.Effect();

        //TODO 상점 열기

        if (GameManager.Instance.upgradeSystem.canOpenUpgradePanel)
            GameManager.Instance.upgradeSystem.OpenUpgradeSelection();
    }
}

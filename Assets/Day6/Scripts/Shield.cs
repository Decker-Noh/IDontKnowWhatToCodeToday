using UnityEngine;

public class Shield : MonoBehaviour
{
    bool isActivated = false;
    public void Activate()
    {
        if (isActivated)
            return;

        GameManager.Instance.player.shieldGenerator.BreakShield(this);
    }
}

using System.Collections;
using UnityEngine;

public class SpeedCandy : Item
{
    [SerializeField] float boostedSpeed = 1.5f;       // 속도 증가 배율
    [SerializeField] float slowSpeed = 0.5f;          // 속도 감소 배율
    public float speedBoostDuration = 3f;              // 효과 지속 시간


    // 속도 증가 및 감소 상태를 PlayerStats에서 관리
    public override void Effect()
    {
        base.Effect();

        GameManager.Instance.player.ApplySpeedCandyEffect(boostedSpeed, speedBoostDuration);
        Destroy(gameObject, 1);
    }

    public override void SideEffect()
    {
        base.SideEffect();

        GameManager.Instance.player.ApplySpeedCandyEffect(slowSpeed, speedBoostDuration);
        Destroy(gameObject, 1);
    }


}
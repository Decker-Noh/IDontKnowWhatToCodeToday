using System.Collections;
using UnityEngine;

public class SpeedCandy : Item
{
    [SerializeField] float boostedSpeed = 2f;
    [SerializeField] float slowSpeed = 0.2f;
    public float speedBoostDuration = 5f;
    private Coroutine speedBoostCoroutine;

    public override void Effect()
    {
        base.Effect();

        if (speedBoostCoroutine != null)
        {
            PlayerStats.moveSpeed = PlayerStats.originalSpeed;
            StopCoroutine(speedBoostCoroutine);
        }

        speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine());
    }

    public override void SideEffect()
    {
        base.SideEffect();

        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        speedBoostCoroutine = StartCoroutine(SlowBoostRoutine());
    }

    private IEnumerator SpeedBoostRoutine()
    {
        PlayerStats.moveSpeed = boostedSpeed;
        yield return new WaitForSeconds(speedBoostDuration);
        PlayerStats.moveSpeed = PlayerStats.originalSpeed;
        speedBoostCoroutine = null;
        Destroy(gameObject, 1);
    }

    private IEnumerator SlowBoostRoutine()
    {
        PlayerStats.moveSpeed = slowSpeed;
        yield return new WaitForSeconds(speedBoostDuration);
        PlayerStats.moveSpeed = PlayerStats.originalSpeed;
        speedBoostCoroutine = null;

        Destroy(gameObject, 1);
    }



}
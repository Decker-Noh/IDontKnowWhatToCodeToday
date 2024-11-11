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
            PlayerStats.MoveSpeed = PlayerStats.OriginalSpeed;
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
        PlayerStats.MoveSpeed = boostedSpeed;
        yield return new WaitForSeconds(speedBoostDuration);
        PlayerStats.MoveSpeed = PlayerStats.OriginalSpeed;
        speedBoostCoroutine = null;
        Destroy(gameObject, 1);
    }

    private IEnumerator SlowBoostRoutine()
    {
        PlayerStats.MoveSpeed = slowSpeed;
        yield return new WaitForSeconds(speedBoostDuration);
        PlayerStats.MoveSpeed = PlayerStats.OriginalSpeed;
        speedBoostCoroutine = null;

        Destroy(gameObject, 1);
    }



}
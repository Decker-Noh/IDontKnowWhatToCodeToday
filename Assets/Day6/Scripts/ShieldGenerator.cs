using System;
using UnityEngine;
using System.Collections;

public class ShieldGenerator : MonoBehaviour
{
    [SerializeField] private RotateShield rotateShield; // 실드 배치를 담당
    private Coroutine regenCoroutine;

    private int currentShieldCount = 0; // 현재 실드 개수
    public int CurrentShieldCount
    {
        get => currentShieldCount;
        set
        {
            if (currentShieldCount != value)
            {
                currentShieldCount = value;
                OnChangedShieldCount?.Invoke(currentShieldCount);

                // 실드 개수가 감소하면 재생성 시작
                if (currentShieldCount < shieldMaxCount && regenCoroutine == null)
                {
                    StartShieldRegeneration();
                }
            }
        }
    }

    public Action<int> OnChangedShieldCount;
    private int shieldMaxCount = 5;

    private void OnEnable()
    {
        PlayerStats.OnChangedShieldLevel += OnChangedShieldLevel;
        OnChangedShieldCount += rotateShield.UpdateShieldObjects;
    }

    private void OnDisable()
    {
        PlayerStats.OnChangedShieldLevel -= OnChangedShieldLevel;
        OnChangedShieldCount -= rotateShield.UpdateShieldObjects;
    }

    private void OnChangedShieldLevel()
    {
        if (PlayerStats.ShieldCount > 0)
        {
            StartShieldRegeneration();
        }
        AddCurrentShieldCount();
    }

    private void StartShieldRegeneration()
    {
        // 이미 리젠 중이라면 중지
        if (regenCoroutine != null)
            return;

        regenCoroutine = StartCoroutine(RegenerateShield());
    }

    private IEnumerator RegenerateShield()
    {
        while (CurrentShieldCount < shieldMaxCount)
        {
            int shieldRegenLevel = PlayerStats.ShieldRegenTimeLevel;
            int regenTime = Mathf.Max(15 - (shieldRegenLevel - 1) * 2, 5);

            // 리젠 타이머 시작
            yield return new WaitForSeconds(regenTime);

            // 실드 추가
            AddCurrentShieldCount();
        }

        // 최대 개수에 도달하면 코루틴 중지
        regenCoroutine = null;
    }

    private void AddCurrentShieldCount()
    {
        CurrentShieldCount = Mathf.Clamp(CurrentShieldCount + 1, 0, shieldMaxCount);
    }

    // 실드를 깨트릴 때 호출
    public void BreakShield(Shield shield)
    {
        rotateShield.DestroyShield(shield);

        if (CurrentShieldCount > 0)
        {
            CurrentShieldCount--;
            Debug.Log("Shield broken!");
        }
    }
}
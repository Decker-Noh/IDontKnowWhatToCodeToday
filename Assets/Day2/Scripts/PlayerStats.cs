using System;
using UnityEngine;

public static class PlayerStats
{
    public static Action OnChangedVisibleRange;
    public static Action OnChangedEatRange;
    public static Action OnChangedEatSpeed;
    public static Action OnChangedShieldLevel;
    public static Action OnChangedShieldRegenLevel;
    public static Action OnChangedMoveSpeed;
    public static Action OnChangedHpPercent;

    public static float OriginalSpeed { get; set; } = 1;
    public static float OriginVisibleRange { get; set; } = 1;

    private static int shieldRegenLevel = 0;
    public static int ShieldRegenTimeLevel
    {
        get => shieldRegenLevel;
        set
        {
            if (shieldRegenLevel != value)
            {
                shieldRegenLevel = value;
                OnChangedShieldRegenLevel?.Invoke();
            }
        }
    }

    private static int shieldLevel = 0;
    public static int ShieldCount
    {
        get => shieldLevel;
        set
        {
            if (shieldLevel != value)
            {
                shieldLevel = value;
                OnChangedShieldLevel?.Invoke();
            }
        }
    }

    // EatSpeed 필드와 속성
    private static float eatSpeed = 1;
    public static float EatSpeed
    {
        get => eatSpeed;
        set
        {
            if (eatSpeed != value)
            {
                eatSpeed = value;
                OnChangedEatSpeed?.Invoke();
            }
        }
    }

    // EatRange 필드와 속성
    private static float eatRange = 2.5f;
    public static float EatRange
    {
        get => eatRange;
        set
        {
            if (eatRange != value)
            {
                eatRange = value;
                OnChangedEatRange?.Invoke();
            }
        }
    }

    // MoveSpeed 필드와 속성
    private static float moveSpeed = 1;
    public static float MoveSpeed
    {
        get => moveSpeed;
        set
        {
            if (moveSpeed != value)
            {
                moveSpeed = value;
                OnChangedMoveSpeed?.Invoke();
            }
        }
    }

    // AddedVisibleRange 필드와 속성
    private static float visibleRange = 0;
    public static float AddedVisibleRange
    {
        get => visibleRange;
        set
        {
            if (visibleRange != value)
            {
                visibleRange = value;
                OnChangedVisibleRange?.Invoke();
            }
        }
    }

    // HpPercent 필드와 속성
    private static float hpPercent = 1;
    public static float HpPercent
    {
        get => hpPercent;
        set
        {
            if (hpPercent != value)
            {
                hpPercent = value;
                OnChangedHpPercent?.Invoke();
            }
        }
    }

    public static void Initialize()
    {
        shieldLevel = 0;
        moveSpeed = 1;
        EatSpeed = 1;
        EatRange = 2.5f;
        //visibleRange = 5;
        hpPercent = 1;
    }
}
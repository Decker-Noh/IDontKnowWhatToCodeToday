using System;
using UnityEngine;

public static class PlayerStats
{
    public static Action visibleRangeChange;
    public static float OriginalSpeed { get; set; } = 1;
    public static float OriginVisibleRange { get; set; } = 1;
    public static float Defense { get; set; } = 1;
    public static float EatSpeed { get; set;} = 1;
    public static float MoveSpeed { get; set;} = 1;
    public static float AddedVisibleRange
    { 
        get => visibleRange;
        set
        {
            if (visibleRange != value)
            {
                visibleRange = value;
                visibleRangeChange.Invoke();
            }
        }
    }
    public static float HpPercent { get; set; } = 1;

    static float visibleRange = 0;
}
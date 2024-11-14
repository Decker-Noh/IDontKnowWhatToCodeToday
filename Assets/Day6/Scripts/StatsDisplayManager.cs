using System;
using UnityEngine;

public class StatsDisplayManager : MonoBehaviour
{
    private void OnEnable()
    {
        // PlayerStats 이벤트에 대한 리스너 등록
        PlayerStats.OnChangedEatRange += UpdateStats;
        PlayerStats.OnChangedVisibleRange += UpdateStats;
        PlayerStats.OnChangedEatSpeed += UpdateStats;
        PlayerStats.OnChangedShieldLevel += UpdateStats;
        PlayerStats.OnChangedMoveSpeed += UpdateStats;
        PlayerStats.OnChangedHpPercent += UpdateStats;
    }

    private void OnDisable()
    {
        // PlayerStats 이벤트에서 리스너 해제
        PlayerStats.OnChangedEatRange -= UpdateStats;
        PlayerStats.OnChangedVisibleRange -= UpdateStats;
        PlayerStats.OnChangedEatSpeed -= UpdateStats;
        PlayerStats.OnChangedShieldLevel -= UpdateStats;
        PlayerStats.OnChangedMoveSpeed -= UpdateStats;
        PlayerStats.OnChangedHpPercent -= UpdateStats;
    }

    private void UpdateStats()
    {
        // 필요한 경우 UI 업데이트 로직 추가 가능
    }

    private void OnGUI()
    {
        // 기본 폰트 스타일 설정
        GUIStyle style = new GUIStyle();
        style.fontSize = 16;
        style.normal.textColor = Color.white;

        // 화면에 스탯 정보 표시
        GUILayout.BeginArea(new Rect(10, 10, 200, 300));

        GUILayout.Label($"Move Speed: {PlayerStats.MoveSpeed}", style);
        GUILayout.Label($"Visible Range: {PlayerStats.AddedVisibleRange + PlayerStats.OriginVisibleRange}", style);
        GUILayout.Label($"Eat Range: {PlayerStats.EatRange}", style);
        GUILayout.Label($"Defense: {PlayerStats.ShieldCount}", style);
        GUILayout.Label($"Eat Speed: {PlayerStats.EatSpeed}", style);
        GUILayout.Label($"HP: {PlayerStats.HpPercent * 100}%", style);

        GUILayout.EndArea();
    }
}
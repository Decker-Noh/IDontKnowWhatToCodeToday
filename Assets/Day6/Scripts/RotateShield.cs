using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class RotateShield : MonoBehaviour
{
    public GameObject shieldPrefab; // 생성할 실드 프리팹
    public float orbitRadius = 2.0f; // 플레이어를 중심으로 실드가 도는 반경
    private List<Shield> shields = new List<Shield>(); // 생성된 실드 개체들
    private Coroutine regenCoroutine; // 실드 리젠을 위한 코루틴


    public void DestroyShield(Shield shield)
    {
        var destroyShield = shields.Find((x) => x == shield);
        if (destroyShield != null)
        {
            shields.Remove(destroyShield);
            Destroy(destroyShield.gameObject); // 실드 개체를 제거
        }
    }

    public void UpdateShieldObjects(int currentShieldCount)
    {
        if (currentShieldCount == shields.Count)
            return;

        // 실드 개수가 변경되면 기존 실드들을 제거
        foreach (var shield in shields)
        {
            Destroy(shield.gameObject);
        }
        shields.Clear();

        // 실드 개수를 설정 (최대 5개)
        int shieldCount = Mathf.Clamp(currentShieldCount, 1, 5);

        // 실드가 즉시 생성되어야 할 때
        for (int i = 0; i < shieldCount; i++)
        {
            float angle = i * (360f / shieldCount); // 각 실드의 각도
            Vector3 position = CalculateShieldPosition(angle);
            GameObject shieldObject = Instantiate(shieldPrefab, position, Quaternion.identity, transform);
            Shield shield = shieldObject.GetComponent<Shield>();
            shields.Add(shield);

            // 실드 개체가 플레이어를 바라보도록 설정
            Vector3 directionToCenter = (position - transform.position).normalized;
            shield.transform.rotation = Quaternion.LookRotation(Vector3.forward, directionToCenter);
        }
    }

    private Vector3 CalculateShieldPosition(float angle)
    {
        // 각도에 따라 위치 계산 (2D에서는 X, Y 축을 사용하여 회전)
        float radian = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radian) * orbitRadius;
        float y = Mathf.Sin(radian) * orbitRadius;
        return transform.position + new Vector3(x, y, 0);
    }

    private void Update()
    {
        RotateShields();
    }

    private void RotateShields()
    {
        // 실드가 공전하도록 모든 실드 개체들을 회전시킴
        foreach (var shield in shields)
        {
            shield.transform.RotateAround(transform.position, Vector3.forward, 50 * Time.deltaTime); // 초당 50도 회전
        }
    }

    // 실드 레벨이 바뀌었을 때 리젠이 시작되는 메서드 (코루틴을 사용)
    public void StartShieldRegeneration(int targetShieldCount)
    {
        // 실드 개수가 기존과 달라졌다면 리젠을 시작
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
        }

        regenCoroutine = StartCoroutine(RegenerateShields(targetShieldCount));
    }

    private IEnumerator RegenerateShields(int targetShieldCount)
    {
        int currentShieldCount = shields.Count;

        // 실드가 부족할 때마다 하나씩 생성
        while (currentShieldCount < targetShieldCount)
        {
            float angle = currentShieldCount * (360f / targetShieldCount);
            Vector3 position = CalculateShieldPosition(angle);
            GameObject shieldObject = Instantiate(shieldPrefab, position, Quaternion.identity, transform);
            Shield shield = shieldObject.GetComponent<Shield>();
            shields.Add(shield);

            // 실드 개체가 플레이어를 바라보도록 설정
            Vector3 directionToCenter = (position - transform.position).normalized;
            shield.transform.rotation = Quaternion.LookRotation(Vector3.forward, directionToCenter);

            currentShieldCount++;

            // 실드가 하나 생성된 후 잠시 대기
            yield return new WaitForSeconds(2f); // 2초 간격으로 하나씩 생성
        }
    }
}
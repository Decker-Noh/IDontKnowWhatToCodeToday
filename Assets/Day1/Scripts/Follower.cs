using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform target;
    public float followDistance = 2f;
    public float followSpeed = 3f;
    private Rigidbody2D rigid;

    private float sqrFollowDistance; // 제곱 거리로 비교를 최적화

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sqrFollowDistance = followDistance * followDistance; // 미리 제곱값 계산
    }

    void FixedUpdate()
    {
        // 타겟이 없거나 비활성화된 경우 처리를 한번만 확인
        if (target == null || !target.gameObject.activeSelf)
        {
            GameManager.Instance.player.SummonArray();
            return;
        }

        // 타겟과의 거리 제곱으로 비교해 불필요한 루트 계산 제거
        Vector2 directionToTarget = (Vector2)(target.position - transform.position);
        float sqrDistanceToTarget = directionToTarget.sqrMagnitude;

        if (sqrDistanceToTarget > sqrFollowDistance)
        {
            // 남은 거리 비례 가속도 적용
            float dynamicSpeed = followSpeed * (1 + Mathf.Sqrt(sqrDistanceToTarget) * 0.1f);

            // 위치 업데이트
            rigid.MovePosition(rigid.position + directionToTarget.normalized * dynamicSpeed * PlayerStats.MoveSpeed * Time.fixedDeltaTime);
        }
    }

    public void ChangeTarget(Transform _target)
    {
        target = _target;
    }
}
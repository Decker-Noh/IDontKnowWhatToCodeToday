using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform target;
    public float followDistance = 2f;
    public float followSpeed = 3f;
    Rigidbody2D rigid;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            GameManager.Instance.player.SummonArray();
            return;
        }
        if (!target.gameObject.activeSelf)
        {
            GameManager.Instance.player.SummonArray();
        }
        //Debug.Log("안들어와유?");
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget > followDistance)
        {
            Vector2 dirVec = target.position - transform.position;

            // 남아있는 거리에 비례한 가속도를 적용
            float dynamicSpeed = followSpeed * (1 + distanceToTarget * 0.1f);

            rigid?.MovePosition(rigid.position + dirVec.normalized * dynamicSpeed * PlayerStats.MoveSpeed * Time.fixedDeltaTime);
        }
    }

    public void ChangeTarget(Transform _target)
    {
        target = _target;
    }
}

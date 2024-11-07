using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform target;
    public float followDistance = 2f;
    public float followSpeed = 3f;
    Rigidbody2D rigid;
    private void Awake()
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
        if(!target.gameObject.activeSelf)
        {
            GameManager.Instance.player.SummonArray();
        }
        Debug.Log("안들어와유?");
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget > followDistance)
        {
            Vector2 dirVec = target.position - transform.position;
            rigid.MovePosition(rigid.position + dirVec.normalized * followSpeed * Time.fixedDeltaTime);
        }
    }
    public void ChangeTarget(Transform _target)
    {
        target = _target;
    }
}

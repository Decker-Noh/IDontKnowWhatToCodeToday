using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;



public class TrackingEnemy : Enemy
{
    public static int Exist;
    public GameObject bullet;
    [Range(0, 10)]
    public int search_range = 5;
    [Range(0, 10)]
    public int action_range = 7;
    [Range(0, 5)]
    bool target_flag = false;
    public float distance;

    Vector2 dirVec;

    protected override void InitAtAwake()
    {
        base.InitAtAwake();
        enemyKind = EnemyKind.TrackingEnemy;
    }

    protected override void InitAtOnEnable()
    {
        base.InitAtOnEnable();
        Exist++;
        if(Exist > 3) Dead();
        target_flag = false;
        distance = float.MaxValue;
    }

    protected override void Dead()
    {
        PoolingManager.Destroy(gameObject);
    }

    protected override void InitAtOnDisable()
    {
        base.InitAtOnDisable();
        Exist--;
        target_flag = false;
        
    }

    protected override void VirtualFixedUpdate()
    {
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 enemyPos = transform.position;
        float diffX = playerPos.x - enemyPos.x;
        float diffY = playerPos.y - enemyPos.y;
        distance = Mathf.Sqrt(diffX * diffX + diffY * diffY);


        if (distance > search_range && target_flag == false)
        {
            dirVec = targetRigidbody.position - rigid.position;
            rigid.MovePosition(rigid.position + dirVec.normalized * speed * Time.fixedDeltaTime);
        }
        else
        {
            target_flag = true;
        }

        if (target_flag)
        {
            rigid.MovePosition(rigid.position + dirVec.normalized * speed * Time.fixedDeltaTime);
        }
    }


}

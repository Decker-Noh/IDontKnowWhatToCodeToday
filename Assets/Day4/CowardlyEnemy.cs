using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;



public class CowardlyEnemy : Enemy
{
    public GameObject bullet;
    [Range(0,10)]
    public int search_range = 5;
    [Range(0,10)]
    public int action_range = 7;
    [Range(0,5)]
    [SerializeField] int action_cooldown = 2;
    float current_action_cooldown = 0;
    bool action_flag = false, target_flag = false;
    public float distance;

    void FixedUpdate()
    {
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 enemyPos = transform.position;
        float diffX = playerPos.x - enemyPos.x; 
        float diffY = playerPos.y - enemyPos.y;
        distance = Mathf.Sqrt(diffX * diffX + diffY * diffY);

        current_action_cooldown += Time.fixedDeltaTime;

        if(distance > search_range && target_flag == false)
        {
            Vector2 dirVec = targetRigidbody.position - rigid.position;
            rigid.MovePosition(rigid.position + dirVec.normalized * speed * Time.fixedDeltaTime);
        }
        else
        {
            target_flag = true;
        }

        if(target_flag)
        {
            if(current_action_cooldown > action_cooldown)
            {
                action_flag = !action_flag;
                if(action_flag)
                {
                    Shot();
                }
                current_action_cooldown = 0;
            }
            
            if(action_flag == false)
            {
                float action = distance - action_range;
                if(action < -0.5f)
                {
                    Vector2 dirVec = targetRigidbody.position - rigid.position;
                    rigid.MovePosition(rigid.position + dirVec.normalized * -1 * speed * Time.fixedDeltaTime);
                }
                else if(action > 0.5f)
                {
                    Vector2 dirVec = targetRigidbody.position - rigid.position;
                    rigid.MovePosition(rigid.position + dirVec.normalized * speed * Time.fixedDeltaTime);
                }

            }
        }    
    }

    void Shot()
    {
        GameObject go = PoolingManager.Instantiate(bullet, transform.position, quaternion.identity);
        CowardlyEnemyBullet bullet_component = go.GetComponent<CowardlyEnemyBullet>();
        bullet_component.Level = Level;
        bullet_component.direction = (Vector2)(GameManager.Instance.player.transform.position - transform.position).normalized;

    }

}

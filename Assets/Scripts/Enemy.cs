using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    Rigidbody2D rigid;
    Rigidbody2D targetRigidbody;
    bool hit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    void OnEnable()
    {
        targetRigidbody = GameManager.Instance.player.GetComponent<Rigidbody2D>();
    }
    IEnumerator KnockBack()
    {
        hit = true;
        yield return new WaitForFixedUpdate();
        if (!gameObject.activeSelf)
            yield break;
        rigid.linearVelocity = Vector3.zero; 
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector2 dirVec = transform.position - playerPos;
  
        rigid.AddForce(dirVec.normalized * 10, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(0.12f);
        hit = false;
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!gameObject.activeSelf)
            return;
        if(collider.CompareTag("Player"))
            StartCoroutine(KnockBack());
    }
    public void BeShallowed()
    {
        // if (hp>30)
        // {
        //     hp -= 30;
        //     return;
        // }
        GameManager.Instance.player.SummonExecute();
        Dead();
        
        
    }
    void Dead()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (hit)
        {
            Debug.Log("히트요");
            return;
        }
            
        Vector2 dirVec = targetRigidbody.position - rigid.position;
        rigid.MovePosition(rigid.position + dirVec.normalized * speed * Time.fixedDeltaTime);
    }
}
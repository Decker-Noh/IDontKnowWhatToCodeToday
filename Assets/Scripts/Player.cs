using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public Vector2 currentGrid;
    public Action EatAttack;
    public float speed; 
    Rigidbody2D rigid;
    public RaycastHit2D[] targets;
    public float scanRange;
    public LayerMask targetLayer;
    public List<GameObject> Summons;
    public GameObject summon;
    public Transform summonPosition;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        EatAttack += () => {Debug.Log("Eat!!");};
        EatAttack += ManuallyAttack;
    }

    void Update()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
        if(Input.GetButtonDown("Jump"))//spacebar
        {
            EatAttack.Invoke();
        }
    }
    void FixedUpdate()
    {
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
    void ManuallyAttack()
    {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        Debug.Log("Catch!!");
        for(int i=0; i<targets.Length; i++)
        {
            Enemy enemy = targets[i].transform.GetComponent<Enemy>();
            enemy.BeShallowed();
        }
    }
    public void SummonExecute()
    {
        GameObject _summon = Instantiate(summon);
        if (Summons.Count == 0)
        {
            _summon.transform.position = transform.position;
        }
        else
        {
            _summon.transform.position = Summons[Summons.Count-1].transform.position;
        }
        _summon.transform.parent = summonPosition;
        Summons.Add(_summon);
    }
    public void SummonArray()
    {
        Follower formerFollower = null;
        Debug.Log("히어"+ Summons.Count);
        for (int i=0; i<Summons.Count; i++)
        {
            Debug.Log("히어");
            Follower follower = Summons[i].GetComponent<Follower>();
            if (i==0)
            {
                follower.ChangeTarget(transform);
            }
            else
            {
                follower.ChangeTarget(formerFollower.transform);
            }
            formerFollower = follower;

        }
    }
    private void LateUpdate()
    {
        
    }
}
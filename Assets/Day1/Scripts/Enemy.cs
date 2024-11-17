using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour, IEatable
{
    public EnemyKind enemyKind;
    public float speed;
    protected Rigidbody2D rigid;
    protected Rigidbody2D targetRigidbody;
    protected bool hit;

    private int level = 0;

    public int Level
    {
        get => level;
        set
        {
            if (level != value)
            {
                level = value;
                OnChangedLevel?.Invoke(level);
            }
        }
    }

    public Action<int> OnChangedLevel;

    [SerializeField] protected TextMeshProUGUI levelText;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    public float dropChance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        InitAtAwake();
    }
    protected virtual void InitAtAwake()
    {
        enemyKind = EnemyKind.NONE;
        rigid = GetComponent<Rigidbody2D>();
        OnChangedLevel += OnChangedLevelEvent;
    }

    void OnEnable()
    {
        InitAtOnEnable();
    }

    protected virtual void InitAtOnEnable()
    {
        targetRigidbody = GameManager.Instance.player.rigid;
        Level = 1;
        StartCoroutine(LifetimeProcess(10));
    }


    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        InitAtOnDisable();
    }

    protected virtual void InitAtOnDisable()
    {
        targetRigidbody = null;
        lock_lifetimeprocess = false;
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

        GameManager.Instance.player.GetDamaged(level);

        rigid.AddForce(dirVec.normalized * 10, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.12f);
        hit = false;


    }
    public int life;
    public bool lock_lifetimeprocess = false;
    IEnumerator LifetimeProcess(int lifetime)
    {
        if (lock_lifetimeprocess == true) yield break;
        lock_lifetimeprocess = true;

        bool alive_flag = true;
        while (alive_flag)
        {
            int remaining_lifetime = lifetime;
            while (remaining_lifetime > 0)
            {
                life = remaining_lifetime;
                yield return new WaitForSeconds(10);
                remaining_lifetime -= 10;
            }

            Vector3 playerPos = GameManager.Instance.player.transform.position;
            Vector3 enemyPos = transform.position;
            float diffX = playerPos.x - enemyPos.x;
            float diffY = playerPos.y - playerPos.y;
            float distance = Mathf.Sqrt(diffX * diffX + diffY * diffY);

            alive_flag = distance <= 20;
        }
        lock_lifetimeprocess = false;
        Dead();
        yield break;
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!gameObject.activeSelf)
            return;
        if (collider.CompareTag("Player"))
            StartCoroutine(KnockBack());
    }

    private void BeShallowed()
    {
        // if (hp>30)
        // {
        //     hp -= 30;
        //     return;
        // }
        GameManager.Instance.player.SummonExecuteByAte(level, this);
        Dead();
    }

    protected virtual void Dead()
    {
        //TODO 아이템 드롭 확률에 따라 아이템 생성
        //* 1. 아이템 드롭을 할 지 안 할지 먼저 드랍확률에 따라 드랍 여부 결정
        //* 2. 마약 아이템 드랍이 결정되었다면 어떤 아이템을 드랍할지 전체 아이템 확률 리스트에 기반하여 아이템을 골라온다.
        //* 3. 그렇게 고른 아이템을 적군이 죽은 위치에 생성한다. 

        // 1. 드롭 확률에 따라 아이템 드롭 여부 결정
        float dropRoll = UnityEngine.Random.Range(0f, 1f);  // 0과 1 사이의 랜덤 값 생성
        if (dropRoll <= dropChance)
        {
            // 2. 아이템 드롭을 결정했다면, 전체 아이템 확률 리스트에서 아이템을 고른다
            DropItemData itemToDrop = GameManager.Instance.itemDropContainer.GetItemToDrop();

            if (itemToDrop != null)
            {
                // 3. 아이템을 적군이 죽은 위치에 생성
                GameManager.Instance.itemDropContainer.GenerateItemAtPosition(transform.position, itemToDrop);
            }
        }

        PoolingManager.Destroy(gameObject);
    }

    void FixedUpdate()
    {
        VirtualFixedUpdate();
    }

    protected virtual void VirtualFixedUpdate()
    {
        if (hit)
        {
            //Debug.Log("히트요");
            return;
        }

        // if(IsFarFromPlayer())
        // {
        //     PoolingManager.Destroy(gameObject);
        //     return;
        // }

        Vector2 dirVec = targetRigidbody.position - rigid.position;
        rigid.MovePosition(rigid.position + dirVec.normalized * speed * Time.fixedDeltaTime);
    }


    void OnChangedLevelEvent(int changedLevel)
    {
        levelText.text = changedLevel.ToString();
        // var colorRed = 100 + changedLevel * 2;
        // colorRed = Mathf.Clamp(colorRed, 0, 255);

        // spriteRenderer.color = new Color(colorRed, 0, 0, 255);
    }

    bool IsFarFromPlayer()
    {
        float distance = Vector2.Distance(transform.position, targetRigidbody.position);

        Camera cam = Camera.main;
        float screenWidthWorldUnits = 2f * cam.orthographicSize * cam.aspect;

        return distance > screenWidthWorldUnits;
    }

    public int GetLevel()
    {
        return Level;
    }

    public void OnAteEvent()
    {
        BeShallowed();
    }

    public string GetName()
    {
        return gameObject.name;
    }
}

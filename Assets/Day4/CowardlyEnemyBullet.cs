using System;
using System.Collections;
using UnityEngine;
using TMPro;
public class CowardlyEnemyBullet : MonoBehaviour, IEatable
{
    public float speed = 7f;
    public float rotationSpeed = 1000f;

    protected Rigidbody2D rigid;
    int lifetime = 3;
    public int life;
    public bool lock_lifetimeprocess = false;
    public Vector2 direction;

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

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        OnChangedLevel += OnChangedLevelEvent;
    }
    void OnEnable()
    {
        speed = UnityEngine.Random.Range(5, 8);
        rotationSpeed = speed * 120;
        StartCoroutine(LifetimeProcess(lifetime));
    }

    void OnDisable()
    {
        lock_lifetimeprocess = false;
    }

    IEnumerator LifetimeProcess(int lifetime)
    {
        if (lock_lifetimeprocess == true) yield break;
        lock_lifetimeprocess = true;

        float remaining_lifetime = lifetime;
        while (remaining_lifetime > 0)
        {
            remaining_lifetime -= Time.fixedDeltaTime;
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

            rigid.MovePosition(rigid.position + (direction * speed * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }

        lock_lifetimeprocess = false;
        Dead();
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!gameObject.activeSelf)
            return;

        if (collider.CompareTag("Shield"))
        {
            var shield = collider.GetComponent<Shield>();
            shield.Activate();
            Dead();
        }


        if (collider.CompareTag("Player"))
        {
            GameManager.Instance.player.GetDamaged(level);
            Dead();
        }

    }

    void Dead()
    {
        GameManager.Instance.player.SummonExecuteByAte(level, this);
        PoolingManager.Destroy(gameObject);
    }

    void OnChangedLevelEvent(int changedLevel)
    {
        levelText.text = changedLevel.ToString();
    }

    public int GetLevel()
    {
        return level;
    }

    public void OnAteEvent()
    {
        Dead();
    }
    public string GetName()
    {
        return gameObject.name;
    }
}

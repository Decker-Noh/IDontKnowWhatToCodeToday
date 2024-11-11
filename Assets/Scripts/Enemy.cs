using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public float speed;
    Rigidbody2D rigid;
    Rigidbody2D targetRigidbody;
    bool hit;

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

    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] SpriteRenderer spriteRenderer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        OnChangedLevel += OnChangedLevelEvent;
    }
    void OnEnable()
    {
        targetRigidbody = GameManager.Instance.player.rigid;
        Level = 1;
    }


    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        targetRigidbody = null;
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
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!gameObject.activeSelf)
            return;
        if (collider.CompareTag("Player"))
            StartCoroutine(KnockBack());
    }

    public void BeShallowed()
    {
        // if (hp>30)
        // {
        //     hp -= 30;
        //     return;
        // }
        GameManager.Instance.player.SummonExecute(level);
        Dead();
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }

    void FixedUpdate()
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
        var colorRed = 100 + changedLevel * 2;
        colorRed = Mathf.Clamp(colorRed, 0, 255);

        spriteRenderer.color = new Color(colorRed, 0, 0, 255);
    }

    bool IsFarFromPlayer()
    {
        float distance = Vector2.Distance(transform.position, targetRigidbody.position);

        Camera cam = Camera.main;
        float screenWidthWorldUnits = 2f * cam.orthographicSize * cam.aspect;

        return distance > screenWidthWorldUnits;
    }
}

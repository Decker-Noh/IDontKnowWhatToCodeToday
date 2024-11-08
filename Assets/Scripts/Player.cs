using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using DG.Tweening;

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
    public GameObject summonPrefab;
    public Transform summonPosition;

    private bool stopEating = false;
    [SerializeField] private int AteItemCount = 0;
    public int GetAteItemCount => AteItemCount;
    private float lastReduceTime;


    private int playerLevel = 0;

    public int PlayerLevel
    {
        get => playerLevel;
        set
        {
            if (playerLevel != value)
            {
                playerLevel = value;
                OnChangedLevel?.Invoke(playerLevel);
            }
        }
    }


    float stopEatingTime = 5;
    private float attackCooldown = 0f;
    [SerializeField] private float attackDelay = 1f;



    [SerializeField] List<AudioClip> damagedAudioClip;
    [SerializeField] List<AudioClip> eatAudioClip;


    [SerializeField]
    private GameObject enemyObjectPrefab;
    public Action<int> OnChangedLevel;
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] SpriteRenderer playerCharacterSpriteReneder;
    [SerializeField] SpriteRenderer eatRangeSpriteReneder;

    bool youCanEat = true;

    AudioSource audioSource;



    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody2D>();
        EatAttack += () => { Debug.Log("Eat!!"); };
        EatAttack += ManuallyAttack;
        OnChangedLevel += OnChangedLevelEvent;
        PlayerLevel = 1;
    }

    void Update()
    {
        // 쿨타임 감소
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;

            if (attackCooldown < 0)
                attackCooldown = 0;
        }


        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");


        if (Input.GetButtonDown("Jump")) //spacebar
        {
            if (youCanEat && attackCooldown == 0)
                EatAttack.Invoke();
        }

        CheckYourAteItem();

    }
    void FixedUpdate()
    {
        Vector2 nextVec = inputVec.normalized * speed * PlayerStats.moveSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);


        if (inputVec != Vector2.zero)
        {
            // 각도 계산
            float angle = Mathf.Atan2(inputVec.y, inputVec.x) * Mathf.Rad2Deg;
            playerCharacterSpriteReneder.transform.rotation = Quaternion.Euler(0, 0, angle);
            playerCharacterSpriteReneder.flipY = inputVec.x < 0;
        }
    }

    void ManuallyAttack()
    {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        if (targets.Length <= 0)
        {
            TryEatEffect();
            return;

        }
        attackCooldown = attackDelay;

        int totalLevel = 0;

        List<Enemy> tempEnemyList = new List<Enemy>();
        foreach (var target in targets)
        {
            var enemy = target.transform.GetComponent<Enemy>();
            tempEnemyList.Add(enemy);
            totalLevel += enemy.Level;
        }

        if (totalLevel > PlayerLevel)
        {
            //* 너무 많이 처먹었어 
            FailEatEffect();
            YourEatTooManyEnemy(totalLevel - playerLevel);
        }
        else
        {
            SuccessEatEffect();

            foreach (var enemy in tempEnemyList)
            {
                enemy.BeShallowed();
                PlayerLevel += enemy.Level;
            }
        }
    }
    public void SummonExecute(int level)
    {
        PlayRandomEatSound();
        GameObject summonObject = Instantiate(summonPrefab);
        if (Summons.Count == 0)
        {
            summonObject.transform.position = transform.position;
        }
        else
        {
            summonObject.transform.position = Summons[Summons.Count - 1].transform.position;
        }
        summonObject.transform.parent = summonPosition;
        Summons.Add(summonObject);

        var summon = summonObject.GetComponent<Summon>();
        summon.Level = level;
    }

    public void SummonArray()
    {
        Follower formerFollower = null;
        Debug.Log("히어" + Summons.Count);
        for (int i = 0; i < Summons.Count; i++)
        {
            Debug.Log("히어");
            Follower follower = Summons[i].GetComponent<Follower>();
            if (i == 0)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            var item = other.GetComponent<Item>();

            if (item.used)
                return;

            if (item.doNotEatTooMuch)
            {
                if (AteItemCount >= 3)
                {
                    YouEatTooMuchItem();
                    item.SideEffect();
                }
                else
                {
                    item.Effect();
                    AteItemCount++;
                }
            }
            else
            {
                item.Effect();
            }


        }
    }

    void CheckYourAteItem()
    {
        //Debug.Log("얼마나 많이 먹는지 확인할꺼야!!");

        if (AteItemCount > 0)
        {
            if (Time.time - lastReduceTime >= 10f)
            {
                AteItemCount--;
                lastReduceTime = Time.time;
            }
        }
    }

    void YouEatTooMuchItem()
    {
        Debug.Log("너무 많이 먹었어!!");
        StopEatingItem();
    }

    void StopEatingItem()
    {
        Debug.Log("그만 먹어!!");
        stopEating = true;
    }

    void YourEatTooManyEnemy(int penaltyCount)
    {
        StartCoroutine(StopEatingForAMoment());

        var startIndex = Summons.Count - penaltyCount;
        startIndex = Mathf.Clamp(startIndex, 0, Summons.Count);


        // 뒤에서 count 개수만큼 추출
        var byeSummonList = Summons.GetRange(startIndex, penaltyCount);
        ChangeSummonToEnemy(byeSummonList);

    }


    public void ChangeSummonToEnemy(List<GameObject> byeSummonList)
    {
        for (int i = 0; i < byeSummonList.Count; i++)
        {
            GameObject enemyObject = Instantiate(enemyObjectPrefab);
            var byeSummonObject = byeSummonList[i];
            var enemy = enemyObject.GetComponent<Enemy>();

            if (byeSummonObject != null && byeSummonObject.activeInHierarchy)
            {
                var summon = byeSummonObject.GetComponent<Summon>();
                enemy.Level = summon.Level;
                PlayerLevel -= summon.Level;
                enemyObject.transform.position = byeSummonObject.transform.position;

                Summons.Remove(byeSummonObject);
                Destroy(byeSummonObject);
            }
        }
    }


    public IEnumerator StopEatingForAMoment()
    {
        youCanEat = false;
        SpriteFlashEffect(playerCharacterSpriteReneder, 5, stopEatingTime, Color.black);
        yield return new WaitForSeconds(stopEatingTime);
        youCanEat = true;
    }



    void SpriteFlashEffect(SpriteRenderer spriteRenderer, int flashCount, float flashDuration, Color32 toColor)
    {
        Sequence flashSequence = DOTween.Sequence();
        Color32 origin = spriteRenderer.color;
        Debug.Log($"originColor:{origin}");

        for (int i = 0; i < flashCount; i++)
        {
            // 하얗게 변환
            flashSequence.Append(spriteRenderer.DOColor(toColor, flashDuration / 2));
            // 원래 색상으로 복귀
            flashSequence.Append(spriteRenderer.DOColor(origin, flashDuration / 2));
        }

        flashSequence.OnComplete(() => spriteRenderer.color = origin);
    }




    void OnChangedLevelEvent(int currentLevel)
    {
        levelText.text = currentLevel.ToString();
    }


    public void GetDamaged(int damage)
    {
        Debug.Log(" 으악 내 소환수들 ");
        PlayRandomDamagedSound();
        int remainingDamage = damage;

        // Summons 리스트를 레벨 기준으로 정렬하여 가장 낮은 레벨의 Summon부터 제거하도록 함.
        Summons = Summons.OrderBy(summon => summon.GetComponent<Summon>().Level).ToList();

        // 레벨을 차감하거나 Summon을 제거
        for (int i = 0; i < Summons.Count && remainingDamage > 0; i++)
        {
            var summon = Summons[i].GetComponent<Summon>();

            if (summon.Level <= remainingDamage)
            {
                // Summon 레벨이 남은 damage보다 작거나 같다면 Summon을 제거
                remainingDamage -= summon.Level;
                RemoveSummon(summon);
            }
            else
            {
                // 남은 damage가 Summon의 레벨보다 작다면 Summon의 레벨을 줄임
                summon.Level -= remainingDamage;
                PlayerLevel -= remainingDamage;
                remainingDamage = 0;
            }
        }
    }

    private void RemoveSummon(Summon summon)
    {
        PlayerLevel -= summon.Level;
        Summons.Remove(summon.gameObject);
        Destroy(summon.gameObject);
    }


    private void PlayRandomDamagedSound()
    {
        var index = UnityEngine.Random.Range(0, damagedAudioClip.Count);
        var currentClip = damagedAudioClip[index];
        audioSource.clip = currentClip;
    }

    void PlayRandomEatSound()
    {
        var index = UnityEngine.Random.Range(0, eatAudioClip.Count);
        var currentClip = eatAudioClip[index];
        audioSource.clip = currentClip;
    }


    void TryEatEffect()
    {
        SpriteFlashEffect(eatRangeSpriteReneder, 1, 0.1f, new Color32(55, 37, 236, 100));
    }

    void SuccessEatEffect()
    {
        SpriteFlashEffect(eatRangeSpriteReneder, 1, 0.1f, new Color32(55, 150, 236, 100));
    }

    void FailEatEffect()
    {
        SpriteFlashEffect(eatRangeSpriteReneder, 1, 0.1f, new Color32(255, 0, 0, 100));
    }
}
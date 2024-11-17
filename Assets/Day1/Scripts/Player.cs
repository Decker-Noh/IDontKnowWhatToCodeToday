using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public Vector2 currentGrid;
    public Action EatAttack;
    public float speed;
    public Rigidbody2D rigid { get; private set; }
    public Light2D light2D { get; private set; }
    public RaycastHit2D[] targets;
    public LayerMask targetLayer;
    public List<GameObject> Summons;
    public List<GameObject> summonPrefabs;
    private Dictionary<string, GameObject> summonsPrefabsDict = new Dictionary<string, GameObject>();

    public Transform summonPosition;

    private bool stopEating = false;
    [SerializeField] private int ateItemCount = 0;
    public int AteItemCount
    {
        get => ateItemCount;
        set
        {
            if (ateItemCount != value)
            {
                ateItemCount = value;
                OnChangedAteItemCount?.Invoke();
            }
        }
    }

    Action OnChangedAteItemCount;

    public List<Image> ateItemCountImageList = new List<Image>();

    private float lastReduceTime = -1;


    private int playerLevel = 0;

    public int PlayerLevel
    {
        get => playerLevel;
        set
        {
            if (playerLevel != value)
            {
                Debug.Log(value);
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
    [SerializeField] SpriteRenderer shieldSpriteReneder;

    bool youCanEat = true;

    AudioSource audioSource;

    public ShieldGenerator shieldGenerator;
    // 효과가 진행 중인지 체크하는 변수
    private Coroutine speedEffectCoroutine;

    void Start()
    {
        PreGameSettingPlayer();
        GameManager.Instance.player = this;
        GameManager.Instance.GameStart += StartGamePlayerInit;
        GameManager.Instance.GameEnd += WoldSlowTime;


        foreach (var summonPrefab in summonPrefabs)
        {
            summonsPrefabsDict.Add(summonPrefab.name, summonPrefab);
        }
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
    void WoldSlowTime()
    {
        Time.timeScale = 0.3f;
    }
    void PreGameSettingPlayer()
    {
        Time.timeScale = 1.0f;
        eatRangeSpriteReneder.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody2D>();
        light2D = GetComponent<Light2D>();
        EatAttack += () => { Debug.Log("Eat!!"); };
        EatAttack += ManuallyAttack;
        OnChangedLevel += OnChangedLevelEvent;
        PlayerStats.OnChangedEatRange += OnChangedEatRange;
        OnChangedAteItemCount += OnChangedAteItemCountHandler;

    }
    void StartGamePlayerInit()
    {
        eatRangeSpriteReneder.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
        AteItemCountUIDeactiveAll();
        PlayerLevel = 1;
        AteItemCount = 0;

        InitPlayerStats();
    }
    void FixedUpdate()
    {
        if (GameManager.Instance.gameStateEnum != GameStateEnum.InGame) return;

        Vector2 nextVec = inputVec.normalized * speed * PlayerStats.MoveSpeed * Time.fixedDeltaTime;
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
        targets = Physics2D.CircleCastAll(transform.position, PlayerStats.EatRange, Vector2.zero, 0, targetLayer);
        if (targets.Length <= 0)
        {
            TryEatEffect();
            return;
        }

        attackCooldown = attackDelay;

        int totalLevel = 0;

        List<IEatable> tempAteList = new List<IEatable>();
        foreach (var target in targets)
        {
            var eatable = target.transform.GetComponent<IEatable>();
            tempAteList.Add(eatable);
            totalLevel += eatable.GetLevel();
        }

        if (totalLevel > PlayerLevel)
        {
            //* 너무 많이 처먹었어 
            FailEatEffect();
            YourEatTooManyEnemy(totalLevel - playerLevel);
            GetDamaged(totalLevel);
        }
        else
        {
            SuccessEatEffect();

            foreach (var eatable in tempAteList)
            {
                eatable.OnAteEvent();
                PlayerLevel += eatable.GetLevel();
            }
        }
    }

    public void SummonExecuteByAte(int level, IEatable eatable)
    {
        var prefabName = eatable.GetName() + "_Summon";
        if (summonsPrefabsDict.ContainsKey(prefabName))
        {
            var summonPrefab = summonsPrefabsDict[prefabName];
            //TODO 먹은 eatable 이름과 같은 오브젝트 프리팹으로 뒤에 생성 이미지를 바꾸던가?
            PlayRandomEatSound();
            GameObject summonObject = PoolingManager.Instantiate(summonPrefab);
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
                    lastReduceTime = -1f;
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

        // 아이템이 1개 이상일 때만 로직을 실행
        if (AteItemCount > 0)
        {
            // 처음 아이템을 먹었을 때 시간을 기록
            if (lastReduceTime == -1f)
            {
                lastReduceTime = Time.time;
            }

            // 10초가 지나면 AteItemCount를 감소시킴
            if (Time.time - lastReduceTime >= 10f)
            {
                AteItemCount--;
                lastReduceTime = Time.time; // 감소 후 시간을 리셋
            }
        }

    }

    void YouEatTooMuchItem()
    {
        Debug.Log("너무 많이 먹었어!!");
        StartCoroutine(StopEatingItem());
    }

    IEnumerator StopEatingItem()
    {
        Debug.Log("그만 먹어!!");
        stopEating = true;
        yield return new WaitForSeconds(3f);
        stopEating = false;
        AteItemCount = 0;
    }

    void YourEatTooManyEnemy(int penaltyCount)
    {
        StartCoroutine(StopEatingForAMoment());

        var startIndex = Summons.Count - penaltyCount;
        startIndex = Mathf.Clamp(startIndex, 0, Summons.Count);

        var tempPenaltyCount = penaltyCount;
        tempPenaltyCount = Mathf.Clamp(penaltyCount, 0, Summons.Count);

        // 뒤에서 count 개수만큼 추출
        var byeSummonList = Summons.GetRange(startIndex, tempPenaltyCount);


        foreach (var byeSummon in byeSummonList)
        {
            Summons.Remove(byeSummon);
            PoolingManager.Destroy(byeSummon);
        }
        //ChangeSummonToEnemy(byeSummonList);

    }


    public void ChangeSummonToEnemy(List<GameObject> byeSummonList)
    {
        for (int i = 0; i < byeSummonList.Count; i++)
        {
            GameObject enemyObject = PoolingManager.Instantiate(enemyObjectPrefab);
            var byeSummonObject = byeSummonList[i];
            var enemy = enemyObject.GetComponent<Enemy>();

            if (byeSummonObject != null && byeSummonObject.activeInHierarchy)
            {
                var summon = byeSummonObject.GetComponent<Summon>();
                enemy.Level = summon.Level;
                PlayerLevel -= summon.Level;
                enemyObject.transform.position = byeSummonObject.transform.position;

                Summons.Remove(byeSummonObject);
                PoolingManager.Destroy(byeSummonObject);
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
        if (GameManager.Instance.gameStateEnum != GameStateEnum.InGame) return;


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
        if (remainingDamage > 0)
        {
            GameManager.Instance.GameEnd.Invoke();
        }
    }

    private void RemoveSummon(Summon summon)
    {
        PlayerLevel -= summon.Level;
        Summons.Remove(summon.gameObject);
        PoolingManager.Destroy(summon.gameObject);
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

    void InitPlayerStats()
    {
        PlayerStats.OriginalSpeed = speed;
        PlayerStats.OriginVisibleRange = light2D.pointLightInnerRadius;
        Debug.Log(light2D.pointLightInnerRadius);
        PlayerStats.OnChangedVisibleRange += ChangeVisibleRange;
    }

    void ChangeVisibleRange()
    {
        Debug.Log(PlayerStats.OriginVisibleRange);
        light2D.pointLightInnerRadius = PlayerStats.OriginVisibleRange + PlayerStats.AddedVisibleRange * .1f;
        light2D.pointLightOuterRadius = light2D.pointLightInnerRadius * 2;
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



    void OnChangedEatRange()
    {
        // TODO 플레이어 먹는 범위 변경
        eatRangeSpriteReneder.transform.localScale = new Vector3(PlayerStats.EatRange, PlayerStats.EatRange, 1);
    }

    // 아이템이 효과를 적용할 때 호출되는 메서드
    public void ApplySpeedCandyEffect(float speedIncrease, float duration)
    {
        // 이미 효과가 진행 중이라면 코루틴을 멈추고 다시 시작
        if (speedEffectCoroutine != null)
        {
            PlayerStats.MoveSpeed = PlayerStats.OriginalSpeed;
            StopCoroutine(speedEffectCoroutine);
        }

        speedEffectCoroutine = StartCoroutine(SpeedBoostCoroutine(speedIncrease, duration));
    }

    // 속도 증가를 위한 코루틴
    private IEnumerator SpeedBoostCoroutine(float speedIncrease, float duration)
    {
        // 원래 속도 저장
        PlayerStats.OriginalSpeed = PlayerStats.MoveSpeed;

        // 이동속도 증가
        PlayerStats.MoveSpeed *= speedIncrease;

        // 효과가 지속되는 시간 동안 대기
        yield return new WaitForSeconds(duration);

        // 시간 지나면 원래 속도로 되돌리기
        PlayerStats.MoveSpeed = PlayerStats.OriginalSpeed;

        // 코루틴 종료 후
        speedEffectCoroutine = null;
    }


    void AteItemCountUIDeactiveAll()
    {
        foreach (var ateItemImage in ateItemCountImageList)
        {
            ateItemImage.gameObject.SetActive(false);
        }
    }

    // 먹은 아이템 개수가 바뀔때
    public void OnChangedAteItemCountHandler()
    {
        Debug.Log($"ateItemCount:{ateItemCount}");
        AteItemCountUIDeactiveAll();

        for (int i = 0; i < AteItemCount; i++)
        {
            ateItemCountImageList[i].gameObject.SetActive(true);
        }

    }
}
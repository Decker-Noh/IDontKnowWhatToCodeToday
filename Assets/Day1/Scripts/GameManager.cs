using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    #region Singleton
    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }
    }
    #endregion

    public UpgradeSystem upgradeSystem;

    public ItemDropContainer itemDropContainer;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        GameStart += () => gameStateEnum = GameStateEnum.InGame;
        GameStart += () => PlayerStats.Initialize();
        GameEnd += () => gameStateEnum = GameStateEnum.PostGame;
    }
    public void StartGame()
    {
        GameStart.Invoke();
    }
    public void GoHome()
    {
        StopAllCoroutines();
        PoolingManager.PoolManagerInit();
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    public Player player;
    public float GameTime;
    public Action GameStart;
    public Action GameEnd;
    public int StageLevel;
    public GameStateEnum gameStateEnum = GameStateEnum.PreGame;
    public int follwerCount;

}

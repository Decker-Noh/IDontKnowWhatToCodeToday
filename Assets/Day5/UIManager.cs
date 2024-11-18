using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject GameStartUI;
    public GameObject GameEndUI;
    public RankingBoard rankingBoard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.GameStart += InGameUI;
        GameManager.Instance.GameEnd += PostGameUI;
        rankingBoard.Init();
        PreGameUI();

    }
    void PreGameUI()
    {
        GameStartUI.SetActive(true);
        GameEndUI.SetActive(false);
    }
    void InGameUI()
    {
        GameStartUI.SetActive(false);
        GameEndUI.SetActive(false);
    }
    void PostGameUI()
    {
        GameStartUI.SetActive(false);
        GameEndUI.SetActive(true);
    }
}

using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
public class HUD : MonoBehaviour
{
    Coroutine timerCoroutine;
    public TMP_Text timerTT;
    int minute;
    int second;
    // Update is called once per frame
    void Start()
    {
        HUDVisible(false);
        GameManager.Instance.GameStart += HUDStart;
        GameManager.Instance.GameEnd += HUDEnd;
    }
    void HUDInit()
    {
        minute = 00;
        second = 00;
        timerTT.text = $"{minute.ToString("D2")} : {second.ToString("D2")}";
    }
    void HUDStart()
    {
        HUDVisible(true);
        HUDInit();
        timerCoroutine = StartCoroutine(UpdateHUDTimer());
    }
    void HUDEnd()
    {
        StopCoroutine(timerCoroutine);
        timerCoroutine = null;
    }
    void HUDVisible(bool visible)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(visible);
        }
    }
    IEnumerator UpdateHUDTimer()
    {
        while(true)
        {
            HUDTimer();
            yield return new WaitForSeconds(1f);
        }
    }
    void HUDTimer()
    {
        minute = Mathf.FloorToInt((GameManager.Instance.GameTime / 60));
        second = Mathf.FloorToInt((GameManager.Instance.GameTime % 60));
        timerTT.text = $"{minute.ToString("D2")} : {second.ToString("D2")}";
    }
}

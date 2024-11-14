using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;


public class UpgradeSystem : MonoBehaviour
{
    [Header("[디버깅]")]
    public List<UpgradeSelection> canUpgradeSelectionList = new List<UpgradeSelection>();
    public Dictionary<UpgradeSelection, int> upgradedSelectionDict = new Dictionary<UpgradeSelection, int>();
    private Dictionary<UpgradeOption, Action> upgradeActions = new Dictionary<UpgradeOption, Action>();
    [SerializeField] List<UpgradeSelectionUI> createdUpgradeSelectionUIList = new List<UpgradeSelectionUI>();
    public bool canOpenUpgradePanel = true;



    [Header("[프리펩 할당]")]
    public List<UpgradeSelection> allUpgradeSelectionList = new List<UpgradeSelection>();
    [SerializeField] private GameObject upgradeSelectionUIPrefab;
    [SerializeField] GameObject upgradeSelectionPanel;





    float eatSpeedPerUpgrade = 0.5f;
    float eatRangePerUpgrade = 0.5f;
    float moveSpeedPerUpgrade = 0.5f;
    int shieldCountPerUpgrade = 1;
    int shieldResenPerUpgrade = 1;
    float visibleRangePerUpgrade = 0.5f;



    public int GetCurrentUpgradeCount(UpgradeSelection upgradeSelection)
    {
        if (upgradedSelectionDict.ContainsKey(upgradeSelection))
        {
            return upgradedSelectionDict[upgradeSelection];
        }
        else
        {
            return 0;
        }
    }


    public void OpenUpgradeSelection()
    {
        canOpenUpgradePanel = false;
        Debug.Log("업그레이드 상점을 엽니다!!");
        //TODO 시간 멈추기
        Time.timeScale = 0;

        canUpgradeSelectionList = GetfilteredUpgrdeSelectionList(allUpgradeSelectionList);
        if (canUpgradeSelectionList.Count <= 0)
        {
            EndOfUpgradeSelection();
            return;
        }
        var threeUpgradeSelection = GetThreeUpgradeSelection();


        foreach (var upgradeSelection in threeUpgradeSelection)
        {
            CreateUpgradeSelectionUI(upgradeSelection);
        }

        EventSystem.current.SetSelectedGameObject(upgradeSelectionPanel.transform.GetChild(0).gameObject);
        upgradeSelectionPanel.SetActive(true);
        Debug.Log(EventSystem.current.currentSelectedGameObject);

        //TODO 업그레이드 선택지 제공
    }

    void CreateUpgradeSelectionUI(UpgradeSelection upgradeSelection)
    {
        var uiObject = Instantiate(upgradeSelectionUIPrefab, upgradeSelectionPanel.transform);
        var upgradeSelectionUI = uiObject.GetComponent<UpgradeSelectionUI>();
        createdUpgradeSelectionUIList.Add(upgradeSelectionUI);

        upgradeSelectionUI.SetUpgradeSelectionData(upgradeSelection);
        upgradeSelectionUI.OnSelectUpgrdaeSelection += OnSelectedUpgradeSelection;
        upgradeSelectionUI.OnSelectUpgrdaeSelection += (value) => upgradeActions[value.option].Invoke();
    }


    private List<UpgradeSelection> GetfilteredUpgrdeSelectionList(List<UpgradeSelection> originalData)
    {
        List<UpgradeSelection> filteredData = new List<UpgradeSelection>();
        Debug.Log($"원본 업그레이드 선택지: {originalData.Count}");

        foreach (var upgradeSelection in originalData)
        {
            //* 1번이라도 업그레이드 된 애들
            if (upgradedSelectionDict.ContainsKey(upgradeSelection))
            {
                //* 최대 수치보다 낮아야함
                if (upgradeSelection.upgradeMaxCount > upgradedSelectionDict[upgradeSelection])
                {
                    filteredData.Add(upgradeSelection);
                }
            }
            //* 업그레이드 한번도 안된 애들 
            else
            {
                filteredData.Add(upgradeSelection);
            }
        }

        //* 업그레이드 횟수가 종료된건 제거해야함


        return filteredData;
    }



    public void OnSelectedUpgradeSelection(UpgradeSelection upgradeSelection)
    {
        if (upgradedSelectionDict.ContainsKey(upgradeSelection))
        {
            if (upgradeSelection.upgradeMaxCount > upgradedSelectionDict[upgradeSelection])
            {
                upgradedSelectionDict[upgradeSelection]++;
                upgradeSelection.UpgradeEffect();
            }
            else
            {
                Debug.Log("업그레이드 끝 , 선택지에서 없었어야함");
            }
        }
        else
        {
            upgradedSelectionDict.Add(upgradeSelection, 1);
            upgradeSelection.UpgradeEffect();
        }

        EndOfUpgradeSelection();
    }

    void EndOfUpgradeSelection()
    {

        foreach (var createdUpgradeSelectionUI in createdUpgradeSelectionUIList)
        {
            Destroy(createdUpgradeSelectionUI.gameObject);
            Debug.Log("생성된 UI 제거");
        }

        upgradeSelectionPanel.SetActive(false);
        createdUpgradeSelectionUIList.Clear();
        canOpenUpgradePanel = true;
        Time.timeScale = 1;

        Debug.Log("업그레이드 선택 완료");

    }


    List<UpgradeSelection> GetThreeUpgradeSelection()
    {
        List<UpgradeSelection> threeUpgradeSelection = new List<UpgradeSelection>();

        // 사용할 수 있는 선택지의 복사본을 생성
        List<UpgradeSelection> tempUpgradeSelectionList = new List<UpgradeSelection>(canUpgradeSelectionList);

        if (tempUpgradeSelectionList.Count > 3)
        {
            int selectedCount = 0;
            while (selectedCount < 3 && tempUpgradeSelectionList.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, tempUpgradeSelectionList.Count);
                var upgradeSelection = tempUpgradeSelectionList[index];

                // 선택된 아이템이 이미 리스트에 없는 경우에만 추가
                if (!threeUpgradeSelection.Contains(upgradeSelection))
                {
                    threeUpgradeSelection.Add(upgradeSelection);
                    tempUpgradeSelectionList.RemoveAt(index);  // 선택 후 리스트에서 제거
                    selectedCount++;
                }
            }
        }
        else
        {
            threeUpgradeSelection.AddRange(tempUpgradeSelectionList);
        }

        return threeUpgradeSelection;
    }

    void Start()
    {
        GameManager.Instance.upgradeSystem = this;
        upgradeActions = new Dictionary<UpgradeOption, Action>()
        {
            { UpgradeOption.ShieldCount, () =>
                PlayerStats.ShieldCount += shieldCountPerUpgrade },
            { UpgradeOption.ShieldResenTime, () =>
                PlayerStats.ShieldRegenTimeLevel += shieldResenPerUpgrade },
            { UpgradeOption.EatSpeed, () =>
                 PlayerStats.EatSpeed+= eatSpeedPerUpgrade },
            { UpgradeOption.EatRange, () =>
                PlayerStats.EatRange += eatRangePerUpgrade},
            { UpgradeOption.MoveSpeed, () =>
                PlayerStats.MoveSpeed += moveSpeedPerUpgrade },
            { UpgradeOption.VisibleRange, () =>
                 PlayerStats.AddedVisibleRange += visibleRangePerUpgrade }
        };
    }
}

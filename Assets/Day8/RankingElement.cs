using UnityEngine;
using TMPro;
public class RankingElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] TextMeshProUGUI summons;

    public void SetRank(int rank, float time, int summons){
        this.rank.text = rank.ToString();
        this.time.text = $"{Mathf.FloorToInt(time / 60).ToString("D2")} : {Mathf.FloorToInt(time % 60).ToString("D2")}";
        this.summons.text = summons.ToString();
    }
}

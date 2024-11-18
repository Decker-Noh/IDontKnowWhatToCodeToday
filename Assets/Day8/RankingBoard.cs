using System.Collections.Generic;
using UnityEngine;

public class Record
{
    public float time;
    public int summons;

    public Record(float time, int summons)
    {
        this.time = time;
        this.summons = summons;
    }
}

public class RankingBoard : MonoBehaviour
{
    List<Record> records = new List<Record>(10);
    RankingElement[] rankingElements;

    public void Init()
    {
        LoadRanking();
        GameManager.Instance.GameEnd += ShowRanking;
        rankingElements = GetComponentsInChildren<RankingElement>();
    }

    void ShowRanking()
    {
        CheckRecord();
        SaveRanking();
        DisplayUI();
    }

    void CheckRecord()
    {
        float gameTime = GameManager.Instance.GameTime;
        int summons = GameManager.Instance.player.Summons.Count;

        Record newRecord = new Record(gameTime, summons);

        if (records.Count < 10)
        {
            records.Add(newRecord);
        }
        else
        {
            records.Sort(CompareRecords);

            Record lowestRecord = records[records.Count - 1];
            int comparison = CompareRecords(newRecord, lowestRecord);

            if (comparison < 0)
            {
                records[records.Count - 1] = newRecord;
            }
        }

        records.Sort(CompareRecords);
    }

    int CompareRecords(Record a, Record b)
    {
        int timeComparison = b.time.CompareTo(a.time);
        if (timeComparison != 0)
            return timeComparison;

        return b.summons.CompareTo(a.summons);
    }

    void DisplayUI()
    {
        for (int i = 0; i < records.Count; i++)
        {
            rankingElements[i].SetRank(i + 1, records[i].time, records[i].summons);
        }
    }

    void SaveRanking()
    {
        for (int i = 0; i < records.Count; i++)
        {
            PlayerPrefs.SetFloat($"Record_Time_{i}", records[i].time);
            PlayerPrefs.SetInt($"Record_Summons_{i}", records[i].summons);
        }
        PlayerPrefs.Save();
    }

    void LoadRanking()
    {
        records.Clear();

        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.HasKey($"Record_Time_{i}") && PlayerPrefs.HasKey($"Record_Summons_{i}"))
            {
                float time = PlayerPrefs.GetFloat($"Record_Time_{i}");
                int summons = PlayerPrefs.GetInt($"Record_Summons_{i}");
                records.Add(new Record(time, summons));
            }
            else
            {
                records.Add(new Record(0, 0));
            }
        }

        records.Sort(CompareRecords);
    }
}

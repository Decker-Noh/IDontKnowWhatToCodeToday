using UnityEngine;
using System;
using TMPro;


public class Summon : Follower
{
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

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        OnChangedLevel += OnChangedLevelEvent;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        Level = 1;
    }

    void OnChangedLevelEvent(int changedLevel)
    {
        levelText.text = changedLevel.ToString();
    }


}

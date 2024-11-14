using UnityEngine;

public interface IEatable
{
    //* 먹을 수 있는 것들은 전부 점수가 될 수 있다.


    public string GetName();
    public int GetLevel();

    public void OnAteEvent();

}

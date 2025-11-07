using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameProgressManager
{
    private const string Key = "UnlockedStage";

    public static int GetUnlockedStage()
    {
        return PlayerPrefs.GetInt(Key, 1); // 初期解放はステージ1
    }

    public static void UnlockStage(int stage)
    {
        int current = GetUnlockedStage();
        if (stage > current)
        {
            PlayerPrefs.SetInt(Key, stage);
            PlayerPrefs.Save();
        }
    }
}

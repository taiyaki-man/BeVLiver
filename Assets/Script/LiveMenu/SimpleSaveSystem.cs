// ===============================
// LiveMenu/SimpleSaveSystem.cs
// まずは PlayerPrefs ベースの超簡易セーブ
// 本番では JSON/セーブデータ管理に置換推奨
// ===============================
using UnityEngine;

public static class SimpleSaveSystem
{
    const string KEY_CURRENCY = "save.currency";
    const string KEY_DATETIME = "save.datetime";

    public static void SaveNow()
    {
        // 例: 所持金や現在ステージ等、必要な値を保存
        int fakeCurrency = Random.Range(100, 1000); // デモ値
        PlayerPrefs.SetInt(KEY_CURRENCY, fakeCurrency);
        PlayerPrefs.SetString(KEY_DATETIME, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        PlayerPrefs.Save();
    }

    public static (int currency, string datetime) Load()
    {
        int c = PlayerPrefs.GetInt(KEY_CURRENCY, 0);
        string t = PlayerPrefs.GetString(KEY_DATETIME, "N/A");
        return (c, t);
    }
}
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ボタン1個にアタッチして画面遷移を実現する、単体スクリプト。
/// - Button の onClick に自動で登録されます（Awake）。
/// - targetSceneName に Build Settings 登録済みのシーン名（拡張子なし）を設定してください。
/// - 同期（LoadScene）/ 非同期（LoadSceneAsync）を切替可能。
/// 使い方：
///   1) 任意の Button に本コンポーネントをアタッチ
///   2) targetSceneName に "LiveScene" など遷移先シーン名を入力
///   3) File > Build Settings... で該当シーンが登録されていることを確認
///   4) 再生し、ボタンを押すと遷移
/// </summary>
[AddComponentMenu("VTuberChat/Button Scene Loader (Single Script)")]
[RequireComponent(typeof(Button))]
public class ButtonSceneLoader : MonoBehaviour
{
    public enum LoadType { Sync, Async }

    [Header("遷移先シーン名（Build Settings 登録済み / 拡張子なし）")]
    [Tooltip("例: LiveScene / ResultScene など。大文字小文字は無視されます。")]
    public string targetSceneName = "LiveScene";

    [Header("ロード方式")]
    public LoadType loadType = LoadType.Async;

    [Tooltip("読み込み中に同ボタンを押せないようにする")]
    public bool disableButtonDuringLoad = true;

    private Button _button;
    private static bool _isLoading = false;

    private void Reset()
    {
        // 便宜的に、子にあるTMP_Textの文字列をシーン名候補として拾っておく（任意）
        var tmp = GetComponentInChildren<TMPro.TMP_Text>();
        if (tmp) targetSceneName = tmp.text.Trim();
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        if (_button != null) _button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogWarning($"{name}: targetSceneName が未設定です。");
            return;
        }

        if (!IsSceneInBuild(targetSceneName))
        {
            Debug.LogError($"{name}: シーン '{targetSceneName}' は Build Settings に登録されていません。File > Build Settings... で追加してください。");
            return;
        }

        if (loadType == LoadType.Sync)
        {
            // 同期ロード：簡単・即切替（次フレーム完了）。重いシーンでは一瞬固まることもあります。
            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
        }
        else
        {
            if (_isLoading) return; // 二重押下ガード
            StartCoroutine(LoadAsyncRoutine(targetSceneName));
        }
    }

    private System.Collections.IEnumerator LoadAsyncRoutine(string sceneName)
    {
        _isLoading = true;
        if (disableButtonDuringLoad && _button) _button.interactable = false;

        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        // 進捗バーを作るなら op.progress (0..0.9) を参照。0.9到達後、isDoneになるまで待機。
        while (!op.isDone)
        {
            yield return null;
        }

        _isLoading = false; // 通常は到達前にシーン遷移で破棄されます
    }

    /// <summary>
    /// Build Settings に登録されている全シーンから名前を探す（拡張子なし名で比較）。
    /// </summary>
    public static bool IsSceneInBuild(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i); // 例: Assets/Scenes/LiveScene.unity
            var name = Path.GetFileNameWithoutExtension(path);
            if (string.Equals(name, sceneName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}

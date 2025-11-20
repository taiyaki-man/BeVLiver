// ===============================
// LiveMenu/MenuButtons.cs
// ボタンの onClick から呼び出す窓口
// ===============================
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LiveMenu
{
    public class MenuButtons : MonoBehaviour
    {
        [SerializeField] private string startSceneName = "StartScene"; // ビルド設定に登録必須
        [SerializeField] private GameObject inventoryPanel;             // インベントリUI（後述のプレースホルダでもOK）
        [SerializeField] private GameObject optionsPanel;               // まだ未実装でも空のPanelを参照可

        public void OnClick_BackToStart()
        {
            // 必要に応じてセーブ・確認ダイアログを挟む
            SceneManager.LoadScene(startSceneName);
        }

        public void OnClick_Save()
        {
            SimpleSaveSystem.SaveNow();
#if UNITY_EDITOR
            Debug.Log("[Save] ゲーム進行をセーブしました（ダミー）。");
#endif
        }

        public void OnClick_Options()
        {
            if (optionsPanel) optionsPanel.SetActive(true); // 後で中身を作成
        }

        public void OnClick_Inventory()
        {
            if (inventoryPanel) inventoryPanel.SetActive(true);
        }
    }
}



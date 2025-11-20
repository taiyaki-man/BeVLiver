// ===============================
// LiveMenu/InventoryPlaceholder.cs
// インベントリの簡易プレースホルダ（閉じるボタン用）
// ===============================
using UnityEngine;

namespace LiveMenu
{
    public class InventoryPlaceholder : MonoBehaviour
    {
        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}

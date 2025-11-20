
// ===============================
// LiveMenu/MenuHotkey.cs
// キー入力（例: Esc / M）でメニューの開閉
// ===============================
using UnityEngine;

namespace LiveMenu
{
    public class MenuHotkey : MonoBehaviour
    {
        [SerializeField] private SlideMenuController slideMenu;
        [SerializeField] private KeyCode toggleKey = KeyCode.Escape;

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                slideMenu.Toggle();
            }
        }
    }
}

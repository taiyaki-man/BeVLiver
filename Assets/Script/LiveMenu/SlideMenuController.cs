// ===============================
// LiveMenu/SlideMenuController.cs
// 横からスライドインするメニューと半透明オーバーレイを制御
// ===============================
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LiveMenu
{
    public class SlideMenuController : MonoBehaviour
    {
        [Header("Menu Panel (left slide)")]
        [SerializeField] private RectTransform menuPanel; // 幅は任意（例: 360）
        [SerializeField] private float slideDuration = 0.25f; // 体感に合わせて調整
        [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0,0,1,1);

        [Header("Overlay (半透明) ※Image+CanvasGroup推奨")]
        [SerializeField] private CanvasGroup overlayGroup; // 画面全体のブロッカー
        [SerializeField] private Image overlayImage;       // Color a=0→0.5 へ
        [Range(0f, 1f)]
        [SerializeField] private float overlayTargetAlpha = 0.5f; // 背景の暗さ

        [Header("State")]        
        [SerializeField] private bool isOpen = false; // 初期状態

        private Vector2 closedPos; // 画面外（左）
        private Vector2 openPos;   // 画面内（左寄せ）
        private Coroutine animCo;

        void Awake()
        {
            if (!menuPanel) Debug.LogError("SlideMenuController: menuPanel 未設定");
            if (!overlayGroup) Debug.LogError("SlideMenuController: overlayGroup 未設定");
            if (!overlayImage) Debug.LogError("SlideMenuController: overlayImage 未設定");

            // menuPanel のアンカーは左揃え推奨（Min=(0,0), Max=(0,1), Pivot=(0,0.5)）
            float width = menuPanel.rect.width; // 実幅
            openPos = Vector2.zero;             // 左端に表示
            closedPos = new Vector2(-width, 0); // 左の外に退避

            // 初期配置
            menuPanel.anchoredPosition = isOpen ? openPos : closedPos;
            SetOverlayInstant(isOpen ? overlayTargetAlpha : 0f);
            SetOverlayBlockRaycasts(isOpen);
        }

        public void Toggle()
        {
            if (animCo != null) StopCoroutine(animCo);
            animCo = StartCoroutine(Animate(!isOpen));
        }

        public void Open()
        {
            if (isOpen) return;
            if (animCo != null) StopCoroutine(animCo);
            animCo = StartCoroutine(Animate(true));
        }

        public void Close()
        {
            if (!isOpen) return;
            if (animCo != null) StopCoroutine(animCo);
            animCo = StartCoroutine(Animate(false));
        }

        private IEnumerator Animate(bool open)
        {
            isOpen = open;
            float t = 0f;
            Vector2 start = menuPanel.anchoredPosition;
            Vector2 end = open ? openPos : closedPos;
            float startAlpha = overlayGroup.alpha;
            float endAlpha = open ? overlayTargetAlpha : 0f;

            // 開く直前にブロックを有効化
            if (open) SetOverlayBlockRaycasts(true);

            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / Mathf.Max(0.0001f, slideDuration);
                float k = ease.Evaluate(Mathf.Clamp01(t));
                menuPanel.anchoredPosition = Vector2.LerpUnclamped(start, end, k);

                float a = Mathf.Lerp(startAlpha, endAlpha, k);
                var c = overlayImage.color; c.a = a; overlayImage.color = c;
                overlayGroup.alpha = a;
                yield return null;
            }

            // 閉じ終わりでブロック無効化
            if (!open) SetOverlayBlockRaycasts(false);
        }

        private void SetOverlayInstant(float alpha)
        {
            overlayGroup.alpha = alpha;
            var c = overlayImage.color; c.a = alpha; overlayImage.color = c;
        }

        private void SetOverlayBlockRaycasts(bool block)
        {
            overlayGroup.blocksRaycasts = block; // 背後クリックを遮断
            overlayGroup.interactable = block;    // Tab移動なども遮断
        }

        // オーバーレイタップでクローズ（EventTrigger or Buttonを付けて呼ぶ）
        public void OnOverlayClicked()
        {
            Close();
        }
    }
}


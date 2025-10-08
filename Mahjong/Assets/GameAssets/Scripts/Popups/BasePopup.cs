using UnityEngine;
using Game.Managers;
using DG.Tweening;

namespace Game.Popups
{
    public abstract class BasePopup : MonoBehaviour
    {
        protected PopupManager PopupManager => DependencyManager.Instance.PopupManager;

        [Header("Popup Animation Settings")]
        [SerializeField] private float duration = 0.35f;
        [SerializeField] private Ease showEase = Ease.OutBack;  // nice bounce
        [SerializeField] private Ease hideEase = Ease.InBack;

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Tween currentTween;

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            rectTransform = GetComponent<RectTransform>();

            // Prepare hidden state
            canvasGroup.alpha = 0f;
            rectTransform.localScale = Vector3.one * 0.8f;
        }

        public virtual void Show(bool KeepLastPopupActive = false)
        {
            currentTween?.Kill();
            gameObject.SetActive(true);
            rectTransform.SetAsLastSibling();
            var Clip = DependencyManager.Instance.GameManager.GetClipByID("Popup");
            DependencyManager.Instance.SoundManager.PlaySFX(Clip);
            // Reset start state
            canvasGroup.alpha = 0f;
            rectTransform.localScale = Vector3.one * 0.8f;

            // Animate fade + scale
            currentTween = DOTween.Sequence()
                .Append(canvasGroup.DOFade(1f, duration))
                .Join(rectTransform.DOScale(1f, duration).SetEase(showEase))
                .OnComplete(() =>
                {
                    PopupManager.SetActivePopup(this, KeepLastPopupActive);
                });
        }

        public virtual void Hide()
        {
            currentTween?.Kill();

            // Animate fade + scale down, then deactivate
            currentTween = DOTween.Sequence()
                .Append(canvasGroup.DOFade(0f, duration))
                .Join(rectTransform.DOScale(0.8f, duration).SetEase(hideEase))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    PopupManager.HideActivePopup(this);
                });
        }
    }
}

using UnityEngine;
using Game.Managers;
using System;

namespace Game.Screens
{
    public abstract class BaseScreen : MonoBehaviour
    {
        protected ScreenManager ScreenManager => DependencyManager.Instance.ScreenManager;

        public virtual void Show(bool KeepLastPopupActive = false)
        {
            gameObject.GetComponent<RectTransform>().SetAsLastSibling();
            gameObject.SetActive(true);
            ScreenManager.SetActiveScreen(this, KeepLastPopupActive);

        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            ScreenManager.HideActiveScreen(this);
        }

    }
}
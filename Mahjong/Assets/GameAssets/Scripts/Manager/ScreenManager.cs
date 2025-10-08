using System;
using Game.Screens;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Game.Managers
{
    public class ScreenManager : MonoBehaviour
    {
        private Stack<BaseScreen> CacheScreen = new Stack<BaseScreen>();

        //public GameObject Container;
        public List<BaseScreen> Screens;

        public TScreen GetScreen<TScreen>()
        where TScreen : BaseScreen
        {
            Type RequestType = typeof(TScreen);
            var Screen = (TScreen)Screens.FirstOrDefault(x => x.GetType() == RequestType);
            return Screen;
        }

        public void HideActiveScreen(BaseScreen Screen)
        {
            CacheScreen?.Pop();
            if (CacheScreen.Count > 0)
            {
                var LastPopup = CacheScreen.Peek();
                LastPopup.gameObject.SetActive(true);
            }
            ShowCacheScreen();
        }

        public void SetActiveScreen(BaseScreen Popup, bool KeepLastPopupActive)
        {
            if (CacheScreen.Count > 0)
            {
                var LastPopup = CacheScreen.Peek();
                LastPopup.gameObject.SetActive(KeepLastPopupActive);
            }
            CacheScreen.Push(Popup);
            ShowCacheScreen();
        }

        //public List<BasePopup> CacheList = new List<BasePopup>();
        private void ShowCacheScreen()
        {
            //CacheList = CachePopups.ToList();
        }
    }
}
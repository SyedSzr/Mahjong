using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Popups;
using Game.Managers;

namespace Game.Utilities
{
    public class ButtonLeaderboard : ButtonComponent
    {
        public override void OnButtonClicked()
        {
            base.OnButtonClicked();
            //var Popup = DependencyManager.Instance.PopupManager.GetPopup<PopupLeaderboard>();
            //Popup.Show();
            DependencyManager.Instance.GooglePlayServicesManager.ShowLeaderboardUI();
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Popups;

namespace Game.Utilities
{
    public class ButtonDailyLeaderboard : ButtonComponent
    {
        
        public override void OnButtonClicked()
        {
            base.OnButtonClicked();
            var Popup= DependencyManager.Instance.PopupManager.GetPopup<PopupLeaderboard>();
            Popup.Show();
        }
    }
}
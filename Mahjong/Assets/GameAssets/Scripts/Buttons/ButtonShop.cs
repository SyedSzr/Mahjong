using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Popups;
using Game.Managers;

namespace Game.Utilities
{
    public class ButtonShop : ButtonComponent
    {
        public override void OnButtonClicked()
        {
            base.OnButtonClicked();
            var Popup = DependencyManager.Instance.PopupManager.GetPopup<PopupShop>();
            Popup.Show();
        }
    }
}
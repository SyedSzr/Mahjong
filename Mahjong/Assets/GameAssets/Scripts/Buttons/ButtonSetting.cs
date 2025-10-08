using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Popups;
using Game.Managers;

namespace Game.Utilities
{
    public class ButtonSetting : ButtonComponent
    {
        public override void OnButtonClicked()
        {
            base.OnButtonClicked();
            DependencyManager.Instance.MultilayerLevelGenerator.ActionUnselectAllTile?.Invoke();
            var Popup = DependencyManager.Instance.PopupManager.GetPopup<PopupSetting>();
            Popup.Show();
        }
    }
}
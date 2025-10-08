using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Popups;

namespace Game.Utilities
{
    public class ButtonHint : ButtonComponent
    {
        public override void OnButtonClicked()
        {
            base.OnButtonClicked();
            var Hint = DependencyManager.Instance.PlayerStateManager.PlayerState.Hint;
            AudioClipName = Hint < 1 ? "Click" : "Hint";

            if (Hint > 0)
                DependencyManager.Instance.MatchManager.HighlightAllMatches();
            else
                DependencyManager.Instance.PopupManager.GetPopup<PopupShop>().Show();

        }
    }
}
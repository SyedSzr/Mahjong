using System.Collections;
using System.Collections.Generic;
using Game.Popups;
using UnityEngine;

namespace Game.Utilities
{
    public class ButtonHide : ButtonComponent
    {
        private BasePopup mPopup;
        private BasePopup Popup
        {
            get
            {
                if (mPopup == null)
                {
                    mPopup = GetComponentInParent<BasePopup>();
                }
                return mPopup;
            }
        }

        public override void OnButtonClicked()
        {
            base.OnButtonClicked();
            Popup.Hide();
        }
    }
}
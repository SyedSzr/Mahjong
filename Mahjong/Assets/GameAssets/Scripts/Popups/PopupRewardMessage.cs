using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;

namespace Game.Popups
{
    public class PopupRewardMessage : BasePopup
    {
        public Button ButtonSuccess;
        public TMP_Text TextHeader;
        public TMP_Text TextMessage;
        public Image ImageTitle;
        Action CallBack;

        private void Start()
        {
            ButtonSuccess.onClick.AddListener(Hide);
        }

        public void Setup(string Header, string Message,string ID, Action CallBack)
        {
            var Icon = DependencyManager.Instance.GameConfigurationManager.IconData.IconSettings.FirstOrDefault(x => x.ID == ID)?.Icon;
            ImageTitle.sprite = Icon;
            this.CallBack = CallBack;
            TextHeader.text = Header;
            TextMessage.text = Message;
        }
        public override void Hide()
        {
            base.Hide();
            CallBack?.Invoke();
        }
    }
}
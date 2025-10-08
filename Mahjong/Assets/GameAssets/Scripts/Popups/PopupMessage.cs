using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
namespace Game.Popups
{
    public class PopupMessage : BasePopup
    {
        public Button ButtonSuccess;
        public TMP_Text TextHeader;
        public TMP_Text TextMessage;
        Action CallBack;

        private void Start()
        {
            ButtonSuccess.onClick.AddListener(HidePopup);
        }

        public void Setup(string Header, string Message, Action CallBack)
        {
            this.CallBack = CallBack;
            TextHeader.text = Header;
            TextMessage.text = Message;
        }
        public void HidePopup()
        {
            base.Hide();
            CallBack?.Invoke();
        }

    }
}
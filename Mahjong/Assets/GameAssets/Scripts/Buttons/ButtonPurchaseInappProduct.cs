using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using System.Linq;
using Game.Popups;
using Game.Settings;
using System;
using Game.Utilities;
using Firebase.Analytics;

namespace Game.Utilities
{
    public class ButtonPurchaseInappProduct : ButtonComponent
    {
        public string ID;
        public ShopItemSetting ShopItemSetting;
        public TextComponent TextPrice;
        public override void Start()
        {
            base.Start();
            ShopItemSetting = DependencyManager.Instance.GameConfigurationManager.ShopData.ShopItemSettings.
                FirstOrDefault(x => x.PackegeName == ID);
        }

        public override void OnButtonClicked()
        {
            base.OnButtonClicked();
            DependencyManager.Instance.InAppManager.PurchaseItem(ID, ID, OnSucess, OnFailed);
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPurchase, new Parameter(ShopItemSetting.PackegeName, ShopItemSetting.Title));

        }

        private void OnFailed(string _Message)
        {
            var popup = DependencyManager.Instance.PopupManager.GetPopup<PopupMessage>();
            popup.Setup("Oops!", _Message +" "+ ShopItemSetting.Title, null);
            popup.Show();
        }

        private void OnSucess(string _Message)
        {
            var popup = DependencyManager.Instance.PopupManager.GetPopup<PopupRewardMessage>();
            popup.Setup("Congrats!", _Message + " " + ShopItemSetting.Description, ShopItemSetting.IconID, GrantReward);
            popup.Show();
            var Clip = DependencyManager.Instance.GameManager.GetClipByID("Purchased");
            DependencyManager.Instance.SoundManager.PlaySFX(Clip);
        }

        private void GrantReward()
        {
            var State = DependencyManager.Instance.PlayerStateManager.PlayerState;
            foreach (var item in ShopItemSetting.PurchaseableItems)
            {
                Debug.Log(item.ItemID);
                if(item.ItemID== "Shuffle")
                {
                    State.Shuffle += item.Quanity;
                }
                if(item.ItemID== "Hint")
                {
                    State.Hint += item.Quanity;
                }
                if(item.ItemID== "NoAds")
                {
                    Debug.Log("No Ads Purchased");
                    State.NoAds = true;
                }
            }

            DependencyManager.Instance.GameManager.ActionUpdateStateItems?.Invoke();
        }
    }
}
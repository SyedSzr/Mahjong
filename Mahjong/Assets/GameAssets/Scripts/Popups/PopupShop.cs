using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using System.Linq;
using Game.Components;

namespace Game.Popups
{
    public class PopupShop : BasePopup
    {
        public GameObject ShopItem;
        public Transform ContentView;
        public bool IsAlreadySetup;
        public override void Show(bool KeepLastPopupActive = false)
        {
            base.Show(KeepLastPopupActive);
            if (IsAlreadySetup)
                return;
            Setup();
        }

        public void Setup()
        {
            var InGameShopItems = DependencyManager.Instance.GameConfigurationManager.ShopData
                .ShopItemSettings.Where(x => !x.IsSpecial);
            foreach (var item in InGameShopItems)
            {
                var Obj = Instantiate(ShopItem, ContentView);
                Obj.GetComponent<ShopItemComponent>().Setup(item);
            }
            IsAlreadySetup = true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using Game.Settings;
using System.Linq;
using Game.Utilities;

namespace Game.Components
{
    public class ShopItemComponent : MonoBehaviour
    {
        public Image ItemIcon;
        public GameObject ShopItemTitleComponent;
        public TextComponent Title;
        public TextComponent TextPrice;
        public Transform ContentParent;
        public ButtonPurchaseInappProduct InAppButton;

        public void Setup(ShopItemSetting ShopItemSetting)
        {
            var Icon = DependencyManager.Instance.GameConfigurationManager.IconData.IconSettings.FirstOrDefault(x => x.ID == ShopItemSetting.IconID).Icon;
            ItemIcon.sprite = Icon;
            for (int i = 0; i < ShopItemSetting.PurchaseableItems.Count; i++)
            {
                PurchaseableItems item = ShopItemSetting.PurchaseableItems[i];
                var obj = Instantiate(ShopItemTitleComponent, ContentParent);
                obj.GetComponent<ShopItemTitleComponent>().Setup(item.ItemID, item.Quanity);
            }
            var Price = DependencyManager.Instance.InAppManager.GetPrice(ShopItemSetting.PackegeName);
            Debug.Log(Price);
            TextPrice.SetupText(Price.ToString());
            Title.SetupText(ShopItemSetting.Title);
            InAppButton.ID = ShopItemSetting.PackegeName;
            //TextTitle.text = ShopItemSetting.Title;
            //TextCount.text
        }
    }
}
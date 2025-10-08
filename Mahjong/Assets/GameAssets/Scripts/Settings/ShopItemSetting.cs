using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings
{
    [System.Serializable]
    public class PurchaseableItems
    {
        public string ItemID;
        public int Quanity;
    }
    [System.Serializable]
    public class ShopItemSetting
    {
        public string PackegeName;
        public string Title;
        public string IconID;
        public string Description;
        public List<PurchaseableItems> PurchaseableItems;
        public bool IsSpecial;
        public bool Consumable;
    }
}
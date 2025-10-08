using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings
{
    [System.Serializable]
    public class StoreItemSetting
    {
        public string ID;
        public string PackegeName;
        public int PurchasedItemCount;
        public string Description;
        public string Price;
        public bool Consumable;
    }
}
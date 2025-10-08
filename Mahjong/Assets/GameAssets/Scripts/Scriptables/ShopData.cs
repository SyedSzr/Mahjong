using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Settings;

namespace Game.Scriptables
{
    [CreateAssetMenu(menuName = "Mahjong/ShopData")]
    public class ShopData : ScriptableObject
    {
        public List<ShopItemSetting> ShopItemSettings;
    }
}
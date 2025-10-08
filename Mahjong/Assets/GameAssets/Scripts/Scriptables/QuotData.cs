using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Settings;

namespace Game.Scriptables
{
    [CreateAssetMenu(menuName = "Mahjong/QuotData")]
    public class QuotData : ScriptableObject
    {
        public List<string> QuotationSettings;
    }
}

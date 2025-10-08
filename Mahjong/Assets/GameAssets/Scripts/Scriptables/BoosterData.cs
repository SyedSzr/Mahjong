using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Settings;

namespace Game.Scriptables
{
    [CreateAssetMenu(menuName = "Mahjong/BoosterData")]
    public class BoosterData : ScriptableObject
    {
        public List<BoosterSetting> BoosterSettings;
    }
}
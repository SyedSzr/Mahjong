using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Settings;

namespace Game.Scriptables
{
    [CreateAssetMenu(menuName = "Mahjong/DailyRewardData")]
    public class DailyRewardData : ScriptableObject
    {
        public List<DailyRewardSetting> DailyRewards;

    }
}
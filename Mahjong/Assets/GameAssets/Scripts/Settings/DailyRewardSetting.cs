using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings
{
    [System.Serializable]
    public class DailyRewardSetting
    {
        public string ID;
        public List<RewardSetting> Rewards;      // Rewards for this day
    }
}
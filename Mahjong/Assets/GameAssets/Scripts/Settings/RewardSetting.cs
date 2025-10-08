using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings
{
    [System.Serializable]
    public class RewardSetting
    {
        public int RewardID;      // Unique ID for reward
        public string IconID;       // Icon reference
        public int Count;         // Amount (coins, hints, etc.)
    }
}
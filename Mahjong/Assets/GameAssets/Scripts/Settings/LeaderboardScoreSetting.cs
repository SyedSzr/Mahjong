using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings
{
    [System.Serializable]
    public class LeaderboardScoreSetting
    {
        public int ID;
        public string Name;
        public long Score;
    }
}
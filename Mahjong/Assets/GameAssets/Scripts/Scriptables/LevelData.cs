using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Settings;

namespace Game.Scriptables
{
    [CreateAssetMenu(menuName = "Mahjong/LevelData")]
    public class LevelData : ScriptableObject
    {
        public List<LevelSetting> LevelSettings;
    }
}
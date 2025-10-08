using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Settings;

namespace Game.Scriptables
{
    [CreateAssetMenu(menuName = "Mahjong/SoundData")]
    public class SoundData : ScriptableObject
    {
        public List<SoundSetting> SoundSettings;
    }
}
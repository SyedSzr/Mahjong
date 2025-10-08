using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Settings;

namespace Game.Scriptables
{
    [CreateAssetMenu(menuName ="Mahjong/IconData")]
    public class IconData : ScriptableObject
    {
        public List<IconSetting> IconSettings;
    }
}
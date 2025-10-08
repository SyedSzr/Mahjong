using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Settings;

namespace Game.Scriptables
{
    [CreateAssetMenu(menuName ="Mahjong/TileData")]
    public class TileData : ScriptableObject
    {
        public List<TileSetting> TileSettings;
    }
}
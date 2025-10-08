using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings
{
    [System.Serializable]
    public class LevelSetting
    {
        public float ShapeNoise;
        public List<TileLayerConfig> LayerConfigs;
    }
}
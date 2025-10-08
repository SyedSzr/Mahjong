using UnityEngine;
using Game.Scriptables;
using System.Collections.Generic;
using System.Linq;

namespace Game.Managers
{
    public class GameConfigurationManager : MonoBehaviour
    {
        public TileData TileData;
        public IconData IconData;
        public IconData GameIcon;
        public LevelData LevelData;
        public ShopData ShopData;
        public QuotData QuotData;
        public SoundData SoundData;
        public DailyRewardData DailyRewardData;
        public DailyRewardData RewardData;
    }
}
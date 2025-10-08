using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using Game.Settings;
using Game.Utilities;
using System.Linq;
namespace Game.Components
{
    public class RewardPackComponent : MonoBehaviour
    {
        public Image Icon;
        public TextComponent TitleText;
        public TextComponent RewardCountText;

        public void Setup(string IconID, string RewardCount)
        {
            var IconSprite =DependencyManager.Instance.GameConfigurationManager.IconData.IconSettings.FirstOrDefault(x => x.ID == IconID).Icon;
            Icon.sprite = IconSprite;
            RewardCountText.SetupText(RewardCount);
            TitleText.SetupText(IconID);
        }
    }
}
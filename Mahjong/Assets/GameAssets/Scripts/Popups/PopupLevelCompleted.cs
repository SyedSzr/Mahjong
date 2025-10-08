using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Utilities;
using TMPro;


namespace Game.Popups
{
    public class PopupLevelCompleted : BasePopup
    {
        public TextComponent textMeshPro;   // Assign in Inspector
        public TextComponent TextHeader;
        public TextComponent TextScore;
        private GameConfigurationManager Config => DependencyManager.Instance.GameConfigurationManager;
        private void OnEnable()
        {
            var Quots = Config.QuotData.QuotationSettings;
            var Quot = Quots[Random.Range(0, Quots.Count)];
            textMeshPro.ShowTextTypewriteEffect(Quot);
            TextHeader.ShowTextTypewriteEffect(TextHeader.GetText(),.025f);
            TextScore.ShowCounter(0, DependencyManager.Instance.GameManager.Score);
        }

        private void OnDisable()
        {
            var dailyRewardData = Config.RewardData;

            var TodayReward = dailyRewardData.DailyRewards[DependencyManager.Instance.PlayerStateManager.PlayerState.Level%dailyRewardData.DailyRewards.Count];

            var Popup = DependencyManager.Instance.PopupManager.GetPopup<PopupDailyReward>();
            Popup.Setup(TodayReward.Rewards, "Level " + (DependencyManager.Instance.PlayerStateManager.PlayerState.Level+1) + " Reward");
            Popup.Show();
        }
       
    }
}
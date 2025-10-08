using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Scriptables;
using Game.Settings;
using Game.Popups;
using System.Collections;

namespace Game.Managers
{
    public class DailyRewardManager : MonoBehaviour
    {
        private const string LastClaimDateKey = "LastClaimDate";
        private const string StreakKey = "DailyStreak";

        [Header("Daily Reward Data")]
        public DailyRewardData dailyRewardData;

        public int CurrentStreak;
        public bool CanClaim;
        public DailyRewardSetting TodayReward;

        // Reward handlers map

        private void Start()
        {
            DependencyManager.Instance.GameManager.ActionDataLoaded+=LoadData;
           

        }

        private void LoadData()
        {
            LoadProgress();
            CheckDailyReward();
            ClaimReward();
        }

        private void LoadProgress()
        {
            CurrentStreak = PlayerPrefs.GetInt(StreakKey, 0);
        }

        private void SaveProgress()
        {
            PlayerPrefs.SetInt(StreakKey, CurrentStreak);
            PlayerPrefs.SetString(LastClaimDateKey, DateTime.Today.ToString());
            PlayerPrefs.Save();
        }

        private void CheckDailyReward()
        {
            string lastDate = PlayerPrefs.GetString(LastClaimDateKey, "");
            DateTime today = DateTime.Today;

            if (string.IsNullOrEmpty(lastDate))
            {
                CanClaim = true;
            }
            else
            {
                DateTime lastClaim = DateTime.Parse(lastDate);

                if (lastClaim == today)
                {
                    CanClaim = false; // already claimed today
                }
                else if (lastClaim == today.AddDays(-1))
                {
                    CanClaim = true; // continue streak
                }
                else
                {
                    CurrentStreak = 0; // reset streak
                    CanClaim = true;
                }
            }
            int rewardIndex = CurrentStreak % dailyRewardData.DailyRewards.Count;
            TodayReward = dailyRewardData.DailyRewards[rewardIndex];
        }

        public void ClaimReward()
        {
            if (!CanClaim || TodayReward == null) return;

            var Popup = DependencyManager.Instance.PopupManager.GetPopup<PopupDailyReward>();
            Popup.Setup(TodayReward.Rewards, "Day "+TodayReward.ID+" Reward");
            Popup.Show();
            var Clip = DependencyManager.Instance.GameManager.GetClipByID("Purchased");
            DependencyManager.Instance.SoundManager.PlaySFX(Clip);

            CurrentStreak++;
            SaveProgress();
            CanClaim = false;
        }
    }
}

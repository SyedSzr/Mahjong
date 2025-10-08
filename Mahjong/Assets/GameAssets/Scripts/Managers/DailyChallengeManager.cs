using System;
using UnityEngine;
using Game.Scriptables;
using Game.Settings;

namespace Game.Managers
{
    public class DailyChallengeManager : MonoBehaviour
    {
        private const string LastChallengeDateKey = "LastChallengeDate";
        private const string ChallengeCompletedKey = "ChallengeCompleted";

        [Header("Data")]
        public DailyRewardManager rewardManager;

        public bool ChallengeAvailable { get; private set; }
        public bool ChallengeCompleted { get; private set; }
        public DateTime NextAvailableTime { get; private set; }

        private void Awake()
        {
            LoadProgress();
            CheckAvailability();
        }

        private void LoadProgress()
        {
            string lastDate = PlayerPrefs.GetString(LastChallengeDateKey, "");
            ChallengeCompleted = PlayerPrefs.GetInt(ChallengeCompletedKey, 0) == 1;

            if (!string.IsNullOrEmpty(lastDate))
            {
                DateTime lastPlayed = DateTime.Parse(lastDate);
                NextAvailableTime = lastPlayed.AddDays(1); // available again after 24hr
            }
            else
            {
                NextAvailableTime = DateTime.MinValue; // never played
            }
        }

        private void SaveProgress()
        {
            PlayerPrefs.SetString(LastChallengeDateKey, DateTime.Now.ToString());
            PlayerPrefs.SetInt(ChallengeCompletedKey, ChallengeCompleted ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void CheckAvailability()
        {
            if (DateTime.Now >= NextAvailableTime)
            {
                ChallengeAvailable = true;
                ChallengeCompleted = false;
            }
            else
            {
                ChallengeAvailable = false;
            }
        }

        /// <summary>
        /// Call when player starts the daily challenge
        /// </summary>
        public bool StartChallenge()
        {
            CheckAvailability();

            if (!ChallengeAvailable)
            {
                Debug.Log("Daily Challenge not available yet!");
                return false;
            }

            Debug.Log("Daily Challenge Started!");
            return true;
        }

        /// <summary>
        /// Call when player completes the daily challenge successfully
        /// </summary>
        public void CompleteChallenge()
        {
            if (!ChallengeAvailable || ChallengeCompleted) return;

            ChallengeCompleted = true;
            ChallengeAvailable = false;

            // Grant daily reward
            rewardManager.ClaimReward();

            SaveProgress();
            Debug.Log("Daily Challenge Completed! Reward Granted 🎁");
        }
    }
}

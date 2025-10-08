using System.Collections.Generic;
using System.Linq;
using Game.Settings;
using Game.Components;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Game.Managers
{
    public class LeaderboardManager : MonoBehaviour
    {
        public List<LeaderboardScoreSetting> LeaderboardScoreSettings;

        
        public void LoadTodayLeaderboard(string leaderboardId, int maxResults, System.Action<List<LeaderboardScoreSetting>> onComplete)
        {
            PlayGamesPlatform.Instance.LoadScores(
                leaderboardId,
                LeaderboardStart.TopScores,
                maxResults,
                LeaderboardCollection.Public,
                LeaderboardTimeSpan.Daily,
                (data) =>
                {
                    if (data.Status == ResponseStatus.Success)
                    {
                        LeaderboardScoreSettings.Clear();
                        var scores = data.Scores;

                        if (scores == null || scores.Length == 0)
                        {
                            Debug.Log("⚠️ No scores found for today.");
                            onComplete?.Invoke(LeaderboardScoreSettings);
                            return;
                        }

                // Collect all user IDs
                string[] userIds = scores.Select(s => s.userID).ToArray();

                // Load user profiles for these IDs
                PlayGamesPlatform.Instance.LoadUsers(userIds, (users) =>
                        {
                            var userDict = users.ToDictionary(u => u.id, u => u);

                            int count = 0;
                            foreach (IScore score in scores)
                            {
                                count++;
                                string userId = score.userID;
                                string playerName = userDict.ContainsKey(userId) ? userDict[userId].userName : userId;

                                LeaderboardScoreSetting scoreData = new LeaderboardScoreSetting
                                {
                                    ID = count,
                                    Name = playerName,
                                    Score = score.value
                                };

                                LeaderboardScoreSettings.Add(scoreData);
                            }

                    // ✅ Return final result
                    onComplete?.Invoke(LeaderboardScoreSettings);
                        });
                    }
                    else
                    {
                        Debug.LogError("❌ Failed to load daily leaderboard: " + data.Status);
                        onComplete?.Invoke(new List<LeaderboardScoreSetting>());
                    }
                });
        }


    }
}

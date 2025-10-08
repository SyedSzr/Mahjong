using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GooglePlayServicesManager : MonoBehaviour
{
    void Start()
    {
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        //PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        SignIn();
    }

    void SignIn()
    {
        Social.localUser.Authenticate(success =>
        {
            Debug.Log(success ? "Google Play Games login successful" : "Google Play Games login failed");
        });
    }

    public void ShowLeaderboardUI()
    {
        Social.ShowLeaderboardUI();
    }

    public void PostScore(long score, string leaderboardID)
    {
        Social.ReportScore(score, leaderboardID, success =>
        {
            Debug.Log(success ? "Score posted to leaderboard" : "Failed to post score");
        });
    }
}

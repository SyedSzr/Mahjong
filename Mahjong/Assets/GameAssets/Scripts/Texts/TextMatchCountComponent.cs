using Game.Managers;
using UnityEngine;
using TMPro; // or UnityEngine.UI if using Text
using DG.Tweening;

public class TextMatchCountComponent : MonoBehaviour
{
    public TMP_Text ScoreText; // Assign in inspector
    private int displayedScore = 0;
    private int actualScore = 0;


    private void Start()
    {
        DependencyManager.Instance.MatchManager.ActionUpdateMatchCount += AddScore;
        ScoreText.text = "0";
    }
    public void AddScore(int amount)
    {
        int startValue = displayedScore;
        actualScore = amount;

        DOTween.To(() => startValue, x => {
            displayedScore = x;
            ScoreText.text = displayedScore.ToString();
        }, actualScore, 0.5f); // 0.5 seconds duration
    }
}

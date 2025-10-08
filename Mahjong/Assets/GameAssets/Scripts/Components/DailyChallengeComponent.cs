using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Utilities;
using UnityEngine.UI;

namespace Game.Components
{
    public class DailyChallengeComponent : MonoBehaviour
    {
        public DailyChallengeManager ChallengeManager;
        public ButtonComponent ChallengeButton;

        private void Update()
        {
            ChallengeButton.Button.interactable = ChallengeManager.ChallengeAvailable;
        }

        public void OnChallengeButtonPressed()
        {
            if (ChallengeManager.StartChallenge())
            {
                // Load special daily challenge scene / level
                UnityEngine.SceneManagement.SceneManager.LoadScene("DailyChallengeLevel");
            }
        }

        public void OnChallengeCompleted()
        {
            ChallengeManager.CompleteChallenge();
        }
    }
}
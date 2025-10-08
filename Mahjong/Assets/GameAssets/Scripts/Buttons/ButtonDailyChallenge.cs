using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;

namespace Game.Utilities
{
    public class ButtonDailyChallenge : ButtonComponent
    {
        
        public override void OnButtonClicked()
        {
            base.OnButtonClicked();
            if (ChallengeManager.StartChallenge())
            {
                Debug.Log("CHallenge");
                // Load special daily challenge scene / level
               // UnityEngine.SceneManagement.SceneManager.LoadScene("DailyChallengeLevel");
            }
        }

        public DailyChallengeManager ChallengeManager;

        private void Update()
        {
            Button.interactable = ChallengeManager.ChallengeAvailable;
        }

     

        public void OnChallengeCompleted()
        {
            ChallengeManager.CompleteChallenge();
        }
    }
}
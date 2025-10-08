using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Popups;

namespace Game.Utilities
{
    public class ButtonShuffle : ButtonComponent
    {

        private float cooldownTime = 2f; // seconds

        public override void Start()
        {
            base.Start();
        }
        public override void OnButtonClicked()
        {
            var Shuffle = DependencyManager.Instance.PlayerStateManager.PlayerState.Shuffle;
            AudioClipName = Shuffle <1? "Click" : "Shuffle";
            base.OnButtonClicked();

            Button.interactable = false;
            StartCoroutine(EnableButtonAfterCooldown());

            if (Shuffle>=1)
            {
                Debug.Log(Shuffle);
                DependencyManager.Instance.PlayerStateManager.PlayerState.Shuffle -= 1;
                DependencyManager.Instance.GameManager.ActionUpdateStateItems?.Invoke();
                DependencyManager.Instance.MultilayerLevelGenerator.ShuffleTilesInScene();

            }
            else
            {
                DependencyManager.Instance.PopupManager.GetPopup<PopupShop>().Show();
            }

        }

       
        private IEnumerator EnableButtonAfterCooldown()
        {
            yield return new WaitForSeconds(cooldownTime);

            Button.interactable = true;
        }
    }
}
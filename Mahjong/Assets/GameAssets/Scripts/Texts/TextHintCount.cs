using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;


namespace Game.Utilities
{
    public class TextHintCount : TextComponent
    {
        private void Start()
        {
            UpdateCount();
            DependencyManager.Instance.GameManager.ActionUpdateStateItems += UpdateCount;
        }

        private void UpdateCount()
        {
            var Count = DependencyManager.Instance.PlayerStateManager.PlayerState.Hint;
            SetupText(Count>0?Count.ToString():"");
        }
    }
}
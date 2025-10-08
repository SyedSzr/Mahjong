using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using System;

namespace Game.Utilities
{
    public class TextShuffleCount : TextComponent
    {

        private void Start()
        {
            UpdateCount();
            DependencyManager.Instance.GameManager.ActionUpdateStateItems += UpdateCount;
        }

        private void UpdateCount()
        {
            var Count = DependencyManager.Instance.PlayerStateManager.PlayerState.Shuffle;
            SetupText(Count > 0 ? Count.ToString() : "");
        }
    }
}
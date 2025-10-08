using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;

namespace Game.Utilities
{
    public class TextInGameLevel : TextComponent
    {
        private void Start()
        {
            DependencyManager.Instance.GameManager.ActionUpdateLevel += SetupLevel;
            SetupText("lvl\n" + (DependencyManager.Instance.PlayerStateManager.PlayerState.Level + 1));
        }

        private void SetupLevel()
        {
            var Level = DependencyManager.Instance.PlayerStateManager.PlayerState.Level;
            SetupText("lvl\n" + (Level + 1));
        }
    }
}
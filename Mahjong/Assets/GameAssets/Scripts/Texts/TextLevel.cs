using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using System;

namespace Game.Utilities
{
    public class TextLevel : TextComponent
    {
        private void Start()
        {
            DependencyManager.Instance.GameManager.ActionUpdateLevel += SetupLevel;
            Debug.Log("Register Action");
        }

        private void OnEnable()
        {
             SetupLevel();

        }
        private void SetupLevel()
        {
            var Level = DependencyManager.Instance?.PlayerStateManager?.PlayerState?.Level;
            Debug.Log("Level Loaded "+Level);
            SetupText("Level "+(Level+1));
        }
    }
}
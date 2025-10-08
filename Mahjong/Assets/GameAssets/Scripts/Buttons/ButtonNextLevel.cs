using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;

namespace Game.Utilities
{
    public class ButtonNextLevel : ButtonComponent
    {
        
        public override void OnButtonClicked()
        {
            base.OnButtonClicked();
            DependencyManager.Instance.MultilayerLevelGenerator.ActionGameStart?.Invoke();
        }
    }
}
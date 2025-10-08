using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Components;
using Game.Managers;
using UnityEngine;

namespace Game.Utilities
{
    public class TextTargetItemCount : TextComponent
    {
        public string ID;
        private void Start()
        {
            ID = GetComponentInParent<TargetItemComponent>().ID;
            DependencyManager.Instance.GameManager.ActionTargetUpdate += Updateounter;
        }

        private void OnDestroy()
        {
            if(DependencyManager.Instance.GameManager)
                DependencyManager.Instance.GameManager.ActionTargetUpdate -= Updateounter;

        }

        public void Updateounter()
        {
            var CurrentState = DependencyManager.Instance.MultilayerLevelGenerator.SavedLevelTargets.FirstOrDefault(x => x.TargetValue == ID);
            SetupText(Mathf.Max(0, CurrentState.RequiredCount).ToString());

        }
    }
}
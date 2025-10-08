using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using Game.Settings;
using Game.Utilities;
using System.Linq;
using System;

namespace Game.Components
{
    public class TargetItemComponent : MonoBehaviour
    {
        public TextComponent TextCount;
        public string ID;
        public Image ImageTargetIcon;

        private void Start()
        {
            DependencyManager.Instance.GameManager.ActionMatchedTile += UpdateMatchStatus;
        }

        private void UpdateMatchStatus(string obj)
        {
            var CurrentTile = obj.Split("_");
            if(CurrentTile[0]!="Pic")
            {
                obj = "10";
                Debug.Log(obj);
            }
            if (obj == ID)
            {
                var CurrentState = DependencyManager.Instance.MultilayerLevelGenerator.SavedLevelTargets.FirstOrDefault(x => x.TargetValue == obj);
                CurrentState.RequiredCount -= 1;
                TextCount.SetupText(Mathf.Max(0, CurrentState.RequiredCount).ToString());
            }
            CheckState();
        }

        bool LevelCompleteStatus = false;
        public void CheckState()
        {
            LevelCompleteStatus = false;
            var CurrentStates = DependencyManager.Instance.MultilayerLevelGenerator.SavedLevelTargets;

            LevelCompleteStatus = CurrentStates.Any(x => x.RequiredCount > 0);
            //foreach (var item in CurrentStates)
            //{
            //    if(item.RequiredCount<=0)
            //    {
            //        LevelCompleteStatus = true;
            //    }
            //    else
            //    {
            //        LevelCompleteStatus = false;
            //    }
            //}

            if(!LevelCompleteStatus)
            {
                DependencyManager.Instance.MultilayerLevelGenerator.AutoMatchAllTiles();
            }
        }

        private void OnDestroy()
        {
            if (DependencyManager.Instance.GameManager)
                DependencyManager.Instance.GameManager.ActionMatchedTile -= UpdateMatchStatus;

        }
        public void Setup(int Count, string IconID)
        {
            ID = IconID;
            var Icon = DependencyManager.Instance.GameConfigurationManager.IconData.IconSettings.FirstOrDefault(x => x.ID == IconID).Icon;
            ImageTargetIcon.sprite = Icon;
            TextCount.SetupText(Count.ToString());
        }
    }
}
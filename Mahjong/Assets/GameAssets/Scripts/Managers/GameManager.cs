using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Settings;
using Game.Components;
using System;
using System.Linq;

namespace Game.Managers
{
    public class GameManager : MonoBehaviour
    {
        public Action ActionUpdateLevel;
        public Action ActionUpdateStateItems;
        public Action ActionDataLoaded;
        public Action ActionTileStatus;
        public Action ActionTargetUpdate;
        public Action<string> ActionMatchedTile;

        public int Score;


        public void UpdateLevel()
        {
            var Level =DependencyManager.Instance.PlayerStateManager.PlayerState.Level += 1;
            ActionUpdateLevel?.Invoke();
        }

        public AudioClip GetClipByID(string ID)
        {
           return DependencyManager.Instance.GameConfigurationManager.SoundData.SoundSettings.FirstOrDefault(x => x.ID == ID).clip;
        }
    }
}
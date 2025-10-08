using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;

namespace Game.Screens
{
    public class GamePlayScreen : BaseScreen
    {
        public GameObject TilesParent;
        public override void Show(bool KeepLastPopupActive = false)
        {
            base.Show(KeepLastPopupActive);
            DependencyManager.Instance.MultilayerLevelGenerator.ActionGameStart?.Invoke();
            TilesParent.SetActive(true);

        }

        [ContextMenu("StartGame")]
        public void StartGame()
        {
            DependencyManager.Instance.MultilayerLevelGenerator.ActionGameStart?.Invoke();

        }

        private void OnDisable()
        {
            TilesParent.SetActive(false);
        }
    }
}

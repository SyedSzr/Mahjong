using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Screens;
using UnityEngine.SceneManagement;
using Game.Popups;

namespace Game.Utilities
{
    public class ButtonBackToMenu : ButtonComponent
    {
        public override void OnButtonClicked()
        {
            base.OnButtonClicked();
            var Menu = DependencyManager.Instance.ScreenManager.GetScreen<MenuScreen>();
            Menu.Show();
            var Screen = DependencyManager.Instance.ScreenManager.GetScreen<GamePlayScreen>();
            Screen.Hide();
            MatchManager.Instance.ClearAllActiveTile();


        }
    }
}

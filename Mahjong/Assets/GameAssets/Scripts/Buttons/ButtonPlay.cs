using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Screens;

namespace Game.Utilities
{
    public class ButtonPlay : ButtonComponent
    {
        public override void OnButtonClicked()
        {
            base.OnButtonClicked();
            
            var Screen = DependencyManager.Instance.ScreenManager.GetScreen<GamePlayScreen>();
            Screen.Show();
            var Menu = DependencyManager.Instance.ScreenManager.GetScreen<MenuScreen>();
            Menu.Hide();
        }
    }
}

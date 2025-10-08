using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;


namespace Game.Utilities
{
    public class TextInAppPrice : TextComponent
    {
        public string ID;
        private void Start()
        {
           SetupText(DependencyManager.Instance.InAppManager.GetPrice(ID));
        }
    }
}
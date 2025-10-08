using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using Game.Settings;
using System;

namespace Game.Components
{
    public class CounterComponent : MonoBehaviour
    {
        public GameObject TextObject;
        public GameObject IconObject;
        public Color DefaultColor;
        public Color IconColor;

        public bool Shuffle;
        private Image IConBg;
        // Start is called before the first frame update
        void Start()
        {
            IConBg = GetComponent<Image>();
            DependencyManager.Instance.GameManager.ActionUpdateStateItems += UpdateIconStatus;
            UpdateIconStatus();
        }

        private void UpdateIconStatus()
        {
            var Count = 0;
            if (Shuffle)
            {
                 Count = DependencyManager.Instance.PlayerStateManager.PlayerState.Shuffle;

            }
            else
            {
                Count = DependencyManager.Instance.PlayerStateManager.PlayerState.Hint;

            }
            if (Count<=0)
                {
                    IConBg.color = IconColor;
                    IconObject.SetActive(true);
                }
                else
                {
                    IConBg.color = DefaultColor;
                    IconObject.SetActive(false);
                }
           
        }
    }
}
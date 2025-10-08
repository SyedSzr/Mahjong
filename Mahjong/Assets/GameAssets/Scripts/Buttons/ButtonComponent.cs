using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Utilities
{
    [RequireComponent(typeof(Button))]
    public class ButtonComponent : MonoBehaviour
    {
        public string AudioClipName;
        private Button mButton = null;
        public Button Button
        {
            get
            {
                if (mButton == null)
                {
                    mButton = GetComponent<Button>();
                }
                return mButton;
            }
        }

        public virtual void Start()
        {
            Button.onClick.AddListener(OnButtonClicked);
        }

        public virtual void OnButtonClicked()
        {
            AudioClipName = string.IsNullOrEmpty(AudioClipName) ? "Click" : AudioClipName;
            var Clip = DependencyManager.Instance.GameManager.GetClipByID(AudioClipName);
            Vibration.Vibrate(30);
            DependencyManager.Instance.SoundManager.PlaySFX(Clip);
        }

      
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;

namespace Game.Utilities
{
    public class ButtonToggleVibration:BaseButtonToggle
    {
        public override void OnKnobMoved()
        {
            base.OnKnobMoved();
            DependencyManager.Instance.SoundManager.ToggleVibration(Status); // Disable vibration

        }
    }
}
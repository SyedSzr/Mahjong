using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;

namespace Game.Utilities
{
    public class ButtonToggleSound : BaseButtonToggle
    {
        public override void OnKnobMoved()
        {
            base.OnKnobMoved();
            DependencyManager.Instance.SoundManager.ToggleSFX(Status);

        }
    }
}
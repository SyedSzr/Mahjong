using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using DG.Tweening;
using System;

namespace Game.Utilities
{
    public class BaseButtonToggle : ButtonComponent
    {
        public Sprite ImageBgOn;
        public Sprite ImageBgOff;
        public Image ToggleBg;
        public GameObject Knob;
        protected bool Status=true;

        public override void OnButtonClicked()
        {
            base.OnButtonClicked();
            Toggle();
        }

        public void Toggle()
        {
            Status = !Status;
            ToggleBg.sprite = Status ? ImageBgOn : ImageBgOff;
            Knob.transform.DOLocalMoveX(Status ? 40 : -40, .1f).OnComplete(()=> {
                OnKnobMoved();
            });
        }

        public virtual void OnKnobMoved()
        {
            
        }
    }
}
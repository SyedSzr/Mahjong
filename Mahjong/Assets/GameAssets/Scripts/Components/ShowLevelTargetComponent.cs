using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Settings;
using System;

namespace Game.Components
{
    public class ShowLevelTargetComponent : MonoBehaviour
    {
        public GameObject TargetItems;
        public Transform ContentItem;

        private void Start()
        {
            DependencyManager.Instance.MultilayerLevelGenerator.ActionGameStart += Setup;
            Setup();
        }

        void ResetData()
        {
            var AllAchild = GetComponentsInChildren<TargetItemComponent>();
            foreach (var item in AllAchild)
            {
                Destroy(item.gameObject);
            }
        }

        private void Setup()
        {
            ResetData();
            var Targets = DependencyManager.Instance.MultilayerLevelGenerator.SavedLevelTargets;
            foreach (var item in Targets)
            {
                var Obj = Instantiate(TargetItems, ContentItem);
                Obj.GetComponent<TargetItemComponent>().Setup(item.RequiredCount, item.TargetValue);
            }
        }
    }
}
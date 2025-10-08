using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Settings;
using Game.Components;
using Game.Utilities;
using System.Linq;

namespace Game.Popups
{
    public class PopupDailyReward : BasePopup
    {
        public GameObject RewardPack;
        public Transform ContentParent;
        public TextComponent TitleText;
        private List<RewardSetting> RewardIDs=new List<RewardSetting>();

        public void Setup(List<RewardSetting> Rewards,string Title)
        {
            TitleText.SetupText(Title);
            RewardIDs.Clear();
            RewardIDs = Rewards.ToList();
        }


        public override void Show(bool KeepLastPopupActive = false)
        {
            base.Show(KeepLastPopupActive);
            ResetData();

            foreach (var item in RewardIDs)
            {
                var Obj = Instantiate(RewardPack, ContentParent);
                Obj.GetComponent<RewardPackComponent>().Setup(item.IconID, item.Count.ToString());
                if (item.IconID == "Shuffle")
                    DependencyManager.Instance.PlayerStateManager.PlayerState.Shuffle += item.Count;
                if (item.IconID == "Hint")
                    DependencyManager.Instance.PlayerStateManager.PlayerState.Hint += item.Count;
            }


            DependencyManager.Instance.GameManager.ActionUpdateStateItems?.Invoke();

        }
        private void ResetData()
        {
            for (int i = 0; i < ContentParent.childCount; i++)
            {
                Destroy(ContentParent.GetChild(i).gameObject);
            }
        }
    }
}
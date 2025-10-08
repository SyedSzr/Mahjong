using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Settings;
using Game.Components;

namespace Game.Popups
{
    public class PopupLeaderboard : BasePopup
    {
        public List<GameObject> LeaderBoardItem;
        public Transform ContentParent;

        void ResetData()
        {
            for (int i = 0; i < ContentParent.childCount; i++)
            {
                Destroy(ContentParent.GetChild(i).gameObject);
            }
        }
        public void Setup()
        {
            DependencyManager.Instance.LeaderboardManager.LoadTodayLeaderboard("CgkIi4zHgLwdEAIQAA", 10, (leaderBoard) =>
            {
                ResetData();

                if (leaderBoard.Count <= 0)
                {
                    var obj = Instantiate(LeaderBoardItem[0], ContentParent);
                    var Score = DependencyManager.Instance.PlayerStateManager.PlayerState.Xp;
                    obj.GetComponent<LeaderBoardUserItemComponent>().Setup(1, "You", Score);
                    return;
                }

                // Clear old children first (avoid duplicates if popup reopens)
                foreach (Transform child in ContentParent)
                {
                    Destroy(child.gameObject);
                }

                for (int i = 0; i < leaderBoard.Count; i++)
                {
                    LeaderboardScoreSetting item = leaderBoard[i];
                    var obj = Instantiate(LeaderBoardItem[i % LeaderBoardItem.Count], ContentParent);
                    obj.GetComponent<LeaderBoardUserItemComponent>().Setup(item.ID, item.Name, item.Score);
                }
            });
        }


        public override void Show(bool KeepLastPopupActive = false)
        {
            base.Show(KeepLastPopupActive);
            Setup();
        }
    }
}
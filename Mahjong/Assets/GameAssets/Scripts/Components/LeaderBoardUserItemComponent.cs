using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Settings;
using Game.Utilities;

namespace Game.Components
{
    public class LeaderBoardUserItemComponent : MonoBehaviour
    {
        public TextComponent TextRank;
        public TextComponent TextName;
        public TextComponent TextScore;

        public void Setup(int Rank, string Name, long Score)
        {
            TextRank.SetupText(Rank.ToString());
            TextName.SetupText(Name);
            TextScore.SetupText("Score\n"+Score.ToString());
        }
    }
}
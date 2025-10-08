using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Utilities
{
    public class TextComponent : MonoBehaviour
    {
        private float delay = 0.05f;    // Time delay between letters

        private TMP_Text mText = null;
        private TMP_Text Text
        {
            get
            {
                if (mText == null)
                {
                    mText = GetComponent<TMP_Text>();
                }
                return mText;
            }
        }

        public void SetupText(string text)
        {
            Text.SetText(text);
        }

        public void SetupColor(Color color)
        {
            Text.color = color;
        }

        public string GetText()
        {
            return Text.text;
        }

        public TMP_Text Get_TMPText()
        {
            return Text;
        }

        public void ShowTextTypewriteEffect(string TextToShow,float delay=.05f)
        {
            this.delay = delay;
            StartCoroutine(ShowText(TextToShow));
        }

        public void ShowCounter(int StartAmount, int FinalAmount)
        {
            int startValue = StartAmount;
            int actualScore = FinalAmount;
            int displayedScore = 0;

            DOTween.To(() => startValue, x => {
                displayedScore = x;
                Text.text = displayedScore.ToString();
            }, actualScore, 0.5f); // 0.5 seconds duration
        }

        IEnumerator ShowText(string fullText)
        {
            Text.text = "";
            foreach (char c in fullText)
            {
                Text.text += c;
                yield return new WaitForSeconds(delay);
            }
        }

        public void SetSortingOrder(int sortIndex)
        {
            var tmpPro = Text as TextMeshPro;
            if (tmpPro != null)
            {
                var textRenderer = tmpPro.GetComponent<Renderer>();
                textRenderer.sortingOrder = sortIndex;
            }
        }
    }
}
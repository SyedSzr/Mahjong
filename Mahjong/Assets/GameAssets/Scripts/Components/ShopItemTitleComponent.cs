using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Settings;
using TMPro;
namespace Game.Components
{
    public class ShopItemTitleComponent : MonoBehaviour
    {
        public TMP_Text Title;
        public TMP_Text Quantity;

        public void Setup(string _title, int _quantity)
        {
            Title.text = _title;
            Quantity.text = _quantity.ToString();
        }
    }
}
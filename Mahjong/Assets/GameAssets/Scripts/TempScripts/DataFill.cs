using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Settings;
using Game.Scriptables;
public class DataFill : MonoBehaviour
{
    public List<Sprite> Icons;
    public IconData IconData;
    public TileData TileData;
    public QuotData QuotData;
    public string Quotations;

    [ContextMenu("Proceed")]
    public void Proceed()
    {
        //Update IconData
        //foreach (var item in Icons)
        //{
        //    IconSetting Icons = new IconSetting();
        //    Icons.ID = item.name;
        //    Icons.Icon = item;
        //    IconData.IconSettings.Add(Icons);
        //}

        //Update TileData
        //foreach (var item in IconData.IconSettings)
        //{
        //    TileSetting tileSetting = new TileSetting();
        //    tileSetting.ID = item.ID;
        //    var TempData = item.ID.Split('_');
        //    Debug.Log(TempData[0]+"---"+TempData[1]);
        //    tileSetting.NumberValue =System.Convert.ToInt32(TempData[1]);
        //    tileSetting.TypeOfTile = TempData[0];
        //    TileData.TileSettings.Add(tileSetting);
        //}

        var Quot = Quotations.Split(".");
        foreach (var item in Quot)
        {
            QuotData.QuotationSettings.Add(item.ToString());
        }

    }
}

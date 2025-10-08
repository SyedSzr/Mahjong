using System.Collections.Generic;
using UnityEngine;

public class MahjongMatchRules
{
    public static bool IsFree(TileComponent tile)
    {
        if (tile.IsBlockedByAbove()) return false;

        bool leftFree = tile.LeftNeighbor == null || !tile.LeftNeighbor.gameObject.activeInHierarchy;
        bool rightFree = tile.RightNeighbor == null || !tile.RightNeighbor.gameObject.activeInHierarchy;

        //Debug.Log("Left " + leftFree);
        //Debug.Log("Right " + rightFree);

        // Check if any tile overlaps (in 3D front view) this tile
        foreach (var other in MatchManager.Instance.GetActiveTiles())
        {
            if (other == tile) continue;

            Vector3 diff = other.transform.position - tile.transform.position;

            // Same X, Y plane but other is closer to the camera (lower z)
            if (Mathf.Abs(diff.x) < 2.5f &&
                Mathf.Abs(diff.y) < 2.3f &&
                diff.z < 0 &&
                Mathf.Abs(diff.z) > 0.01f)
            {
                //Debug.Log("Tile is partially hidden by another tile in front.");
                return false;
            }
        }

        return leftFree || rightFree;
    }



    public static bool IsMatch(TileComponent a, TileComponent b)
    {
        if (!a || !b || a.TileSetting == null || b.TileSetting == null)
            return false;

        if (!a.gameObject.activeInHierarchy || !b.gameObject.activeInHierarchy)
            return false;

        // Matching Pictorial Tiles (can match any with any)
        if (a.TileSetting.TypeOfTile == "Pic" && b.TileSetting.TypeOfTile == "Pic")
            return a.TileSetting.NumberValue == b.TileSetting.NumberValue;

        // Matching number tiles (must be same type and sum to 10)
        if (a.TileSetting.TypeOfTile == b.TileSetting.TypeOfTile)
            return a.TileSetting.NumberValue + b.TileSetting.NumberValue == 10;

        return false;
    }

    public static void HighlightAllMatches(List<TileComponent> allTiles)
    {
        foreach (var t in allTiles)
        {
            if (t != null) t.Highlight(false);
        }

        var matches = GetAllFreeMatches(allTiles);
        foreach (var pair in matches)
        {
            pair.Item1.Highlight(true);
            pair.Item2.Highlight(true);
        }
    }


    public static List<(TileComponent, TileComponent)> GetAllFreeMatches(List<TileComponent> allTiles)
    {
        List<TileComponent> freeTiles = new List<TileComponent>();

        foreach (var tile in allTiles)
        {
            if (tile != null && tile.gameObject.activeInHierarchy && IsFree(tile))
            {
                freeTiles.Add(tile);
            }
        }

        List<(TileComponent, TileComponent)> matches = new();
        for (int i = 0; i < freeTiles.Count; i++)
        {
            for (int j = i + 1; j < freeTiles.Count; j++)
            {
                if (IsMatch(freeTiles[i], freeTiles[j]))
                {
                    matches.Add((freeTiles[i], freeTiles[j]));
                }
            }
        }

        return matches;
    }
}

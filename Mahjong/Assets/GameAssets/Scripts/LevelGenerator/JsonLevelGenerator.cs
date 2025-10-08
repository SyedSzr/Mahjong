using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.Scriptables;
using Game.Settings;
using DG.Tweening;
using System.IO;

[System.Serializable]
public class JsonLevelData
{
    public List<Layer> layers;
    public List<int> gridSizes;
}

[System.Serializable]
public class Layer
{
    public List<Row> rows = new List<Row>();
}

[System.Serializable]
public class Row
{
    public List<int> tiles = new List<int>();
}

public class JsonLevelGenerator : MonoBehaviour
{
    public TextAsset jsonFile;
    public GameObject tilePrefab;
    public Transform tileParent;
    public TileData TileData;
    public float spacingX = 1.1f;
    public float spacingY = 1.1f;

    private List<TileComponent> allTiles = new();
    private Dictionary<int, List<TileComponent>> tilesByLayer = new();
    private int layerCount = 0;

    void Start()
    {
        GenerateLevelFromJson();
        AssignNeighbors();
        HighlightFrontLayer();
    }

    public void GenerateLevelFromJson()
    {
        ClearOldTiles();
        allTiles.Clear();
        tilesByLayer.Clear();
       JsonLevelData levelData = JsonUtility.FromJson<JsonLevelData>(jsonFile.text);
        layerCount = levelData.layers.Count;

        int currentLayer = 1;
        foreach (var layer in levelData.layers)
        {
            int height = layer.rows.Count;
            int width = layer.rows[0].tiles.Count;
            Vector2 offset = new Vector2((width - 1) * spacingX / 2f, (height - 1) * spacingY / 2f);
            List<TileSetting> matchablePool = GenerateMatchableTiles(CountTilesInLayer(layer));
            int tileIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int val = layer.rows[y].tiles[x];
                    if (val != 1) continue; // skip empty or special tiles

                    Vector3 pos = new Vector3(x * spacingX - offset.x, (height - 1 - y) * spacingY - offset.y, currentLayer);
                    GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity, tileParent);
                    tile.name = $"Tile_{x}_{y}_L{currentLayer}";

                    var comp = tile.GetComponent<TileComponent>();

                    GameObject shadow = new GameObject("Shadow");
                    shadow.transform.SetParent(tile.transform);
                    shadow.transform.localPosition = new Vector3(0.2f, -0.2f, 0.1f);
                    shadow.transform.localScale = Vector3.one * 1.1f;
                    var sr = tile.GetComponent<SpriteRenderer>();
                    var shadowRenderer = shadow.AddComponent<SpriteRenderer>();
                    shadowRenderer.sprite = sr.sprite;
                    shadowRenderer.color = new Color(0, 0, 0, 0.15f);
                    shadowRenderer.sortingOrder = sr.sortingOrder - 1;

                    if (comp != null)
                    {
                        if (tileIndex < matchablePool.Count)
                        {
                            comp.TileSetting = matchablePool[tileIndex++];
                        }
                        else
                        {
                            Debug.LogWarning($"Tile index {tileIndex} out of range. Matchable pool has only {matchablePool.Count} tiles.");
                            Destroy(tile);
                            continue;
                        }
                        comp.Layer = currentLayer;
                        comp.Setup();
                        MatchManager.Instance.RegisterTile(comp);
                        allTiles.Add(comp);

                        if (!tilesByLayer.ContainsKey(currentLayer))
                            tilesByLayer[currentLayer] = new List<TileComponent>();
                        tilesByLayer[currentLayer].Add(comp);
                    }
                }
            }
            currentLayer++;
        }
    }

    int CountTilesInLayer(Layer layer)
    {
        return layer.rows.Sum(row => row.tiles.Count(cell => cell == 1));
    }

    List<TileSetting> GenerateMatchableTiles(int count)
    {
        List<TileSetting> pool = new();
        var allTiles = TileData.TileSettings;
        List<TileSetting> pictorials = allTiles.Where(t => t.TypeOfTile == "Pic").ToList();
        List<TileSetting> numbers = allTiles.Where(t => t.TypeOfTile != "Pic").ToList();

        int picCount = count / 2;
        int numCount = count - picCount;
        picCount -= picCount % 2;
        numCount -= numCount % 2;

        for (int i = 0; i < picCount / 2; i++)
        {
            var pic = pictorials[Random.Range(0, pictorials.Count)];
            pool.Add(pic);
            pool.Add(pic);
        }

        Dictionary<string, List<TileSetting>> grouped = new();
        foreach (var tile in numbers)
        {
            if (!grouped.ContainsKey(tile.TypeOfTile))
                grouped[tile.TypeOfTile] = new List<TileSetting>();
            grouped[tile.TypeOfTile].Add(tile);
        }

        int generated = 0;
        while (generated < numCount)
        {
            bool pairAdded = false;

            foreach (var group in grouped)
            {
                var list = group.Value;
                var pairs = FindAllPairsThatSum10(list);
                if (pairs.Count == 0) continue;

                var pair = pairs[Random.Range(0, pairs.Count)];
                pool.Add(pair.Item1);
                pool.Add(pair.Item2);
                generated += 2;
                pairAdded = true;

                if (generated >= numCount) break;
            }

            if (!pairAdded) break;
        }

        return pool.OrderBy(x => Random.value).ToList();
    }

    List<(TileSetting, TileSetting)> FindAllPairsThatSum10(List<TileSetting> tiles)
    {
        var result = new List<(TileSetting, TileSetting)>();
        for (int i = 0; i < tiles.Count; i++)
        {
            for (int j = i + 1; j < tiles.Count; j++)
            {
                if (tiles[i].TypeOfTile == tiles[j].TypeOfTile && tiles[i].NumberValue + tiles[j].NumberValue == 10)
                {
                    result.Add((tiles[i], tiles[j]));
                }
            }
        }
        return result;
    }

    void AssignNeighbors()
    {
        foreach (var tile in MatchManager.Instance.activeTiles)
        {
            tile.LeftNeighbor = null;
            tile.RightNeighbor = null;
            tile.AboveNeighbors.Clear();

            foreach (var other in MatchManager.Instance.activeTiles)
            {
                if (other == tile) continue;
                Vector3 diff = other.transform.position - tile.transform.position;
                if (Mathf.Abs(diff.y) < 0.1f && Mathf.Abs(diff.z) < 0.1f)
                {
                    if (Mathf.Abs(diff.x - spacingX) < 0.1f) tile.RightNeighbor = other;
                    if (Mathf.Abs(diff.x + spacingX) < 0.1f) tile.LeftNeighbor = other;
                }
                bool isAbove = Mathf.Abs(diff.x) < 0.5f && Mathf.Abs(diff.y) < 0.5f && diff.z > 0.05f;
                if (isAbove) tile.AboveNeighbors.Add(other);
            }
        }
    }

    void HighlightFrontLayer()
    {
        int frontLayer = tilesByLayer.Keys.Min();
        foreach (var kvp in tilesByLayer)
        {
            bool isFront = kvp.Key == frontLayer;
            foreach (var tile in kvp.Value)
            {
                var sr = tile.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color c = sr.color;
                    sr.color = isFront ? Color.white : new Color(c.r * 0.7f, c.g * 0.7f, c.b * 0.7f, c.a);
                }
            }
        }
    }

    void ClearOldTiles()
    {
        foreach (Transform child in tileParent)
        {
            Destroy(child.gameObject);
        }
    }

    public List<TileComponent> GetAllTiles() => allTiles;
}

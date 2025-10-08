using System.Collections.Generic;
using UnityEngine;
using Game.Settings;
using Game.Scriptables;
using System.Linq;
using DG.Tweening;

public class LevelGenerator : MonoBehaviour
{
    [Header("Tile Settings")]
    public TileData TileData;
    public GameObject tilePrefab;
    public Transform tileParent;

    [Range(1, 5)]
    public int difficulty = 1;

    public Vector2Int boardSize = new Vector2Int(10, 6);
    public float spacing = 1.1f;

    private List<TileSetting> tilePool = new List<TileSetting>();

    void Start()
    {
        GenerateLevel(difficulty);
        CenterCamera();
    }

    public void GenerateLevel(int difficultyLevel)
    {
        tilePool.Clear();
        ClearOldTiles();

        List<TileSetting> allTiles = TileData.TileSettings;
        List<TileComponent> grid = new(); // Track all tiles

        int totalTiles = Mathf.Clamp(30 + difficultyLevel * 10, 40, 80);
        if (totalTiles % 2 != 0) totalTiles++;

        int pictorialCount = totalTiles / (6 - difficultyLevel);
        pictorialCount = pictorialCount % 2 == 0 ? pictorialCount : pictorialCount + 1;

        int numberedCount = totalTiles - pictorialCount;
        numberedCount = numberedCount % 2 == 0 ? numberedCount : numberedCount - 1;

        CreateNumberedTilePairs(numberedCount, allTiles);
        CreatePictorialTilePairs(pictorialCount, allTiles);

        Shuffle(tilePool);

        int index = 0;
        Vector2 offset = new Vector2((boardSize.x - 1) * spacing / 2f, (boardSize.y - 1) * spacing / 2f);

        for (int y = 0; y < boardSize.y && index < tilePool.Count; y++)
        {
            for (int x = 0; x < boardSize.x && index < tilePool.Count; x++)
            {
                Vector3 pos = new Vector3(x * spacing - offset.x, y * spacing - offset.y, 0);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity, tileParent);
                tile.name = tilePool[index].ID;

                TileComponent comp = tile.GetComponent<TileComponent>();
                if (comp != null)
                {
                    comp.TileSetting = tilePool[index];
                    comp.Setup();
                    grid.Add(comp);
                }
                index++;
            }
        }

        // Assign neighbors for Mahjong logic
        for (int i = 0; i < grid.Count; i++)
        {
            TileComponent current = grid[i];
            if (current == null) continue;

            int x = i % boardSize.x;

            if (x > 0)
                current.LeftNeighbor = grid[i - 1];

            if (x < boardSize.x - 1 && i + 1 < grid.Count)
                current.RightNeighbor = grid[i + 1];

            // Check tiles stacked above
            for (int j = 0; j < grid.Count; j++)
            {
                if (j == i) continue;
                var other = grid[j];
                if (other != null &&
                    Mathf.Abs(other.transform.position.x - current.transform.position.x) < 0.5f &&
                    Mathf.Abs(other.transform.position.y - current.transform.position.y) < 0.5f &&
                    other.transform.position.z > current.transform.position.z)
                {
                    current.AboveNeighbors.Add(other);
                }
            }
        }
    }

    void CreateNumberedTilePairs(int count, List<TileSetting> allTiles)
    {
        Dictionary<string, List<TileSetting>> grouped = new();

        foreach (var tile in allTiles)
        {
            if (tile.TypeOfTile == "Pic") continue;
            if (!grouped.ContainsKey(tile.TypeOfTile))
                grouped[tile.TypeOfTile] = new List<TileSetting>();
            grouped[tile.TypeOfTile].Add(tile);
        }

        int generated = 0;
        while (generated < count)
        {
            bool pairAdded = false;

            foreach (var group in grouped)
            {
                var list = group.Value;
                var pairs = FindAllPairsThatSum10(list);

                if (pairs.Count == 0) continue;

                var pair = pairs[Random.Range(0, pairs.Count)];
                tilePool.Add(pair.Item1);
                tilePool.Add(pair.Item2);
                generated += 2;
                pairAdded = true;

                if (generated >= count) break;
            }

            if (!pairAdded) break; // Stop if no more pairs are possible
        }
    }

    List<(TileSetting, TileSetting)> FindAllPairsThatSum10(List<TileSetting> tiles)
    {
        var result = new List<(TileSetting, TileSetting)>();

        for (int i = 0; i < tiles.Count; i++)
        {
            for (int j = i + 1; j < tiles.Count; j++)
            {
                if (tiles[i].NumberValue + tiles[j].NumberValue == 10 && tiles[i].TypeOfTile == tiles[j].TypeOfTile)
                {
                    result.Add((tiles[i], tiles[j]));
                }
            }
        }

        return result;
    }

    void CreatePictorialTilePairs(int count, List<TileSetting> allTiles)
    {
        var pictorials = allTiles.FindAll(t => t.TypeOfTile == "Pic");

        int pairCount = count / 2;
        for (int i = 0; i < pairCount; i++)
        {
            var pic = pictorials[Random.Range(0, pictorials.Count)];
            tilePool.Add(pic);
            tilePool.Add(pic);
        }
    }

    void Shuffle(List<TileSetting> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    void CenterCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float width = (boardSize.x - 1) * spacing;
            float height = (boardSize.y - 1) * spacing;
            Vector3 centerPosition = new Vector3(0, 0, -10); // Tiles are centered at origin
            mainCamera.transform.position = centerPosition;

            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = width / height;

            if (screenRatio >= targetRatio)
            {
                mainCamera.orthographicSize = height / 2f + spacing;
            }
            else
            {
                float differenceInSize = targetRatio / screenRatio;
                mainCamera.orthographicSize = (height / 2f + spacing) * differenceInSize;
            }
        }
    }

    [ContextMenu("Shuffle")]
    public void ShuffleTilesInScene(System.Action onComplete = null)
    {
        List<Transform> tiles = new List<Transform>();
        foreach (Transform child in tileParent)
        {
            tiles.Add(child);
        }

        // Store original positions in grid order
        List<Vector3> gridPositions = new List<Vector3>();
        Vector2 offset = new Vector2((boardSize.x - 1) * spacing / 2f, (boardSize.y - 1) * spacing / 2f);
        for (int y = 0; y < boardSize.y; y++)
        {
            for (int x = 0; x < boardSize.x; x++)
            {
                Vector3 pos = new Vector3(x * spacing - offset.x, y * spacing - offset.y, 0);
                gridPositions.Add(pos);
            }
        }

        // Move all tiles to center first
        Vector3 center = Vector3.zero;
        foreach (var tile in tiles)
        {
            tile.DOMove(center, 0.3f).SetEase(Ease.InOutQuad);
        }

        // Delay shuffle movement until all have gathered in center
        DOVirtual.DelayedCall(0.35f, () =>
        {
                // Disable all tile interactions during shuffle
                foreach (var tile in tiles)
            {
                var comp = tile.GetComponent<TileComponent>();
                    //if (comp != null) comp.SetInteractable(false);
                }

                // Play shuffle sound or animation
                // Example: AudioManager.Instance.Play("Shuffle");
                foreach (var tile in tiles)
            {
                tile.DOScale(0.8f, 0.2f).SetLoops(2, LoopType.Yoyo);
            }
            var shuffledTiles = tiles.OrderBy(t => Random.value).ToList();

            int completed = 0;
            for (int i = 0; i < shuffledTiles.Count && i < gridPositions.Count; i++)
            {
                shuffledTiles[i].DOMove(gridPositions[i], 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    completed++;
                    if (completed >= shuffledTiles.Count)
                    {
                        foreach (var tile in shuffledTiles)
                        {
                            var comp = tile.GetComponent<TileComponent>();
                                //if (comp != null) comp.SetInteractable(true);
                            }
                        onComplete?.Invoke();
                    }
                });
            }
        });
    }




    void ClearOldTiles()
    {
        foreach (Transform child in tileParent)
        {
            Destroy(child.gameObject);
        }
    }
}

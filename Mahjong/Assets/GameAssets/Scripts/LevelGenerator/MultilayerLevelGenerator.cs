using System.Collections.Generic;
using UnityEngine;
using Game.Scriptables;
using System.Linq;
using DG.Tweening;
using Game.Settings;
using Game.Managers;
using System;
using Random = UnityEngine.Random;
using System.Collections;
using Game.Popups;
using Firebase.Analytics;

[System.Serializable]
public class TileLayerConfig
{
    public int width;
    public int height;
    public float zOffset;
}
[System.Serializable]
public class SavedTileData
{
    public string TileID;
    public string TypeOfTile;
    public int NumberValue;
    public Vector3 Position;
    public int Layer;
    public float ZOffset;
}

[Serializable]
public class SavedLevelTarget
{
    public string TargetType;   // "SumTo10" or "PicMatch"
    public string TargetValue;  // For PicMatch: "Pic_4", for SumTo10: "10"
    public int RequiredCount;   // number of pairs or tiles required
}

[Serializable]
public class SavedLevelData
{
    public List<SavedTileData> Tiles = new();
    public float ComplexityScore;
    public List<SavedLevelTarget> Targets = new(); // ✅ new field
}

[System.Serializable]
public class SavedLevelsContainer
{
    public List<SavedLevelData> Levels = new();
}


public class MultilayerLevelGenerator : MonoBehaviour
{
    public Action<List<TileComponent>> OnTileRemoved;
    public Action ActionGameStart;

    [Header("Tile Settings")]
    public TileData TileData;
    public GameObject tilePrefab;
    public Transform tileParent;
    public float SpecingX;
    public float SpecingY;
    public Action ActionUnselectAllTile;
    public List<SavedLevelTarget> SavedLevelTargets;
    public int shuffleNoMatchCount = 0;
    [SerializeField] private int maxNoMatchShuffles = 2; // after 2 no-match shuffles, auto clear

    public List<LevelSetting> LevelSettings;
    private List<TileComponent> allTiles = new();
    int count;
    public float CameraOffset;
    public Color ShadowColor;
    void Start()
    {
        ActionGameStart+= StartLevel;
        CenterCamera();
    }


    [ContextMenu("StartLevel")]
    public void StartLevel()
    {
        //GenerateMultilayerLevel();
        var Level = DependencyManager.Instance.PlayerStateManager.PlayerState.Level;
        LoadLevelFromFile(Level+1);
        DependencyManager.Instance.MatchManager.Score = 0;
        DependencyManager.Instance.GameManager.Score = 0;
        DependencyManager.Instance.MatchManager.ActionUpdateScore?.Invoke(0);

        DependencyManager.Instance.MatchManager.isLevelCompleted = false;
        TinySauce.OnGameStarted(Level+1);
        FirebaseAnalytics.LogEvent("level_start", "level_index", Level+1);

        //CenterCamera();

    }

    public void LoadLevelFromFile(int levelNumber)
    {
        Debug.Log("levelNumber " + levelNumber);

        string fileName = $"Level_{levelNumber:D3}"; // No .json extension for Resources.Load
        TextAsset levelFile = Resources.Load<TextAsset>("Levels/" + fileName);

        if (levelFile == null)
        {
            Debug.LogWarning($"❌ Level file not found in Resources: {fileName}");
            return;
        }

        string json = levelFile.text;
        var levelData = JsonUtility.FromJson<SavedLevelData>(json);
        LoadLevelFromSavedData(levelData);
        ValidateAndRemoveUnmatchableTiles();
    }


    //public void LoadLevelFromSavedData(SavedLevelData levelData)
    //{
    //    if (levelData == null || levelData.Tiles == null || levelData.Tiles.Count == 0)
    //    {
    //        Debug.LogWarning("⚠️ Invalid level data.");
    //        return;
    //    }

    //    ClearOldTiles();
    //    allTiles.Clear();
    //    //MatchManager.Instance.ClearAllTiles();
    //    SavedLevelTargets = levelData.Targets;
    //    foreach (var saved in levelData.Tiles)
    //    {
    //        Vector3 fixedPos = saved.Position;
    //        int row = Mathf.RoundToInt(saved.Position.y);
    //        //fixedPos.z = (-row);

    //        //GameObject tile = Instantiate(tilePrefab, fixedPos, Quaternion.identity, tileParent);
    //        //tile.name = $"Tile_L{saved.Layer}_R{row}_{saved.TileID}";

    //        // Position and instantiate
    //        GameObject tile = Instantiate(tilePrefab, saved.Position, Quaternion.identity, tileParent);
    //        tile.name = $"Tile_Layer{saved.Layer}_Row_{row}_ID_{saved.TileID}";
    //        // Setup TileComponent
    //        TileComponent comp = tile.GetComponent<TileComponent>();
    //        if (comp != null)
    //        {
    //            // Find TileSetting from current TileData
    //            var tileSetting = TileData.TileSettings.FirstOrDefault(t => t.ID == saved.TileID);
    //            if (tileSetting == null)
    //            {
    //                Debug.LogWarning($"⚠️ TileSetting with ID '{saved.TileID}' not found!");
    //                continue;
    //            }

    //            comp.TileSetting = tileSetting;
    //            comp.Layer = saved.Layer;
    //            comp.Setup();

    //            // Register with match manager
    //            MatchManager.Instance.RegisterTile(comp);
    //            allTiles.Add(comp);

    //            //Add Shadow
    //            GameObject shadow = new GameObject("Shadow");
    //            shadow.transform.SetParent(tile.transform);
    //            shadow.transform.localPosition = new Vector3(0f, -0f, 0.1f);
    //            shadow.transform.localScale = Vector3.one * 1.1f;
    //            var sr = tile.GetComponent<SpriteRenderer>();

    //            //Color layerColor = Color.HSVToRGB((saved.Layer * 0.15f) % 1f, 0.3f, 1f);
    //            //sr.color = layerColor;


    //            var shadowRenderer = shadow.AddComponent<SpriteRenderer>();
    //            shadowRenderer.sprite = sr.sprite;
    //            shadowRenderer.color = ShadowColor;
    //            shadowRenderer.sortingOrder = sr.sortingOrder;
    //        }
    //    }

    //    AssignNeighbors();
    //    CenterCamera();
    //}

    public void LoadLevelFromSavedData(SavedLevelData levelData)
    {
        if (levelData == null || levelData.Tiles == null || levelData.Tiles.Count == 0)
        {
            Debug.LogWarning("⚠️ Invalid level data.");
            return;
        }

        ClearOldTiles();
        allTiles.Clear();
        SavedLevelTargets = levelData.Targets;

        foreach (var saved in levelData.Tiles)
        {
            int row = Mathf.RoundToInt(saved.Position.y);

            // --- Position with depth ---
            Vector3 fixedPos = saved.Position;
            float zDepth = -(saved.Layer * 0.5f) - (row * 0.01f);
            fixedPos.z = zDepth;

            // --- Instantiate tile ---
            GameObject tile = Instantiate(tilePrefab, fixedPos, Quaternion.identity, tileParent);
            tile.name = $"Tile_Layer{saved.Layer}_Row{row}_ID_{saved.TileID}";

            TileComponent comp = tile.GetComponent<TileComponent>();
            if (comp != null)
            {
                var tileSetting = TileData.TileSettings.FirstOrDefault(t => t.ID == saved.TileID);
                if (tileSetting == null)
                {
                    Debug.LogWarning($"⚠️ TileSetting with ID '{saved.TileID}' not found!");
                    continue;
                }

                comp.TileSetting = tileSetting;
                comp.Layer = saved.Layer;
                comp.Setup();

                MatchManager.Instance.RegisterTile(comp);
                allTiles.Add(comp);

                // --- Sorting Order Fix ---
                var sr = tile.GetComponent<SpriteRenderer>();

                // Layer * 1000 ensures layer dominance.
                // Row is NEGATIVE so that lower rows draw above higher rows.
                sr.sortingOrder = saved.Layer * 1000 - row;

                // --- Shadow Setup ---
                GameObject shadow = new GameObject("Shadow");
                shadow.transform.SetParent(tile.transform, false);
                shadow.transform.localPosition = new Vector3(0.08f, -0.08f, 0.01f);
                shadow.transform.localScale = new Vector3(1.2f,1,1.2f);

                var shadowRenderer = shadow.AddComponent<SpriteRenderer>();
                shadowRenderer.sprite = sr.sprite;
                shadowRenderer.color = ShadowColor;
                shadowRenderer.sortingOrder = sr.sortingOrder - 1; // Always behind main tile
            }
        }

        AssignNeighbors();
        CenterCamera();
    }
   


    void CenterCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        var tiles = GetAllTiles();
        if (tiles == null || tiles.Count == 0) return;

        // 🔍 1. Calculate bounding box of all tile positions
        Vector3 min = tiles[0].transform.position;
        Vector3 max = tiles[0].transform.position;

        foreach (var tile in tiles)
        {
            Vector3 pos = tile.transform.position;
            min = Vector3.Min(min, pos);
            max = Vector3.Max(max, pos);
        }

        // 🎯 2. Calculate center and force Z = -10
        Vector3 targetPos = (min + max) / 2f;
        targetPos.z = -10f;

        // 📏 3. Calculate visible height needed (Y space + spacing)
        float tileAreaHeight = Mathf.Abs(max.y - min.y) + SpecingY;

        // ✨ 4. Scale height to occupy only 60% of screen height (leave 20% on top & bottom)
        float targetSize = tileAreaHeight / 0.6f / 2f;
        targetSize = targetSize < 22 ? 22 : targetSize;
        // 🎥 5. Animate camera position and zoom
        mainCamera.transform.DOMove(targetPos, 0.5f).SetEase(Ease.InOutQuad);
        DOTween.To(() => mainCamera.orthographicSize, x => mainCamera.orthographicSize = x, targetSize, 0.5f)
               .SetEase(Ease.InOutQuad);
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
                    if (Mathf.Abs(diff.x - SpecingX) < 0.1f) tile.RightNeighbor = other;
                    if (Mathf.Abs(diff.x + SpecingX) < 0.1f) tile.LeftNeighbor = other;
                }

                bool isAbove = Mathf.Abs(other.transform.position.x - tile.transform.position.x) < 0.01f &&
                                Mathf.Abs(other.transform.position.y - tile.transform.position.y) < 0.01f &&
                                other.transform.position.z > tile.transform.position.z;
                if (isAbove)
                {
                    //tile.AboveNeighbors.Add(other);
                   
                }
                
            }
        }
    }

    private void SetTilesInteractable(bool state)
    {
        foreach (var t in MatchManager.Instance.GetActiveTiles())
        {
            // If you have a method on TileComponent: t.SetInteractable(state);
            // else fallback:
            var col = t.GetComponent<Collider2D>();
            if (col != null) col.enabled = state;

            // also optionally disable raycast-target on sprite or other UI flags
        }
    }

    public void BringTileToTop(TileComponent tile)
    {
        if (tile == null) return;

        // Find the highest sorting order among all active tiles
        var allTiles = MatchManager.Instance.GetActiveTiles();
        int maxSorting = allTiles
            .Select(t => t.GetComponent<SpriteRenderer>().sortingOrder)
            .DefaultIfEmpty(0)
            .Max();

        // Increase by 1 to ensure it's on top
        int newSortingOrder = maxSorting + 1;

        // Apply to main tile
        var sr = tile.GetComponent<SpriteRenderer>();
        sr.sortingOrder = newSortingOrder;

        // Apply to shadow (keep relative order, e.g., -1)
        var shadowSR = tile.GetComponentsInChildren<SpriteRenderer>(true)
            .FirstOrDefault(s => s.gameObject.name == "Shadow");
        shadowSR.gameObject.SetActive(false);


    }

    int attempt = 0;

    /// <summary>
    /// Shuffle tiles up to maxShuffleAttempts times until at least `minMatchesRequired` matches exist.
    /// If no matches found after attempts, auto-match all tiles.
    /// </summary>
    //public void ShuffleTilesInSceneWithAutoMatch(System.Action onComplete = null, int minMatchesRequired = 2, int maxShuffleAttempts = 5)
    //{
    //    var components = MatchManager.Instance.GetActiveTiles();
    //    if (components == null || components.Count == 0)
    //    {
    //        onComplete?.Invoke();
    //        return;
    //    }

    //    int attempt = 0;
    //    bool success = false;

    //    while (attempt < maxShuffleAttempts && !success)
    //    {
    //        attempt++;

    //        // Save original data
    //        List<Vector3> positions = components.Select(t => t.DefaultPosition).ToList();
    //        List<int> sortingOrders = new List<int>();
    //        List<int> shadowSortingOrders = new List<int>();

    //        foreach (var c in components)
    //        {
    //            var sr = c.GetComponent<SpriteRenderer>();
    //            sortingOrders.Add(sr.sortingOrder);

    //            var shadowSR = c.GetComponentInChildren<SpriteRenderer>(true);
    //            if (shadowSR != null && shadowSR.gameObject.name == "Shadow")
    //                shadowSortingOrders.Add(shadowSR.sortingOrder);
    //            else
    //                shadowSortingOrders.Add(sr.sortingOrder - 1);
    //        }

    //        // Shuffle indices
    //        List<int> indices = Enumerable.Range(0, components.Count).ToList();
    //        for (int i = indices.Count - 1; i > 0; i--)
    //        {
    //            int j = Random.Range(0, i + 1);
    //            (indices[i], indices[j]) = (indices[j], indices[i]);
    //        }

    //        // Apply shuffled values
    //        for (int i = 0; i < components.Count; i++)
    //        {
    //            int newIndex = indices[i];

    //            // Position
    //            components[i].transform.position = positions[newIndex];
    //            components[i].DefaultPosition = positions[newIndex];

    //            // Tile sorting
    //            var sr = components[i].GetComponent<SpriteRenderer>();
    //            sr.sortingOrder = sortingOrders[newIndex];

    //            // Shadow sorting
    //            var shadowSR = components[i].GetComponentsInChildren<SpriteRenderer>(true)
    //                .FirstOrDefault(s => s.gameObject.name == "Shadow");

    //            if (shadowSR != null)
    //                shadowSR.sortingOrder = shadowSortingOrders[newIndex];
    //        }

    //        // Update neighbors
    //        AssignNeighbors();

    //        // Check if enough matches available
    //        var matches = MahjongMatchRules.GetAllFreeMatches(MatchManager.Instance.GetActiveTiles());
    //        if (matches.Count >= minMatchesRequired)
    //        {
    //            success = true;
    //        }
    //    }

    //    if (!success)
    //    {
    //        Debug.LogWarning("No valid shuffle with matches found after " + maxShuffleAttempts + " attempts. Auto-matching...");
    //        AutoMatchAllTiles();
    //    }

    //    onComplete?.Invoke();
    //}
    public void ShuffleTilesInSceneWithAutoMatch(System.Action onComplete = null, int minMatchesRequired = 2, int maxShuffleAttempts = 5, float shuffleDuration = 0.5f)
    {
        DependencyManager.Instance.MultilayerLevelGenerator.ActionUnselectAllTile?.Invoke();
        var components = MatchManager.Instance.GetActiveTiles();
        if (components == null || components.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        bool success = false;

        while (attempt < maxShuffleAttempts && !success)
        {
            attempt++;

            // Save original data
            List<Vector3> positions = components.Select(t => t.DefaultPosition).ToList();
            List<int> sortingOrders = new List<int>();
            List<int> shadowSortingOrders = new List<int>();

            foreach (var c in components)
            {
                var sr = c.GetComponent<SpriteRenderer>();
                sortingOrders.Add(sr.sortingOrder);

                var shadowSR = c.GetComponentInChildren<SpriteRenderer>(true);
                if (shadowSR != null && shadowSR.gameObject.name == "Shadow")
                    shadowSortingOrders.Add(shadowSR.sortingOrder);
                else
                    shadowSortingOrders.Add(sr.sortingOrder - 1);
            }

            // Shuffle indices
            List<int> indices = Enumerable.Range(0, components.Count).ToList();
            for (int i = indices.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (indices[i], indices[j]) = (indices[j], indices[i]);
            }

            // Animate shuffle using DOTween
            Sequence shuffleSequence = DOTween.Sequence();

            for (int i = 0; i < components.Count; i++)
            {
                int newIndex = indices[i];

                // Animate position
                shuffleSequence.Join(
                    components[i].transform.DOMove(positions[newIndex], shuffleDuration).SetEase(Ease.InOutQuad)
                );

                // Save updated default position after tween
                components[i].DefaultPosition = positions[newIndex];
                // Sorting orders (apply immediately, no tween needed)
                var sr = components[i].GetComponent<SpriteRenderer>();
                sr.sortingOrder = sortingOrders[newIndex];

                var shadowSR = components[i].GetComponentsInChildren<SpriteRenderer>(true)
                    .FirstOrDefault(s => s.gameObject.name == "Shadow");
                if (shadowSR != null)
                    shadowSR.sortingOrder = shadowSortingOrders[newIndex];
            }

            shuffleSequence.OnComplete(() =>
            {
                // Update neighbors after tween finishes
                AssignNeighbors();

                // Check if enough matches available
                var matches = MahjongMatchRules.GetAllFreeMatches(MatchManager.Instance.GetActiveTiles());
                if (matches.Count >= minMatchesRequired)
                {
                    success = true;
                    attempt = 0;
                    onComplete?.Invoke();
                }
                else
                {
                    if (attempt >= maxShuffleAttempts)
                    {
                        Debug.LogWarning("No valid shuffle with matches found after " + maxShuffleAttempts + " attempts. Auto-matching...");
                        AutoMatchAllTiles();
                        attempt = 0;
                        onComplete?.Invoke();
                    }
                    else
                    {
                        // Try again
                        ShuffleTilesInSceneWithAutoMatch(onComplete, minMatchesRequired, maxShuffleAttempts, shuffleDuration);
                    }
                }
            });

            // Run only the first shuffle attempt animation, later ones retry internally
            break;
        }
    }

    /// <summary>
    /// Removes any tiles that do not have a valid match in the scene.
    /// Ensures no unmatchable leftovers exist at level start.
    /// </summary>
    void ValidateAndRemoveUnmatchableTiles()
    {
        var tiles = MatchManager.Instance.GetActiveTiles().ToList();
        HashSet<TileComponent> toRemove = new HashSet<TileComponent>();

        foreach (var tile in tiles)
        {
            // Skip if already marked
            if (toRemove.Contains(tile)) continue;

            // Check if this tile has a match available
            var match = tiles.FirstOrDefault(t => t != tile &&
                MahjongMatchRules.IsMatch(tile, t));

            if (match == null)
            {
                // ❌ No match found, mark tile for removal
                toRemove.Add(tile);
            }
        }

        // Actually remove the bad tiles
        foreach (var t in toRemove)
        {
            MatchManager.Instance.RemoveTile(t);
            Debug.Log($"Removed unmatchable tile: {t.name}");
        }
    }


    [ContextMenu("AutoMatchAllTiles")]
    public void AutoMatchAllTiles()
    {
        var tiles = MatchManager.Instance.GetActiveTiles();
        if (tiles.Count < 2) return;
        foreach (var item in tiles)
        {
            item.SetInteractable(false);

        }
        StartCoroutine(AutoMatchSequenceFast(tiles));
    }

    private IEnumerator AutoMatchSequenceFast(List<TileComponent> tiles)
    {
        var candidates = new List<TileComponent>(tiles);

        while (candidates.Count > 1)
        {
            TileComponent t1 = null, t2 = null;

            // Find first match
            for (int i = 0; i < candidates.Count; i++)
            {
                for (int j = i + 1; j < candidates.Count; j++)
                {
                    if (MahjongMatchRules.IsMatch(candidates[i], candidates[j]))
                    {
                        t1 = candidates[i];
                        t2 = candidates[j];
                        break;
                    }
                }
                if (t1 != null) break;
            }

            if (t1 == null || t2 == null) break;

            Vector3 mergePoint = (t1.transform.position + t2.transform.position) / 2f;

            // animate quickly
            t1.transform.DOMove(mergePoint, 0.15f).SetEase(Ease.Linear);
            t2.transform.DOMove(mergePoint, 0.15f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Vibration.Vibrate(5);
                    DependencyManager.Instance.MatchManager.PlaSfx("Match");

                    t1.Hide();
                    t2.Hide();
                    MatchManager.Instance.UnregisterTile(t1);
                    MatchManager.Instance.UnregisterTile(t2);
                });

            candidates.Remove(t1);
            candidates.Remove(t2);

            // small overlap before next pair starts (don’t wait fully)
            yield return new WaitForSeconds(0.05f);
        }

        if (MatchManager.Instance.GetActiveTiles().Count == 0)
            DependencyManager.Instance.MatchManager.LevelCompleted();
    }


    public void ShuffleTilesInScene()
    {

        ShuffleTilesInSceneWithAutoMatch();
        DependencyManager.Instance.GameManager.ActionTileStatus?.Invoke();
    }

    void ClearOldTiles()
    {
        foreach (Transform child in tileParent)
        {
            Destroy(child.gameObject);
        }
    }

    public List<TileComponent> GetAllTiles()
    {
        return allTiles;
    }


}
// You can copy it from your local file and drop it below this comment.

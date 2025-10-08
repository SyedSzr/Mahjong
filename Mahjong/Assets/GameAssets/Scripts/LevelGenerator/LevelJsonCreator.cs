using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Scriptables;
using Game.Settings;
using UnityEngine;

public class LevelJsonCreator : MonoBehaviour
{
    public TileData TileData; // Reference to your tile settings
    public float SpecingX = 1f;
    public float SpecingY = 1f;
    public float noiseScale = 0.25f;
    public float noiseThreshold = 0.5f;

    //[Serializable]
    //public class SavedLevelTarget
    //{
    //    public string TargetType;   // "SumTo10" or "PicMatch"
    //    public string TargetValue;  // For PicMatch: tile ID, for SumTo10: "10"
    //    public int RequiredCount;
    //}

    //[Serializable]
    //public class SavedTileData
    //{
    //    public string TileID;
    //    public string TypeOfTile;
    //    public int NumberValue;
    //    public Vector3 Position;
    //    public int Layer;
    //    public float ZOffset;
    //}

    //[Serializable]
    //public class SavedLevelData
    //{
    //    public List<SavedTileData> Tiles = new();
    //    public float ComplexityScore;
    //    public List<SavedLevelTarget> Targets = new();
    //}

    //[Serializable]
    //public class TileLayerConfig
    //{
    //    public int width;
    //    public int height;
    //    public float zOffset;
    //}

    [ContextMenu("GenerateLevel")]
    public void GenerateAllLevels()
    {
        string folderPath = Path.Combine(Application.dataPath, "Levels");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        for (int levelIndex = 1; levelIndex <= 20; levelIndex++)
        {
            var levelData = GenerateSingleLevelForSave(levelIndex);
            string json = JsonUtility.ToJson(levelData, true);
            File.WriteAllText(Path.Combine(folderPath, $"Level_{levelIndex:D3}.json"), json);
        }

        Debug.Log("✅ All levels generated and saved in: " + folderPath);
    }
    public SavedLevelData GenerateSingleLevelForSave(int levelIndex)
    {
        // -------- Step 1: Decide number of layers & grid sizes ----------
        List<TileLayerConfig> layers = new();
        int totalLayers;

        if (levelIndex==1)
        {
            totalLayers = 1;
            layers.Add(new TileLayerConfig { width = 2, height = 2, zOffset = 0 });
        }
        else if (levelIndex <= 5)
        {
            totalLayers = 1;
            int size = Mathf.Min(2 + (levelIndex - 1), 5);
            layers.Add(new TileLayerConfig { width = size, height = Mathf.Min(size + 1, 9), zOffset = 0 });
        }
        else if (levelIndex <= 20)
        {
            totalLayers = Mathf.Min(2 + (levelIndex / 5), 5);
            for (int i = 0; i < totalLayers; i++)
            {
                int w = UnityEngine.Random.Range(3, 7);
                var _Height = UnityEngine.Random.Range(w + 1, w + 5);
                int h = Mathf.Min(_Height, 9);
                layers.Add(new TileLayerConfig { width = w, height = h, zOffset = i * 0.3f });
            }
        }
        else
        {
            totalLayers = UnityEngine.Random.Range(4, 12);
            for (int i = 0; i < totalLayers; i++)
            {
                int w = UnityEngine.Random.Range(3, 8);
                var _Height = UnityEngine.Random.Range(w + 1, w + 5);
                int h = Mathf.Min(_Height, 9);
                layers.Add(new TileLayerConfig { width = w, height = h, zOffset = i * 0.3f });
            }
        }

        // -------- Step 2: Shape generation with/without noise ----------
        List<(Vector3 pos, int layer, float z)> allPositions = new();
        float noiseSeed = UnityEngine.Random.Range(0f, 100f);
        float effectiveNoiseThreshold = noiseThreshold;

        if (levelIndex > 5)
        {
            effectiveNoiseThreshold = Mathf.Lerp(0.55f, 0.2f, (levelIndex - 5) / 195f);
        }

        foreach (var config in layers.Select((c, idx) => (c, idx)))
        {
            float centerX = config.c.width / 2f;
            for (int y = 0; y < config.c.height; y++)
            {
                for (int x = 0; x < config.c.width; x++)
                {
                    if (levelIndex > 5)
                    {
                        float dx = Mathf.Abs(x - centerX);
                        float sampleX = dx * noiseScale + noiseSeed;
                        float sampleY = y * noiseScale + noiseSeed;
                        float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);
                        if (noiseValue < effectiveNoiseThreshold) continue;
                    }

                    Vector2 offset = new Vector2((config.c.width - 1) * SpecingX / 2f, (config.c.height - 1) * SpecingY / 2f);
                    Vector3 pos = new Vector3(x * SpecingX - offset.x, y * SpecingY - offset.y, config.c.zOffset);
                    allPositions.Add((pos, config.idx + 1, config.c.zOffset));
                }
            }
        }

        // if too few positions -> fail (caller can retry)
        if (allPositions.Count < 2)
        {
            Debug.LogWarning($"Level {levelIndex}: too few positions ({allPositions.Count}) after noise. Skipping.");
            return null;
        }

        // make even
        if (allPositions.Count % 2 != 0)
            allPositions.RemoveAt(UnityEngine.Random.Range(0, allPositions.Count));

        // shuffle positions for random placement
        var allPositionsShuffled = allPositions.OrderBy(_ => UnityEngine.Random.value).ToList();

        int totalTiles = allPositionsShuffled.Count;
        int totalPairs = totalTiles / 2;

        // enforce 50% pictorial tiles and 50% numeric tiles => half pairs pictorial, half numeric
        int pictPairsWanted = totalPairs / 2;
        int numericPairsWanted = totalPairs - pictPairsWanted;

        // gather tile pools
        var pictorials = TileData.TileSettings.Where(t => t.TypeOfTile == "Pic").ToList();
        var numbers = TileData.TileSettings.Where(t => t.TypeOfTile != "Pic").ToList();

        // -------- Step 3: Choose 2-3 targets (PicMatch only) and reserve their pairs ----------
        int targetCount = UnityEngine.Random.Range(2, 4);
        var targets = new List<SavedLevelTarget>();
        var availablePics = new List<TileSetting>(pictorials);

        // Shuffle and choose up to targetCount unique pics (if not enough pictorials, choose less)
        var chosenPics = availablePics.OrderBy(_ => UnityEngine.Random.value).Take(Mathf.Min(targetCount, availablePics.Count)).ToList();

        Dictionary<string, int> targetPairsRequired = new();

        foreach (var pic in chosenPics)
        {
            // choose pairs for target - try to keep reasonable for level size
            int maxAllowed = Mathf.Max(1, pictPairsWanted); // at least 1 but not more than available pict pairs
            int minReq = Mathf.Clamp(1, 1, maxAllowed);
            int pairs = UnityEngine.Random.Range(minReq, Mathf.Clamp(4, 1, maxAllowed) + 1); // 1..4 but clamped
            pairs = Mathf.Min(pairs, pictPairsWanted); // don't exceed availability

            if (pairs <= 0) continue;

            targets.Add(new SavedLevelTarget
            {
                TargetType = "PicMatch",
                TargetValue = pic.ID,
                RequiredCount = pairs
            });

            targetPairsRequired[pic.ID] = pairs;
            pictPairsWanted -= pairs;
        }

        // ensure we still have at least two targets (if pictorial pool too small, fallback: create dummy small targets)
        if (targets.Count < 2)
        {
            // try to create additional small pic targets (may duplicate)
            var fallbackPick = pictorials.FirstOrDefault();
            if (fallbackPick != null)
            {
                int pairs = Mathf.Max(1, Mathf.Min(2, pictPairsWanted));
                targets.Add(new SavedLevelTarget { TargetType = "PicMatch", TargetValue = fallbackPick.ID, RequiredCount = pairs });
                if (!targetPairsRequired.ContainsKey(fallbackPick.ID)) targetPairsRequired[fallbackPick.ID] = 0;
                targetPairsRequired[fallbackPick.ID] += pairs;
                pictPairsWanted = Mathf.Max(0, pictPairsWanted - pairs);
            }
        }

        // If pictorial pool is empty, fall back to at least create SumTo10 target if numeric pairs available
        if (targets.Count < 2 && numbers.Count > 0)
        {
            // require some number pairs as sum-to-10
            int reqPairs = Mathf.Max(1, Mathf.Min(2, numericPairsWanted));
            targets.Add(new SavedLevelTarget { TargetType = "SumTo10", TargetValue = "10", RequiredCount = reqPairs });
            numericPairsWanted = Mathf.Max(0, numericPairsWanted - reqPairs);
        }

        // -------- Step 4: Place target pairs first (consume positions from the front) ----------
        var savedTiles = new List<SavedTileData>();
        int positionIndex = 0;

        foreach (var kvp in targetPairsRequired)
        {
            string tileId = kvp.Key;
            int pairsNeeded = kvp.Value;
            var tileSetting = pictorials.FirstOrDefault(t => t.ID == tileId);
            if (tileSetting == null) continue;

            for (int i = 0; i < pairsNeeded; i++)
            {
                if (positionIndex + 1 >= allPositionsShuffled.Count) break;
                var pos1 = allPositionsShuffled[positionIndex++];
                var pos2 = allPositionsShuffled[positionIndex++];

                savedTiles.Add(MakeTile(tileSetting, pos1));
                savedTiles.Add(MakeTile(tileSetting, pos2));
            }
        }

        // -------- Step 5: Determine per-layer same-layer pairs requirement ----------
        // Remaining positions after target placement:
        var remainingPositions = allPositionsShuffled.Skip(positionIndex).ToList();

        // group remaining positions by layer for same-layer pairing
        var remainingByLayer = remainingPositions.GroupBy(p => p.layer)
                                                 .ToDictionary(g => g.Key, g => g.ToList());

        // For each layer we will ensure ~50% of that layer's remaining positions are paired within layer
        var sameLayerPairSlots = new Dictionary<int, int>(); // layer -> number of pairs to create in that layer
        int sumSameLayerPairs = 0;

        foreach (var kv in remainingByLayer)
        {
            var list = kv.Value;
            int layerTileCount = list.Count;
            int desiredTilesSameLayer = Mathf.FloorToInt(layerTileCount * 0.5f); // number of tiles to be matched inside layer
            if (desiredTilesSameLayer % 2 != 0) desiredTilesSameLayer--; // make even number of tiles
            if (desiredTilesSameLayer < 0) desiredTilesSameLayer = 0;
            int pairsForLayer = desiredTilesSameLayer / 2;
            // don't ask for more pairs than available positions
            pairsForLayer = Mathf.Min(pairsForLayer, list.Count / 2);
            sameLayerPairSlots[kv.Key] = pairsForLayer;
            sumSameLayerPairs += pairsForLayer;
        }

        // Clamp pair counts to availability of pict/numeric pairs
        int totalSameLayerPairs = sumSameLayerPairs;
        // We'll allocate pict/numeric type to same-layer slots preferring to fill pictorial requirement first
        int pictRemaining = pictPairsWanted;
        int numericRemaining = numericPairsWanted;

        // But remember: we earlier already subtracted target pict pairs from pictPairsWanted. Good.

        // -------- Step 6: Fill same-layer pairs by iterating layers and creating pairs of correct type ----------
        var layerKeys = remainingByLayer.Keys.ToList(); // snapshot keys

        foreach (var layerKey in layerKeys)
        {
            var posList = remainingByLayer[layerKey].OrderBy(_ => UnityEngine.Random.value).ToList();
            int pairsToMake = sameLayerPairSlots[layerKey];

            for (int p = 0; p < pairsToMake; p++)
            {
                if (posList.Count < 2) break;
                var posA = posList[0];
                var posB = posList[1];
                posList.RemoveAt(0);
                posList.RemoveAt(0);

                (TileSetting t1, TileSetting t2) pair;

                if (pictRemaining > 0)
                {
                    pair = PickRandomPictorialPair();
                    pictRemaining--;
                }
                else if (numericRemaining > 0)
                {
                    pair = PickRandomNumericPair(numbers);
                    numericRemaining--;
                }
                else
                {
                    pair = PickRandomPictorialPair();
                }

                savedTiles.Add(MakeTile(pair.t1, posA));
                savedTiles.Add(MakeTile(pair.t2, posB));
            }

            // now safely replace the value in dictionary
            remainingByLayer[layerKey] = posList;
        }


        // rebuild leftover positions after same-layer allocation
        var leftovers = remainingByLayer.SelectMany(kv => kv.Value).ToList();

        // ensure even number of leftover positions
        if (leftovers.Count % 2 != 0)
            leftovers.RemoveAt(UnityEngine.Random.Range(0, leftovers.Count));

        // -------- Step 7: Fill cross-layer pairs from leftover positions respecting pict/numeric counts ----------
        for (int i = 0; i < leftovers.Count; i += 2)
        {
            var posA = leftovers[i];
            var posB = leftovers[i + 1];

            (TileSetting t1, TileSetting t2) pair;
            if (pictRemaining > 0 && pictorials.Count > 0)
            {
                pair = PickRandomPictorialPair();
                pictRemaining--;
            }
            else if (numericRemaining > 0 && numbers.Count > 0)
            {
                pair = PickRandomNumericPair(numbers);
                numericRemaining--;
            }
            else if (pictorials.Count > 0)
            {
                pair = PickRandomPictorialPair();
            }
            else
            {
                pair = PickRandomNumericPair(numbers);
            }

            savedTiles.Add(MakeTile(pair.t1, posA));
            savedTiles.Add(MakeTile(pair.t2, posB));
        }

        // -------- Step 8: Sanity checks and finalization ----------
        // If counts mismatch (rare fallback cases), ensure all tiles are in pairs and tile count equals positions
        if (savedTiles.Count != totalTiles)
        {
            // attempt to trim or report
            Debug.LogWarning($"Level {levelIndex}: savedTiles ({savedTiles.Count}) != totalTiles ({totalTiles}). Adjusting/trimming.");
            // Trim or pad with pictorial duplicate pairs (simple fallback)
            while (savedTiles.Count > totalTiles) savedTiles.RemoveAt(savedTiles.Count - 1);
            while (savedTiles.Count < totalTiles)
            {
                var pos = allPositionsShuffled[UnityEngine.Random.Range(0, allPositionsShuffled.Count)];
                var pick = PickRandomPictorialPair();
                savedTiles.Add(MakeTile(pick.t1, pos));
                savedTiles.Add(MakeTile(pick.t2, pos)); // will overlap but ensures pairs; rare fallback
            }
        }

        float complexityScore = layers.Count * 0.5f + allPositions.Count * 0.1f;

        return new SavedLevelData
        {
            Tiles = savedTiles,
            ComplexityScore = complexityScore,
            Targets = targets
        };
    }

    // ---------- Helper functions to add into the same class ----------

    private (TileSetting t1, TileSetting t2) PickRandomPictorialPair()
    {
        var pictorials = TileData.TileSettings.Where(t => t.TypeOfTile == "Pic").ToList();
        if (pictorials.Count == 0)
            return (TileData.TileSettings[0], TileData.TileSettings[0]); // fallback

        var p = pictorials[UnityEngine.Random.Range(0, pictorials.Count)];
        return (p, p);
    }

    private (TileSetting t1, TileSetting t2) PickRandomNumericPair(List<TileSetting> numbersPool)
    {
        if (numbersPool == null || numbersPool.Count == 0)
        {
            // fallback to any tile
            var any = TileData.TileSettings[UnityEngine.Random.Range(0, TileData.TileSettings.Count)];
            return (any, any);
        }

        // Group by TypeOfTile (e.g., "Dot","Bamboo","Simple")
        var grouped = numbersPool.GroupBy(t => t.TypeOfTile).ToList();
        var candidates = new List<(TileSetting, TileSetting)>();

        foreach (var g in grouped)
        {
            var pairs = FindAllPairsThatSum10(g.ToList());
            if (pairs != null && pairs.Count > 0)
                candidates.AddRange(pairs);
        }

        if (candidates.Count > 0)
        {
            var chosen = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            return (chosen.Item1, chosen.Item2);
        }

        // fallback: try identical numeric (5,5) if exists
        var five = numbersPool.FirstOrDefault(t => t.NumberValue == 5);
        if (five != null) return (five, five);

        // last resort: pick any two numeric (possibly identical)
        var a = numbersPool[UnityEngine.Random.Range(0, numbersPool.Count)];
        var b = numbersPool[UnityEngine.Random.Range(0, numbersPool.Count)];
        return (a, b);
    }



    private SavedTileData MakeTile(TileSetting t, (Vector3 pos, int layer, float z) info)
    {
        return new SavedTileData
        {
            TileID = t.ID,
            TypeOfTile = t.TypeOfTile,
            NumberValue = t.NumberValue,
            Position = info.pos,
            Layer = info.layer,
            ZOffset = info.z
        };
    }

    private (TileSetting, TileSetting) GenerateRandomTilePair()
    {
        var all = TileData.TileSettings;
        if (all == null || all.Count == 0) return (null, null);

        var pictorials = all.Where(t => t.TypeOfTile == "Pic").ToList();
        var numbers = all.Where(t => t.TypeOfTile != "Pic").ToList();

        if (pictorials.Count > 0 && UnityEngine.Random.value < 0.5f)
        {
            var p = pictorials[UnityEngine.Random.Range(0, pictorials.Count)];
            return (p, p);
        }

        if (numbers.Count > 0)
        {
            var pairs = new List<(TileSetting, TileSetting)>();
            var grouped = numbers.GroupBy(t => t.TypeOfTile).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var g in grouped)
            {
                var found = FindAllPairsThatSum10(g.Value);
                if (found.Count > 0) pairs.AddRange(found);
            }

            if (pairs.Count > 0)
                return pairs[UnityEngine.Random.Range(0, pairs.Count)];

            var anyNum = numbers[UnityEngine.Random.Range(0, numbers.Count)];
            return (anyNum, anyNum);
        }

        var anyTile = all[UnityEngine.Random.Range(0, all.Count)];
        return (anyTile, anyTile);
    }

    private List<(TileSetting, TileSetting)> FindAllPairsThatSum10(List<TileSetting> list)
    {
        var result = new List<(TileSetting, TileSetting)>();
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = i; j < list.Count; j++)
            {
                if (list[i].NumberValue + list[j].NumberValue == 10)
                    result.Add((list[i], list[j]));
            }
        }
        return result;
    }

    private List<SavedLevelTarget> GenerateLevelTargets(List<SavedTileData> tiles)
    {
        var targets = new List<SavedLevelTarget>();

        var numericGroups = tiles.Where(t => t.TypeOfTile != "Pic")
            .GroupBy(t => t.NumberValue)
            .ToDictionary(g => g.Key, g => g.Count());

        var sum10Pairs = new List<(int, int)>();
        foreach (var kv in numericGroups)
        {
            int num = kv.Key;
            int complement = 10 - num;
            if (numericGroups.ContainsKey(complement) && num <= complement)
            {
                int pairsPossible = Mathf.Min(numericGroups[num], numericGroups[complement]) / (num == complement ? 2 : 1);
                if (pairsPossible >= 2)
                    sum10Pairs.Add((num, complement));
            }
        }

        if (sum10Pairs.Count > 0)
        {
            targets.Add(new SavedLevelTarget
            {
                TargetType = "SumTo10",
                TargetValue = "10",
                RequiredCount = UnityEngine.Random.Range(3, 6)
            });
        }

        var picGroups = tiles.Where(t => t.TypeOfTile == "Pic")
            .GroupBy(t => t.TileID)
            .Select(g => new { ID = g.Key, Count = g.Count() })
            .Where(g => g.Count >= 3)
            .ToList();

        if (picGroups.Count > 0)
        {
            var chosenPic = picGroups[UnityEngine.Random.Range(0, picGroups.Count)];
            targets.Add(new SavedLevelTarget
            {
                TargetType = "PicMatch",
                TargetValue = chosenPic.ID,
                RequiredCount = Mathf.Min(5, chosenPic.Count)
            });
        }

        while (targets.Count < 2)
        {
            targets.Add(new SavedLevelTarget
            {
                TargetType = "PicMatch",
                TargetValue = picGroups.Count > 0 ? picGroups[0].ID : "Pic_Default",
                RequiredCount = 3
            });
        }

        return targets;
    }
}

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Game.Scriptables;

public class LevelVisualizerWindow : EditorWindow
{
    private int levelNumber = 1;
    private string levelsPath;
    private GameObject previewRoot;

    private TileData tileData;   // Your TileData asset
    private IconData iconData;   // Your IconData asset
    private LevelJsonCreator levelCreator; // Reference to generator

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

    [System.Serializable]
    public class SavedLevelTarget
    {
        public string TargetType;   // "SumTo10" or "PicMatch"
        public string TargetValue;  // For PicMatch: "Pic_4", for SumTo10: "10"
        public int RequiredCount;   // number of pairs or tiles required
    }


    [MenuItem("Tools/Level Visualizer")]
    public static void ShowWindow()
    {
        GetWindow<LevelVisualizerWindow>("Level Visualizer");
    }

    private void OnEnable()
    {
        levelsPath = Path.Combine(Application.dataPath, "Levels");
    }

    private SavedLevelData loadedLevelData; // store the last loaded level

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Level Visualizer", EditorStyles.boldLabel);

        levelNumber = EditorGUILayout.IntSlider("Level Number", levelNumber, 1, 1000);
        tileData = (TileData)EditorGUILayout.ObjectField("Tile Data Asset", tileData, typeof(TileData), false);
        iconData = (IconData)EditorGUILayout.ObjectField("Icon Data Asset", iconData, typeof(IconData), false);
        levelCreator = (LevelJsonCreator)EditorGUILayout.ObjectField("Level Generator", levelCreator, typeof(LevelJsonCreator), true);

        EditorGUILayout.Space();

        if (GUILayout.Button("Load Level"))
        {
            LoadLevel(levelNumber);
        }

        if (GUILayout.Button("Regenerate Level"))
        {
           
            RegenerateLevel(levelNumber);
            LoadLevel(levelNumber);
        }

        if (previewRoot != null && GUILayout.Button("Clear Preview"))
        {
            DestroyImmediate(previewRoot);
        }

        // Show loaded level targets
        if (loadedLevelData != null && loadedLevelData.Targets != null && loadedLevelData.Targets.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("🎯 Level Targets:", EditorStyles.boldLabel);
            //foreach (var target in loadedLevelData.Targets)
            //{
            //    string label = $"- {target.TargetType} | {target.TargetValue} | Required: {target.RequiredCount}";
            //    EditorGUILayout.LabelField(label);
            //}
            foreach (var target in loadedLevelData.Targets)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(target.TargetType, GUILayout.Width(100));
                EditorGUILayout.LabelField(target.TargetValue, GUILayout.Width(100));
                EditorGUILayout.LabelField($"x{target.RequiredCount}", GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();
            }

        }

    }

    private void RegenerateLevel(int number)
    {
        if (levelCreator == null)
        {
            Debug.LogError("❌ Please assign your LevelJsonCreator instance.");
            return;
        }

        var levelData = levelCreator.GenerateSingleLevelForSave(number);
        if (levelData == null)
        {
            Debug.LogError("Failed to regenerate level " + number);
            return;
        }

        string json = JsonUtility.ToJson(levelData, true);
        string filePath = Path.Combine(levelsPath, $"Level_{number:D3}.json");
        File.WriteAllText(filePath, json);

        AssetDatabase.Refresh();
        Debug.Log($"✅ Level {number} regenerated and saved to {filePath}");
    }

    private void LoadLevel(int number)
    {
        if (!tileData)
        {
            Debug.LogError("❌ Please assign your TileData asset.");
            return;
        }
        if (!iconData)
        {
            Debug.LogError("❌ Please assign your IconData asset.");
            return;
        }
        if (!Directory.Exists(levelsPath))
        {
            Debug.LogError("Levels folder not found: " + levelsPath);
            return;
        }

        string filePath = Path.Combine(levelsPath, $"Level_{number:D3}.json");
        if (!File.Exists(filePath))
        {
            Debug.LogError("Level file not found: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        loadedLevelData = JsonUtility.FromJson<SavedLevelData>(json);

        if (loadedLevelData == null || loadedLevelData.Tiles == null || loadedLevelData.Tiles.Count == 0)
        {
            Debug.LogError("Failed to parse level data or no tiles present.");
            return;
        }

        if (previewRoot != null) DestroyImmediate(previewRoot);
        previewRoot = new GameObject($"Preview_Level_{number}");

        foreach (var tile in loadedLevelData.Tiles)
        {
            var iconSetting = iconData.IconSettings.FirstOrDefault(i => i.ID == tile.TileID);
            if (iconSetting == null)
            {
                Debug.LogWarning($"No Icon found for TileID: {tile.TileID}");
                continue;
            }

            GameObject go = new GameObject($"{tile.TileID} ({tile.TypeOfTile})");
            go.transform.SetParent(previewRoot.transform);
            go.transform.localPosition = tile.Position;

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = iconSetting.Icon;

            Color layerColor = Color.HSVToRGB((tile.Layer * 0.15f) % 1f, 0.3f, 1f);
            renderer.color = layerColor;
        }

        Selection.activeGameObject = previewRoot;
    }

}

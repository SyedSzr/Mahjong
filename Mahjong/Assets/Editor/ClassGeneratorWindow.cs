using UnityEngine;
using UnityEditor;
using System.IO;

public class ClassGeneratorWindow : EditorWindow
{
    private string className = "NewClass";
    private ClassType selectedType = ClassType.Screen;

    private enum ClassType
    {
        Screen,
        Popup,
        Button,
        Setting,
        Scriptable,
        Component ,
        Text, // ✅ Added
        Manager // ✅ Added

    }

    [MenuItem("Tools/Class Generator")]
    public static void ShowWindow()
    {
        GetWindow<ClassGeneratorWindow>("Class Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate Class", EditorStyles.boldLabel);
        className = EditorGUILayout.TextField("Class Name", className);
        selectedType = (ClassType)EditorGUILayout.EnumPopup("Class Type", selectedType);

        if (GUILayout.Button("Generate"))
        {
            GenerateClassFile(className, selectedType);
        }
    }

    private void GenerateClassFile(string className, ClassType type)
    {
        string directory = GetDirectory(type);
        string fullPath = Path.Combine(directory, $"{className}.cs");

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        if (File.Exists(fullPath))
        {
            Debug.LogWarning($"File {fullPath} already exists.");
            return;
        }

        string code = type switch
        {
            ClassType.Screen => GenerateMonoClass(className, "BaseScreen", "Game.Screen"),
            ClassType.Popup => GenerateMonoClass(className, "BasePopup", "Game.Popups"),
            ClassType.Button => GenerateMonoClass(className, "ButtonComponent", "Game.Utilities", isOverrideButton: true),
            ClassType.Setting => GenerateSettingClass(className),
            ClassType.Scriptable => GenerateScriptableClass(className),
            ClassType.Component => GenerateComponentClass(className),
            ClassType.Manager => GenerateManagerClass(className), // ✅ Added
            ClassType.Text => GenerateTextClass(className), // ✅ Added

            _ => ""
        };

        File.WriteAllText(fullPath, code);
        AssetDatabase.Refresh();

        // Auto-open the file in the default code editor
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(fullPath);
        if (asset != null)
        {
            AssetDatabase.OpenAsset(asset);
        }

        Debug.Log($"{className} created at {directory}");

    }

    private string GetDirectory(ClassType type)
    {
        return type switch
        {
            ClassType.Screen => "Assets/GameAssets/Scripts/Screens",
            ClassType.Popup => "Assets/GameAssets/Scripts/Popups",
            ClassType.Button => "Assets/GameAssets/Scripts/Buttons",
            ClassType.Setting => "Assets/GameAssets/Scripts/Settings",
            ClassType.Scriptable => "Assets/GameAssets/Scripts/Scriptables",
            ClassType.Component => "Assets/GameAssets/Scripts/Components", // ✅ New
            ClassType.Manager => "Assets/GameAssets/Scripts/Managers", // ✅ Added
            ClassType.Text => "Assets/GameAssets/Scripts/Texts", // ✅ New


            _ => "Assets/Scripts"
        };
    }

    private string GenerateTextClass(string className)
    {
        return $@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utilities
{{
    public class {className} : TextComponent
    {{
        
    }}
}}";
    }


    private string GenerateMonoClass(string className, string baseClass, string ns, bool isOverrideButton = false)
    {
        string buttonMethod = isOverrideButton
            ? $@"
        public override void OnButtonClicked()
        {{
            base.OnButtonClicked();
        }}"
            : "";

        return $@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;

namespace {ns}
{{
    public class {className} : {baseClass}
    {{
        {buttonMethod}
    }}
}}";
    }

    private string GenerateSettingClass(string className)
    {
        return $@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings
{{
    [System.Serializable]
    public class {className}
    {{
        public string ID;
    }}
}}";
    }

    private string GenerateScriptableClass(string className)
    {
        return $@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Settings;

namespace Game.Scriptables
{{
    [CreateAssetMenu(menuName = ""Mahjong/{className}"")]
    public class {className} : ScriptableObject
    {{

    }}
}}";
    }

    private string GenerateComponentClass(string className)
    {
        return $@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Settings;

namespace Game.Components
{{
    public class {className} : MonoBehaviour
    {{
        // Start is called before the first frame update
        void Start()
        {{

        }}

        // Update is called once per frame
        void Update()
        {{

        }}
    }}
}}";
    }

    private string GenerateManagerClass(string className)
    {
        return $@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Settings;
using Game.Components;

namespace Game.Managers
{{
    public class {className} : MonoBehaviour
    {{
        // Start is called before the first frame update
        void Start()
        {{

        }}

        // Update is called once per frame
        void Update()
        {{

        }}
    }}
}}";
    }


}

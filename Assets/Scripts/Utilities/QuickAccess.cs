#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class QuickAccess
{
    [MenuItem("QuickAccess/Initialization Scene", priority = 0)]
    public static void OpenInitializationScene() => OpenScene("Assets/Level/Scenes/Initialization.unity");
    [MenuItem("QuickAccess/Login Scene", priority = 1)]
    public static void OpenLoginScene() => OpenScene("Assets/Level/Scenes/AutoLogin.unity");
    [MenuItem("QuickAccess/Lobby Scene", priority = 2)]
    public static void OpenLobbyScene() => OpenScene("Assets/Level/Scenes/Lobby.unity");
    [MenuItem("QuickAccess/Game Scene", priority = 3)]
    public static void OpenGameScene() => OpenScene("Assets/Level/Scenes/Game.unity");
    [MenuItem("QuickAccess/Splash Scene", priority = 4)]


    private static void OpenScene(string scenePath) => EditorSceneManager.OpenScene(scenePath);
    private static void FocusFolder(string folderName)
    {
        var guids = AssetDatabase.FindAssets($"t:folder {folderName}");

        var path = guids.Select(AssetDatabase.GUIDToAssetPath).First(s => s.Equals($"Assets/{folderName}"));
        path = Directory.GetDirectories(path)[0];
        var type = AssetDatabase.GetMainAssetTypeAtPath(path);
        var obj = AssetDatabase.LoadAssetAtPath(path, type);
        EditorUtility.FocusProjectWindow();
        EditorGUIUtility.PingObject(obj);

    }
    public static void OpenScriptableObject(string fileName, string unityType = "scriptableobject")
    {
        var guids = AssetDatabase.FindAssets($"t:{unityType} {fileName}");
        if (guids.Length != 1)
        {
            Debug.LogWarning($"QuickAccess Error: Found more than one asset match for {fileName}");
            return;
        }
        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
        var type = AssetDatabase.GetMainAssetTypeAtPath(path);
        var obj = AssetDatabase.LoadAssetAtPath(path, type);
        AssetDatabase.OpenAsset(obj);
        EditorGUIUtility.PingObject(obj);
    }
}
#endif
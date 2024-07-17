#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueGames.Utilities
{
    [InitializeOnLoadAttribute]
    public static class EditorSceneLoader
    {
        private const string PlayMenuKey = "Tools/GlueGames/Play From Initialization";

        private static bool _playFromInit
        {
            get { return !EditorPrefs.HasKey(PlayMenuKey) || EditorPrefs.GetBool(PlayMenuKey); }
            set { EditorPrefs.SetBool(PlayMenuKey, value); }
        }

        [MenuItem(PlayMenuKey, false, 0)]
        private static void PlayFromFirstSceneCheckMenu()
        {
            _playFromInit = !_playFromInit;
            Menu.SetChecked(PlayMenuKey, _playFromInit);
            ShowNotifyOrLog(_playFromInit ? "Play from Intialization" : "Play from current scene");
        }

        // The menu won't be gray out, we use this validate method for update check state.
        [MenuItem(PlayMenuKey, true)]
        private static bool PlayFromFirstSceneCheckMenuValidate()
        {
            Menu.SetChecked(PlayMenuKey, _playFromInit);
            return true;
        }

        // This method is called before any Awake. It's the perfect callback for this feature.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadFirstSceneAtGameBegins()
        {
            // 0 - Tracking Consent

            if (!_playFromInit || EditorBuildSettings.scenes.Length == 0 || SceneManager.GetActiveScene().buildIndex <= 1)
            {
                return;
            }

            Commons.LaunchNormally = false;
            Commons.StartingScene = SceneManager.GetActiveScene().path;
            Commons.StartingSceneName = SceneManager.GetActiveScene().name;
            foreach (GameObject go in Object.FindObjectsOfType<GameObject>())
            {
                go.SetActive(false);
            }

            SceneManager.LoadScene(1); //Load from Initialization scene
        }

        private static void ShowNotifyOrLog(string msg)
        {
            if (Resources.FindObjectsOfTypeAll<SceneView>().Length > 0)
            {
                EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent(msg));
            }
            else
            {
                Debug.Log(msg); // When there's no scene view opened, we just print a log.
            }
        }

    }
}
#endif

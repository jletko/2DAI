using UnityEditor;
using UnityEditor.SceneManagement;

namespace Assets.AutoSave.Editor
{
    [InitializeOnLoad]
    public class AutoSaveExtension
    {
        // Static constructor that gets called when unity fires up.
        static AutoSaveExtension()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            // If we're about to run the scene...
            if (playModeStateChange == PlayModeStateChange.ExitingEditMode)
            {
                // Save the scene and the assets.
                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
            }
        }
    }
}
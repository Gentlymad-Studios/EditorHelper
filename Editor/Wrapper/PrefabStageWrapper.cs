using System.Reflection;
using UnityEditor.SceneManagement;

namespace EditorHelper {
    public class PrefabStageWrapper {
        private static MethodInfo savePrefabMethod = null;

        static PrefabStageWrapper() {
            savePrefabMethod = typeof(PrefabStage).GetMethod("SavePrefab", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static void SavePrefab(PrefabStage prefabStage = null) {
            if (prefabStage == null) {
                prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            }

            if (prefabStage == null) {
                return;
            }

            savePrefabMethod.Invoke(prefabStage, null);
        }
    }
}
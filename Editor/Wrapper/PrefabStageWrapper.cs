using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EditorHelper {
    public class PrefabStageWrapper {
        private static MethodInfo savePrefabMethod = null;
        private static void SetupPrefabStageWrapper() {
            if (savePrefabMethod == null) {
                savePrefabMethod = typeof(PrefabStage).GetMethod("SavePrefab", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        public static void SavePrefab(PrefabStage prefabStage = null) {
            SetupPrefabStageWrapper();

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
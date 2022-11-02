#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorHelper {
    /// <summary>
    /// Small wrapper to manage scene view items more orderly.
    /// </summary>
    [ExecuteInEditMode]
    public class SceneViewManager : MonoBehaviour {

        private List<Func<int, int>> actions = new List<Func<int, int>>();

        public static void Register(Func<int, int> action) {
            SceneViewManager sceneViewManager = FindObjectOfType<SceneViewManager>();
            if (sceneViewManager != null) {
                if (!sceneViewManager.actions.Contains(action)) {
                    sceneViewManager.actions.Add(action);
                }
            }
        }

        private void OnEnable() {
            // subscribe to the scene view
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable() {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView) {
            int y = 0;
            foreach (var action in actions) {
                y += action(y);
            }
        }
    }
}
#endif

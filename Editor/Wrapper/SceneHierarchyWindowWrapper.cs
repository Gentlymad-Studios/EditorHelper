using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorHelper {
    public static class SceneHierarchyWindowWrapper {
        private static Type T;
        private static EditorWindow window;

        static SceneHierarchyWindowWrapper() {
            T = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            window = EditorWindow.GetWindow(T);
        }

        /// <summary>
        /// SetExpandRecursive in hierarchy for the given gameobject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="expanded"></param>
        public static void SetExpandedRecursive(GameObject gameObject, bool expanded) {
            MethodInfo exprec = T.GetMethod("SetExpandedRecursive");
            exprec!.Invoke(window, new object[] { gameObject.GetInstanceID(), expanded });
        }
    }
}
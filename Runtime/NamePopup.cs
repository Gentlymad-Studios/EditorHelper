#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace EditorHelper {
    /// <summary> Utility for renaming assets </summary>
    public class NamePopup : EditorWindow {
        public static NamePopup current { get; private set; }
        public UnityEngine.Object target;
        public string input;
        public Action<string> applyAction;
        private bool firstFrame = true;

        /// <summary> Show a rename popup for an asset at mouse position. Will trigger reimport of the asset on apply.
        public static NamePopup Show(UnityEngine.Object target, Action<string> applyAction, string defaultName = null, float width = 200) {
            NamePopup window = EditorWindow.GetWindow<NamePopup>(true, "Graph Name", true);
            if (current != null) current.Close();
            current = window;
            window.target = target;
            window.applyAction = applyAction;
            if (defaultName == null) {
                window.input = target.GetType().ToString();
            } else {
                window.input = defaultName;
            }
            window.minSize = new Vector2(100, 44);
            window.position = new Rect(0, 0, width, 44);
            GUI.FocusControl("ClearAllFocus");
            window.UpdatePositionToMouse();
            return window;
        }

        private void UpdatePositionToMouse() {
            if (Event.current == null) return;
            Vector3 mousePoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            Rect pos = position;
            pos.x = mousePoint.x - position.width * 0.5f;
            pos.y = mousePoint.y - 10;
            position = pos;
        }

        private void OnLostFocus() {
            // Make the popup close on lose focus
            Close();
        }

        private void OnGUI() {
            if (firstFrame) {
                UpdatePositionToMouse();
                firstFrame = false;
            }
            input = EditorGUILayout.TextField(input);
            Event e = Event.current;
            // If input is empty, revert name to default instead
            if (input != null && input.Trim() != "") {
                if (GUILayout.Button("Apply") || (e.isKey && e.keyCode == KeyCode.Return)) {
                    applyAction(input);
                    Close();
                }
            }
        }
    }
}
#endif
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace EditorHelper {

    public abstract class SettingsProviderBase : SettingsProvider {
        protected const string basePath = "Project/Tools/";

        private bool retrieved = false;
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        protected SerializedObject serializedObject;
        private List<SerializedProperty> serializedProperties = new List<SerializedProperty>();

        public SettingsProviderBase(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) {}

        public abstract dynamic GetInstance();
        public abstract System.Type GetDataType();
        protected abstract void OnChange();
        protected virtual bool OnCustomChangeCheck() { return false; }

        // This function is called when the user clicks on the MyCustom element in the Settings window.
        public override void OnActivate(string searchContext, VisualElement rootElement) {
            serializedProperties.Clear();
            serializedObject = new SerializedObject(GetInstance());
            retrieved = false;
        }

        public override void OnGUI(string searchContext) {
            // retrieve serialized properties
            if (!retrieved) {
                RetrieveAllSerializedProperties(GetDataType());
                retrieved = true;
            }
            // Use IMGUI to display UI:
            DrawBaseInspector();
        }

        private void RetrieveAllSerializedProperties(System.Type type) {
            FieldInfo[] fields = type.GetFields(flags);
            SerializedProperty prop;
            foreach (FieldInfo info in fields) {
                prop = serializedObject.FindProperty(info.Name);
                if (prop != null) {
                    serializedProperties.Add(prop);
                }
            }
        }

        private void DrawBaseInspector() {
            serializedObject.Update();
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            EditorGUI.BeginChangeCheck();
            foreach (SerializedProperty property in serializedProperties) {
                EditorGUILayout.PropertyField(property, true);
            }
            if (EditorGUI.EndChangeCheck() || OnCustomChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
                OnChange();
            }
            EditorGUILayout.EndVertical();
        }
    }
}
#endif

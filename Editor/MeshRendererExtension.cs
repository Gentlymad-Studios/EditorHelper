using UnityEditor;
using UnityEngine;

namespace EditorHelper {
    public static class MeshRendererExtension {
        /// <summary>
		/// Check if the Material for the given submesh is overridden
		/// </summary>
		/// <param name="meshRenderer"></param>
		/// <param name="subMeshIndex"></param>
		/// <returns></returns>
		public static bool MaterialIsOverridden(this MeshRenderer meshRenderer, int subMeshIndex) {
            //return if object is not a prefab
            if (!PrefabUtility.IsPartOfPrefabInstance(meshRenderer)) {
                return true;
            }

            SerializedObject so = new SerializedObject(meshRenderer);
            SerializedProperty property = so.FindProperty("m_Materials");
            PropertyModification[] mods = PrefabUtility.GetPropertyModifications(meshRenderer);

            foreach (PropertyModification mod in mods) {
                if (mod.target.name == meshRenderer.gameObject.name) {
                    if (mod.propertyPath == $"m_Materials.Array.data[{subMeshIndex}]") {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Revert the override for the given submesh material
        /// </summary>
        /// <param name="meshRenderer"></param>
        /// <param name="subMeshIndex"></param>
        public static void RevertMaterialOverride(this MeshRenderer meshRenderer, int subMeshIndex) {
            //return if object is not a prefab
            if (!PrefabUtility.IsPartOfPrefabInstance(meshRenderer)) {
                return;
            }

            SerializedObject so = new SerializedObject(meshRenderer);
            SerializedProperty property = so.FindProperty("m_Materials");
            SerializedProperty subProperty = property.GetArrayElementAtIndex(subMeshIndex);

            PrefabUtility.RevertPropertyOverride(subProperty, InteractionMode.AutomatedAction);
        }
    }
}
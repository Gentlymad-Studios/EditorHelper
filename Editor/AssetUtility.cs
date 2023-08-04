using UnityEditor;
using UnityEngine;

namespace EditorHelper {
    public static class AssetUtility {
        /// <summary>
		/// Check if the given Object has the given label
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="label"></param>
		/// <returns></returns>
		public static bool HasLabel(Object obj, string label) {
            string[] labels = AssetDatabase.GetLabels(obj);

            label = label.ToLowerInvariant();

            for (int i = 0; i < labels.Length; i++) {
                if (labels[i].ToLowerInvariant() == label) {
                    return true;
                }
            }

            return false;
        }
    }
}
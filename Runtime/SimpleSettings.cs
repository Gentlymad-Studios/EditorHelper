#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace EditorHelper {
	public class SimpleSettings<T> : ScriptableSingleton<T> where T : SimpleSettings<T> {
		public void OnEnable() {
			hideFlags &= ~HideFlags.NotEditable;
		}

		public void Save() {
			base.Save(true);
		}
	}
}
#endif
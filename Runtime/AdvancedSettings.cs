using UnityEditor;
using UnityEngine;

namespace EditorHelper {
	/// <summary>
	/// Settings class to make it easier to create custom project settings
	/// </summary>
	/// <typeparam name="T">The concrete class type of these settings</typeparam>
	public abstract class AdvancedSettings<T> : ScriptableSingleton<T> where T : ScriptableObject {
		/// <summary>
		/// The menu path for the project settings window
		/// </summary>
		public abstract string Path { get; }

		/// <summary>
		/// make sure that our settings are editable
		/// </summary>
		public void OnEnable() {
			hideFlags &= ~HideFlags.NotEditable;
		}

		/// <summary>
		/// Make sure we have a callable Save method to save the settings
		/// </summary>
		public void Save() {
			Save(true);
		}
	}
}

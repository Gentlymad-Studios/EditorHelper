#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Reflection;

namespace EditorHelper {

	/// <summary>
	/// Further simplifications of settings providers and settings. This  reduces the amount of boilerplate code needed to get funtional project settings for custom data.
	/// </summary>
	public abstract class AdvancedSingletonProviderBase<T> : ScriptableSingletonProviderBase where T : AdvancedSettings<T> {
		
		/// <summary>
		/// The concrete settings object
		/// </summary>
		private T settings;

		/// <summary>
		/// Cached flag composition so we can retrieve getter methods
		/// </summary>
		private const BindingFlags CachedFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

		/// <summary>
		/// Get the static ScriptableSingleton instance Getter method for the specific type
		/// </summary>
		private static MethodInfo _getInstanceMethod;
		private static MethodInfo _GetInstanceMethod { 
			get {
				if (_getInstanceMethod == null) {
					_getInstanceMethod = typeof(T).GetProperty(nameof(AdvancedSettings<T>.instance), CachedFlags).GetGetMethod();
				}
				return _getInstanceMethod; 
			}
		}

		/// <summary>
		/// Get the Path getter method of the Advanced settings derived class
		/// </summary>
		private static MethodInfo _getPathMethod;
		private static MethodInfo _GetPathMethod {
			get {
				if (_getPathMethod == null) {
					_getPathMethod = typeof(T).GetProperty(nameof(AdvancedSettings<T>.Path), CachedFlags).GetGetMethod();
				}
				return _getPathMethod;
			}
		}

		/// <summary>
		/// Create a settings provider of a specific type
		/// </summary>
		/// <typeparam name="T3"></typeparam>
		/// <returns></returns>
		protected static SettingsProvider CreateSettingsProvider<T3>() where T3: AdvancedSingletonProviderBase<T> {
			// get the singleton instance of type T
			object instance = _GetInstanceMethod.Invoke(null, null);
			if (instance != null) { 
				// retrieve the path of the settings instance
				object path = _GetPathMethod.Invoke(instance, null);
				// actually create the provider and pass over the retrieved path
				T3 provider = (T3)Activator.CreateInstance(typeof(T3), new object[] { path });
				return provider;
			}
			return null;
		} 

		protected AdvancedSingletonProviderBase(string path) : base(path, SettingsScope.Project) {
			keywords = GetTags();
		}

		/// <summary>
		/// The tags of this settings provider
		/// </summary>
		/// <returns></returns>
        protected abstract string[] GetTags();

        /// <summary>
        /// the callback executed when a value changed
        /// </summary>
        /// <returns></returns>
        protected override EventCallback<SerializedPropertyChangeEvent> GetValueChangedCallback() {
			return ValueChanged;
		}

		/// <summary>
		/// Get a automatically created header
		/// </summary>
		/// <returns></returns>
		protected override string GetHeader() {
			return typeof(T).Name;
		}

		/// <summary>
		/// retrieve the data type automatically
		/// </summary>
		/// <returns></returns>
		public override Type GetDataType() {
			return typeof(T);
		}

		/// <summary>
		/// get the dynamic instance object
		/// </summary>
		/// <returns></returns>
		public override dynamic GetInstance() {
			//Force HideFlags
			return GetConcreteSettings();
		}

		/// <summary>
		/// Retrieve the concrete settings object
		/// </summary>
		/// <returns></returns>
		public T GetConcreteSettings() {
			if (settings == null) {
				settings = (T)_GetInstanceMethod.Invoke(null, null);
				if (settings != null) {
					settings.OnEnable();
				}
			}
			return settings;
		}

		/// <summary>
		/// Called when any value changed.
		/// </summary>
		/// <param name="evt"></param>
		private void ValueChanged(SerializedPropertyChangeEvent evt) {
			// notify all listeneres (ReactiveSettings)
			serializedObject.ApplyModifiedProperties();

			// call save on our singleton as it is a strange hybrid and not a full ScriptableObject
			GetConcreteSettings().Save();
		}
	}
}
#endif
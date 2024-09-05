using System;
using System.Reflection;
using UnityEditor;

namespace EditorHelper {
    public static class ProjectWindowWrapper {
        //ProjectBrowser related
        private static Type projectBrowserType;
        private static FieldInfo lastInteractedProjectBrowserField;
        private static PropertyInfo isLockedProperty;
        private static MethodInfo clearSearchMethod;

        //ProjectWindowUtil related
        private static Type projectWindowUtilType;
        private static MethodInfo getActiveFolderPathMethod;
        private static MethodInfo getProjectBrowserIfExistsMethod;

        static ProjectWindowWrapper() {
            projectBrowserType = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");
            lastInteractedProjectBrowserField = projectBrowserType.GetField("s_LastInteractedProjectBrowser", BindingFlags.Static | BindingFlags.Public);
            isLockedProperty = projectBrowserType.GetProperty("isLocked", BindingFlags.NonPublic | BindingFlags.Instance);
            clearSearchMethod = projectBrowserType.GetMethod("ClearSearch", BindingFlags.NonPublic | BindingFlags.Instance);

            projectWindowUtilType = typeof(ProjectWindowUtil);
            getActiveFolderPathMethod = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            getProjectBrowserIfExistsMethod = projectWindowUtilType.GetMethod("GetProjectBrowserIfExists", BindingFlags.Static | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Select an asset by the given path
        /// </summary>
        /// <param name="path">path of the asset or just a folder</param>
        /// <param name="ping">ping the selection</param>
        /// <param name="force">force the selection</param>
        public static void SelectAsset(string path, bool ping = false, bool force = false) {
            bool wasLocked = false;
            if (force && GetProjectBrowserLock()) {
                SetProjectBrowserLock(false);
                wasLocked = true;
            }

            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            Selection.activeObject = asset;

            if (ping) {
                EditorGUIUtility.PingObject(asset);
            }

            if (wasLocked) {
                SetProjectBrowserLock(true);
            }
        }

        /// <summary>
        /// Returns the ActiveFolderPath of the ProjectWindow
        /// </summary>
        /// <returns></returns>
        public static string GetActiveFolderPath() {
            object activeFolderPath = getActiveFolderPathMethod.Invoke(null, new object[0]);
            return activeFolderPath == null ? string.Empty : activeFolderPath.ToString();
        }

        /// <summary>
        /// Returns an Array of selected folders
        /// </summary>
        /// <returns></returns>
        public static string[] GetSelectedFolder() {
            object projectBrowser = GetProjectBrowserIfExists();
            FieldInfo searchFilter_FieldInfo = projectBrowser.GetType().GetField("m_SearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);
            object searchFilter = searchFilter_FieldInfo.GetValue(projectBrowser);
            PropertyInfo folders_Property = searchFilter.GetType().GetProperty("folders");
            return folders_Property.GetValue(searchFilter) as string[];
        }

        /// <summary>
        /// Get the Lock of the last interacted ProjectBrowser
        /// </summary>
        /// <returns></returns>
        public static bool GetProjectBrowserLock() {
            object ins = GetLastInteractedProjectBrowser();
            return (bool)isLockedProperty.GetValue(ins);
        }

        /// <summary>
        /// Sets the Lock of the last interacted ProjectBrowser
        /// </summary>
        /// <param name="locked"></param>
        public static void SetProjectBrowserLock(bool locked) {
            object ins = GetLastInteractedProjectBrowser();
            isLockedProperty.SetValue(ins, locked);
        }

        /// <summary>
        /// Clear the Searchbar of the last interacted ProjectBrowser
        /// </summary>
        public static void ClearSearch() {
            object ins = GetLastInteractedProjectBrowser();
            clearSearchMethod.Invoke(ins, null);
        }

        /// <summary>
        /// Get the last interacted ProjectBrowser
        /// </summary>
        /// <returns></returns>
        public static object GetLastInteractedProjectBrowser() {
            return lastInteractedProjectBrowserField.GetValue(null);
        }

        /// <summary>
        /// Returns a ProjectBrowser if exists
        /// </summary>
        /// <returns></returns>
        public static object GetProjectBrowserIfExists() {
            return getProjectBrowserIfExistsMethod.Invoke(null, new object[0]);
        }
    }
}
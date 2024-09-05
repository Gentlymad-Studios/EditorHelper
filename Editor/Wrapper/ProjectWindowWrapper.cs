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

        static ProjectWindowWrapper() {
            projectBrowserType = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");
            lastInteractedProjectBrowserField = projectBrowserType.GetField("s_LastInteractedProjectBrowser", BindingFlags.Static | BindingFlags.Public);
            isLockedProperty = projectBrowserType.GetProperty("isLocked", BindingFlags.NonPublic | BindingFlags.Instance);
            clearSearchMethod = projectBrowserType.GetMethod("ClearSearch", BindingFlags.NonPublic | BindingFlags.Instance);

            projectWindowUtilType = typeof(ProjectWindowUtil);
            getActiveFolderPathMethod = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Returns the ActiveFolderPath of the ProjectWindow
        /// </summary>
        /// <returns></returns>
        public static string GetActiveFolderPath() {
            return getActiveFolderPathMethod.Invoke(null, new object[0]).ToString();
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
    }
}
#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorHelper {
    /// <summary>
    /// Helper class to handle common Editor scripting functions.
    /// </summary>
    public static class Utility {

        public const string toolsFolderName = "Tools";
        public const string settingsFolderName = "_Settings";
        public const string relativeBasePathToSettings = toolsFolderName + "/" + settingsFolderName;
        public const string assetsFolderName = "Assets";
        public const string assetsBaseFolder = assetsFolderName + "/";
        public const string scriptableObjectGenericFileEnding = ".asset";

        /// <summary>
        /// Get or create a Settings file that resides in the Tools folder in the correct place for a specific ScriptableObject type
        /// </summary>
        /// <typeparam name="T">The Scriptable Object type that we should create or get the asset for</typeparam>
        /// <returns>The created or retrieved scriptable object type</returns>
        public static T CreateSettingsFile<T>(string dataPath = null) where T : ScriptableObject {
            // in case no data path was supllied, we use the standard one
            if (dataPath == null) {
                dataPath = Application.dataPath;
            }

            string assetFilename = typeof(T).Name + scriptableObjectGenericFileEnding;
            string fullBaseAssetPath = Path.Combine(dataPath, relativeBasePathToSettings);
            string fullFilepath = Path.Combine(fullBaseAssetPath, assetFilename);
            string filePathRelativeToAssetFolder = Path.Combine(assetsBaseFolder, relativeBasePathToSettings, assetFilename);
            
            //Debug.Log(relativeBasePathToSettings + "\r\n" + assetFilename + "\r\n" + fullBaseAssetPath + "\r\n" + fullFilepath + "\r\n" + filePathRelativeToAssetFolder);

            if (!Directory.Exists(fullBaseAssetPath)) {
                //Debug.Log(fullBaseAssetPath);
                Directory.CreateDirectory(fullBaseAssetPath);
                AssetDatabase.Refresh();
            }

            if (!File.Exists(fullFilepath)) {
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(), filePathRelativeToAssetFolder);
                AssetDatabase.Refresh();
            }

            return AssetDatabase.LoadAssetAtPath<T>(filePathRelativeToAssetFolder);
        }

        /// <summary>
        /// Resize an array by padding it.
        /// Supports reduction as well as keeping values in order.
        /// https://stackoverflow.com/questions/6539571/how-to-resize-multidimensional-2d-array-in-c
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="padLeft"></param>
        /// <param name="padRight"></param>
        /// <param name="padTop"></param>
        /// <param name="padBottom"></param>
        public static void ResizeArray<T>(ref T[,] array, int padLeft, int padRight, int padTop, int padBottom) {
            int ow = array.GetLength(0);
            int oh = array.GetLength(1);
            int nw = ow + padLeft + padRight;
            int nh = oh + padTop + padBottom;

            int x0 = padLeft;
            int y0 = padTop;
            int x1 = x0 + ow - 1;
            int y1 = y0 + oh - 1;
            int u0 = -x0;
            int v0 = -y0;

            if (x0 < 0) x0 = 0;
            if (y0 < 0) y0 = 0;
            if (x1 >= nw) x1 = nw - 1;
            if (y1 >= nh) y1 = nh - 1;

            T[,] nArr = new T[nw, nh];
            for (int y = y0; y <= y1; y++) {
                for (int x = x0; x <= x1; x++) {
                    nArr[x, y] = array[u0 + x, v0 + y];
                }
            }
            array = nArr;
        }

        public static string[] GetDefinesForTargetGroup(BuildTargetGroup targetGroup) {
            string defineString = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            return defineString.Split(';');
        }

        public static string CurrentBuildTargetName {
            get {
                return EditorUserBuildSettings.activeBuildTarget.ToString();
            }
        }

        public static string CreateValidBundleIdentifier(string companyName, string productName, string prefix = "com") {
            string template = prefix + ".{0}.{1}";
            return string.Format(
                template,
                SanitizeStringForUseAsBundleIdentifier(companyName),
                SanitizeStringForUseAsBundleIdentifier(productName)
            );
        }

        public static string SanitizeStringForUseAsBundleIdentifier(string name) {
            // write out the first character if it is a digit
            // Why: https://developer.android.com/guide/topics/manifest/manifest-element.html
            // ...individual package name parts may only start with letters.
            string[] ones = new string[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            if (Char.IsNumber(name[0])) {
                int i = (int)Char.GetNumericValue(name[0]);
                if (name.Length > 1) {
                    name = ones[i] + name.Substring(1);
                } else {
                    name = ones[i];
                }
            }
            // lowercase
            name = name.ToLower();
            string invalidChars = System.Text.RegularExpressions.Regex.Escape((new string(System.IO.Path.GetInvalidFileNameChars())) + "_ ");
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "");
        }
    }
}
#endif
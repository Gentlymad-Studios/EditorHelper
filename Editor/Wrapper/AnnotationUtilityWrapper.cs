using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace EditorHelper {
    public static class AnnotationUtilityWrapper {
        private static Assembly assembly;
        private static Type type;
        private static MethodInfo getAnnotations;
        private static MethodInfo setGizmoEnabled;
        private static MethodInfo setIconEnabled;

        static AnnotationUtilityWrapper() {
            assembly = Assembly.GetAssembly(typeof(Editor));
            type = assembly.GetType("UnityEditor.AnnotationUtility");
            getAnnotations = type.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic);
            setGizmoEnabled = type.GetMethod("SetGizmoEnabled", BindingFlags.Static | BindingFlags.NonPublic);
            setIconEnabled = type.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);
        }

        /// <summary>
        /// ToggleGizmo by ClassID -> https://docs.unity3d.com/Manual/ClassIDReference.html
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="classId">use -1 to toggle all</param>
        public static void ToggleGizmo(bool enabled, int classId) {
            int val = enabled ? 1 : 0;
            if (type != null) {
                object annotations = getAnnotations.Invoke(null, null);
                foreach (object annotation in (IEnumerable)annotations) {
                    Type annotationType = annotation.GetType();
                    FieldInfo classIdField = annotationType.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
                    FieldInfo scriptClassField = annotationType.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);
                    if (classIdField != null && scriptClassField != null) {
                        int id = (int)classIdField.GetValue(annotation);
                        string scriptClass = (string)scriptClassField.GetValue(annotation);
                        if (classId == id || classId == -1) {
                            setGizmoEnabled.Invoke(null, new object[] { id, scriptClass, val, false });
                            setIconEnabled.Invoke(null, new object[] { id, scriptClass, val });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Try to get the state of the Gizmo with the given ClassID -> https://docs.unity3d.com/Manual/ClassIDReference.html
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="gizmoEnabled"></param>
        /// <param name="iconEnabled"></param>
        /// <returns></returns>
        public static bool TryGetGizmoToggleState(int classId, out bool gizmoEnabled, out bool iconEnabled) {
            if (type != null) {
                object annotations = getAnnotations.Invoke(null, null);
                foreach (object annotation in (IEnumerable)annotations) {
                    Type annotationType = annotation.GetType();
                    FieldInfo classIdField = annotationType.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
                    FieldInfo scriptClassField = annotationType.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);
                    FieldInfo iconEnabledField = annotationType.GetField("iconEnabled", BindingFlags.Public | BindingFlags.Instance);
                    FieldInfo gizmoEnabledField = annotationType.GetField("gizmoEnabled", BindingFlags.Public | BindingFlags.Instance);
                    if (classIdField != null && scriptClassField != null) {
                        int id = (int)classIdField.GetValue(annotation);
                        if (classId == id) {
                            iconEnabled = ((int)iconEnabledField.GetValue(annotation)) != 0;
                            gizmoEnabled = ((int)gizmoEnabledField.GetValue(annotation)) != 0;
                            return true;
                        }
                    }
                }
            }

            iconEnabled = false;
            gizmoEnabled = false;
            return false;
        }

        /// <summary>
        /// ToggleGizmo by ClassID -> https://docs.unity3d.com/Manual/ClassIDReference.html
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="className"></param>
        public static void ToggleGizmo(bool enabled, string className) {
            int val = enabled ? 1 : 0;
            if (type != null) {
                object annotations = getAnnotations.Invoke(null, null);
                foreach (object annotation in (IEnumerable)annotations) {
                    Type annotationType = annotation.GetType();
                    FieldInfo classIdField = annotationType.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
                    FieldInfo scriptClassField = annotationType.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);
                    if (classIdField != null && scriptClassField != null) {
                        int id = (int)classIdField.GetValue(annotation);
                        string scriptClass = (string)scriptClassField.GetValue(annotation);
                        if (className == scriptClass) {
                            setGizmoEnabled.Invoke(null, new object[] { id, scriptClass, val, false });
                            setIconEnabled.Invoke(null, new object[] { id, scriptClass, val });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Try to get the state of the Gizmo with the given ClassID -> https://docs.unity3d.com/Manual/ClassIDReference.html
        /// </summary>
        /// <param name="className"></param>
        /// <param name="gizmoEnabled"></param>
        /// <param name="iconEnabled"></param>
        /// <returns></returns>
        public static bool TryGetGizmoToggleState(string className, out bool gizmoEnabled, out bool iconEnabled) {
            if (type != null) {
                object annotations = getAnnotations.Invoke(null, null);
                foreach (object annotation in (IEnumerable)annotations) {
                    Type annotationType = annotation.GetType();
                    FieldInfo scriptClassField = annotationType.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);
                    FieldInfo iconEnabledField = annotationType.GetField("iconEnabled", BindingFlags.Public | BindingFlags.Instance);
                    FieldInfo gizmoEnabledField = annotationType.GetField("gizmoEnabled", BindingFlags.Public | BindingFlags.Instance);
                    if (scriptClassField != null) {
                        string scriptClass = (string)scriptClassField.GetValue(annotation);
                        if (className == scriptClass) {
                            iconEnabled = ((int)iconEnabledField.GetValue(annotation)) != 0;
                            gizmoEnabled = ((int)gizmoEnabledField.GetValue(annotation)) != 0;
                            return true;
                        }
                    }
                }
            }

            iconEnabled = false;
            gizmoEnabled = false;
            return false;
        }
    }
}
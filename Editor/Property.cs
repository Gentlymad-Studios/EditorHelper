using System;
using System.Reflection;
using UnityEditor;

namespace EditorHelper {
    public static class Property {
        private static MethodInfo internal_GetFieldInfoAndStaticTypeFromProperty;

        public static FieldInfo GetFieldInfoAndStaticType(SerializedProperty prop, out Type type) {
            if (internal_GetFieldInfoAndStaticTypeFromProperty == null) {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                    foreach (Type t in assembly.GetTypes()) {
                        if (t.Name == "ScriptAttributeUtility") {
                            internal_GetFieldInfoAndStaticTypeFromProperty = t.GetMethod("GetFieldInfoAndStaticTypeFromProperty", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                            break;
                        }
                    }
                    if (internal_GetFieldInfoAndStaticTypeFromProperty != null)
                        break;
                }
            }
            object[] p = new object[] { prop, null };
            FieldInfo fieldInfo = internal_GetFieldInfoAndStaticTypeFromProperty.Invoke(null, p) as FieldInfo;
            type = p[1] as Type;
            return fieldInfo;
        }

        public static T GetCustomAttributeFromProperty<T>(SerializedProperty prop) where T : System.Attribute {
            FieldInfo info = GetFieldInfoAndStaticType(prop, out _);
            return info.GetCustomAttribute<T>();
        }
    }
}
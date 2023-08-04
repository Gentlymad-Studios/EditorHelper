//powerful class, allows to detect intersection with mesh, without requiring any collider, etc
//Works in editor only
//
// Main Author https://gist.github.com/MattRix
// Igor Aherne improved it to include object picking as well   facebook.com/igor.aherne
//https://github.com/MattRix/UnityDecompiled/blob/master/UnityEditor/UnityEditor/HandleUtility.cs
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorHelper {
    public static class HandleUtilityWrapper {
        static Type type_HandleUtility;
        static MethodInfo meth_IntersectRayMesh;

        static HandleUtilityWrapper() {
            Type[] editorTypes = typeof(Editor).Assembly.GetTypes();

            type_HandleUtility = editorTypes.FirstOrDefault(t => t.Name == "HandleUtility");
            meth_IntersectRayMesh = type_HandleUtility.GetMethod("IntersectRayMesh", BindingFlags.Static | BindingFlags.NonPublic);
        }

        //get a point from interected with any meshes in scene, based on mouse position.
        //WE DON'T NOT NEED to have to have colliders ;)
        //usually used in conjunction with  PickGameObject()
        public static bool IntersectRayMesh(Ray ray, MeshFilter meshFilter, out RaycastHit hit) {
            return IntersectRayMesh(ray, meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix, out hit);
        }

        //get a point from interected with any meshes in scene, based on mouse position.
        //WE DON'T NOT NEED to have to have colliders ;)
        //usually used in conjunction with  PickGameObject()
        public static bool IntersectRayMesh(Ray ray, Mesh mesh, Matrix4x4 matrix, out RaycastHit hit) {
            object[] parameters = new object[] { ray, mesh, matrix, null };
            bool result = (bool)meth_IntersectRayMesh.Invoke(null, parameters);
            hit = (RaycastHit)parameters[3];
            return result;
        }
    }
}
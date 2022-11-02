#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace EditorHelper {
    public static class Primitive {
        private static Dictionary<PrimitiveType, Mesh> primitiveMeshes = new Dictionary<PrimitiveType, Mesh>();

        public static Mesh GetPrimitiveMesh(PrimitiveType type) {
            if (!Primitive.primitiveMeshes.ContainsKey(type)) {
                Primitive.CreatePrimitiveMesh(type);
            }

            return Primitive.primitiveMeshes[type];
        }

        private static Mesh CreatePrimitiveMesh(PrimitiveType type) {
            GameObject gameObject = GameObject.CreatePrimitive(type);
            Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            GameObject.DestroyImmediate(gameObject);

            Primitive.primitiveMeshes[type] = mesh;
            return mesh;
        }
    }
}
#endif
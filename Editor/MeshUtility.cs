using UnityEngine;

namespace EditorHelper {
    public static class MeshUtility {
        /// <summary>
        /// Get Normal for the given hit
        /// </summary>
        /// <param name="meshFilter"></param>
        /// <param name="hit"></param>
        /// <param name="interpolate"></param>
        /// <returns></returns>
		public static Vector3 GetTriangleNormal(MeshFilter meshFilter, RaycastHit hit, bool interpolate = false) {
            Mesh mesh = meshFilter.sharedMesh;
            Transform transform = meshFilter.transform;

            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;

            Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
            Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
            Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];

            Vector3 normal = n0 + n1 + n2;
            normal = transform.TransformDirection(normal);

            if (interpolate) {
                Vector3 baryCenter = hit.barycentricCoordinate;
                normal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
            }

            normal.Normalize();

            return normal;
        }

        /// <summary>
        /// Get Normal for the given hitobject
        /// </summary>
        /// <param name="hitObject"></param>
        /// <param name="interpolate"></param>
        /// <returns></returns>
        public static Vector3 GetTriangleNormal(HitObjects hitObject, bool interpolate = false) {
            return GetTriangleNormal(hitObject.meshFilter, hitObject.hit, interpolate);
        }

        /// <summary>
        /// Get SubMeshIndex for the given triangle
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="triangleIndex"></param>
        /// <returns></returns>
        public static int GetSubMeshIndex(Mesh mesh, int triangleIndex) {
            int triangleCounter = 0;
            for (int i = 0; i < mesh.subMeshCount; i++) {
                int indexCount = mesh.GetSubMesh(i).indexCount;
                triangleCounter += indexCount / 3;
                if (triangleIndex < triangleCounter) {
                    return i;
                }
            }

            return 0;
        }
    }
}
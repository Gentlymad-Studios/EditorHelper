using System.Collections.Generic;
using UnityEngine;

namespace EditorHelper {
    public class MeshDrawer {
        /// <summary>
		/// Draw Mesh Visual with the given transform
		/// </summary>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		public static void DrawMesh(MeshVisual meshVisual, Vector3 position, Quaternion rotation) {
            if (meshVisual != null) {
                List<MeshRenderBundle> meshRenderBundle = meshVisual.meshRenderBundle;
                for (int i = 0; i < meshRenderBundle.Count; i++) {
                    MeshRenderBundle mrb = meshRenderBundle[i];

                    Quaternion rot = rotation * mrb.rotationOffset;
                    Vector3 pos = position + rotation * mrb.positionOffset;

                    Matrix4x4 matrix = Matrix4x4.TRS(pos, rot, mrb.scaleOffset);
                    Graphics.DrawMesh(mrb.mesh, matrix, mrb.material, 0, Camera.current, mrb.submeshIndex);
                }
            }
        }
    }

    public class MeshVisual {
        public List<MeshRenderBundle> meshRenderBundle;
        GameObject asset;

        public MeshVisual(GameObject asset) {
            this.asset = asset;

            meshRenderBundle = new List<MeshRenderBundle>();
            MeshRenderer[] meshRenderer = asset.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < meshRenderer.Length; i++) {
                Mesh mesh = meshRenderer[i].GetComponent<MeshFilter>().sharedMesh;
                Material[] materials = meshRenderer[i].sharedMaterials;

                for (int j = 0; j < materials.Length; j++) {
                    meshRenderBundle.Add(new MeshRenderBundle(mesh, materials[j], j, meshRenderer[i].transform.position, meshRenderer[i].transform.rotation, meshRenderer[i].transform.lossyScale));
                }
            }
        }
    }

    public class MeshRenderBundle {
        public Mesh mesh;
        public Material material;
        public int submeshIndex;
        public Vector3 positionOffset;
        public Quaternion rotationOffset;
        public Vector3 scaleOffset;

        public MeshRenderBundle(Mesh mesh, Material material, int submeshIndex, Vector3 positionOffset, Quaternion rotationOffset, Vector3 scaleOffset) {
            this.mesh = mesh;
            this.material = material;
            this.submeshIndex = submeshIndex;
            this.positionOffset = positionOffset;
            this.rotationOffset = rotationOffset;
            this.scaleOffset = scaleOffset;
        }
    }
}

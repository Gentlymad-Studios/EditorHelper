using System.Collections.Generic;
using UnityEngine;

namespace EditorHelper {
    public class MeshInstance {
        private Mesh mesh;
        private Material material;
        private MaterialPropertyBlock[] matBlocks;
        private List<Matrix4x4>[] transforms;

        public MeshInstance(Mesh mesh, Material material, MaterialPropertyBlock[] matBlocks) {
            this.mesh = mesh;
            this.material = material;
            this.matBlocks = matBlocks;
        }

        /// <summary>
        /// Update Transforms with the given meshes
        /// </summary>
        /// <param name="data"></param>
        public void UpdateTransforms(IMeshInstanceData[] data) {
            if (transforms == null) {
                transforms = new List<Matrix4x4>[matBlocks.Length];
            }

            for (int i = 0; i < transforms.Length; i++) {
                if (transforms[i] == null) {
                    transforms[i] = new List<Matrix4x4>();
                } else {
                    transforms[i].Clear();
                }
            }

            for (int i = 0; i < data.Length; i++) {
                int index = data[i].InstanceIndex;
                transforms[index].Add(data[i].TransformMatrix);
            }
        }

        /// <summary>
        /// Render all Meshes to the given camera
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="data"></param>
        public void RenderMesh(Camera cam, IMeshInstanceData[] data) {
            if (transforms == null) {
                UpdateTransforms(data);
            }

            for (int i = 0; i < matBlocks.Length; i++) {
                if (transforms[i].Count == 0) {
                    continue;
                }

                RenderParams rp = new RenderParams(material);
                rp.matProps = matBlocks[i];
                rp.camera = cam;
                rp.worldBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(100, 100, 100));

                Graphics.RenderMeshInstanced(rp, mesh, 0, transforms[i]);
            }
        }
    }

    public interface IMeshInstanceData {
        public int InstanceIndex {
            get;
        }

        public Matrix4x4 TransformMatrix {
            get;
        }
    }
}
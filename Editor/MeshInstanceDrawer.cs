using System.Collections.Generic;
using UnityEngine;

namespace EditorHelper {
    /// <summary>
    /// MeshInstance for a single mesh with an single material, but multiple MaterialBlocks
    /// </summary>
    public class SingleMeshInstance {
        private Mesh mesh;
        private Material material;
        private MaterialPropertyBlock[] matBlocks;
        private List<Matrix4x4>[] transforms;

        public SingleMeshInstance(Mesh mesh, Material material, MaterialPropertyBlock[] matBlocks) {
            this.mesh = mesh;
            this.material = material;
            this.matBlocks = matBlocks;
        }

        /// <summary>
        /// Update Transforms with the given data, index of the IMeshInstanceData will be index of the MaterialPropertyBlocks
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
                if (index > 0 && index < transforms.Length) {
                    transforms[index].Add(data[i].TransformMatrix);
                }
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


    /// <summary>
    /// MeshInstance for a multiple meshes + material
    /// </summary>
    public class MultiMeshInstance {
        public enum MultiMeshHandling {
            EqualMeshesAndMaterials,
            MultipleMeshesWithSingleMaterial,
            SingleMeshWithMultipleMaterials
        }

        private Mesh[] meshes;
        private Material[] materials;
        private List<Matrix4x4>[] transforms;
        private MultiMeshHandling handling;

        /// <summary>
        /// Create an MultiMeshInstance, Meshes and Materials should be the same count
        /// </summary>
        /// <param name="meshes"></param>
        /// <param name="materials"></param>
        public MultiMeshInstance(Mesh[] meshes, Material[] materials, MultiMeshHandling handling) {
            if (meshes.Length != materials.Length && handling == MultiMeshHandling.EqualMeshesAndMaterials) {
                Debug.LogError("[MultiMeshInstance] You need to provide an equal count of Meshes and Materials.");
                return;
            }

            this.meshes = meshes;
            this.materials = materials;
            this.handling = handling;
        }


        /// <summary>
        /// Update Transforms with the given data, index of the IMeshInstanceData will be index of the Meshes
        /// </summary>
        /// <param name="data"></param>
        public void UpdateTransforms(IMeshInstanceData[] data) {
            if (transforms == null) {
                transforms = new List<Matrix4x4>[meshes.Length];
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
                if (index > 0 && index < transforms.Length) {
                    transforms[index].Add(data[i].TransformMatrix);
                }
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

            switch (handling) {
                case MultiMeshHandling.EqualMeshesAndMaterials:
                    for (int i = 0; i < meshes.Length; i++) {
                        if (transforms[i].Count == 0) {
                            continue;
                        }

                        RenderParams rp = new RenderParams(materials[i]);
                        rp.camera = cam;
                        rp.worldBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(100, 100, 100));

                        Graphics.RenderMeshInstanced(rp, meshes[i], 0, transforms[i]);
                    }
                    break;

                case MultiMeshHandling.MultipleMeshesWithSingleMaterial:
                    for (int i = 0; i < meshes.Length; i++) {
                        if (transforms[i].Count == 0) {
                            continue;
                        }

                        RenderParams rp = new RenderParams(materials[0]);
                        rp.camera = cam;
                        rp.worldBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(100, 100, 100));

                        Graphics.RenderMeshInstanced(rp, meshes[i], 0, transforms[i]);
                    }
                    break;

                case MultiMeshHandling.SingleMeshWithMultipleMaterials:
                    for (int i = 0; i < materials.Length; i++) {
                        if (transforms[i].Count == 0) {
                            continue;
                        }

                        RenderParams rp = new RenderParams(materials[i]);
                        rp.camera = cam;
                        rp.worldBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(100, 100, 100));

                        Graphics.RenderMeshInstanced(rp, meshes[0], 0, transforms[i]);
                    }
                    break;
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
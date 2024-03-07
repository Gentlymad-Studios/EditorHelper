using System;
using System.Collections.Generic;
using UnityEngine;

namespace EditorHelper {
    public class MeshDrawer {
        /// <summary>
		/// Draw Mesh Visual with the given transform
		/// </summary>
        /// <param name="meshVisual"></param>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		/// <param name="scale"></param>
		public static void DrawMesh(MeshVisual meshVisual, Vector3 position, Quaternion rotation, Vector3 scale) {
            DrawMesh(meshVisual, position, rotation, scale, Vector3.zero);
        }

        /// <summary>
        /// Draw Mesh Visual with the given transform
        /// </summary>
        /// <param name="meshVisual"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="parentPosition">Position of an optinal Parent</param>
        /// <param name="propertyBlockOverride">if an propertyblock is given, this will override the internal one</param>
        public static void DrawMesh(MeshVisual meshVisual, Vector3 position, Quaternion rotation, Vector3 scale, Vector3 parentPosition, MaterialPropertyBlock propertyBlockOverride = null) {
            if (meshVisual != null) {
                List<MeshRenderBundle> meshRenderBundle = meshVisual.meshRenderBundle;
                for (int i = 0; i < meshRenderBundle.Count; i++) {
                    MeshRenderBundle mrb = meshRenderBundle[i];

                    Quaternion rot = rotation * mrb.rotationOffset;
                    Vector3 pos = position + rotation * (mrb.positionOffset - parentPosition);
                    Vector3 scl = Vector3.Scale(scale, mrb.scaleOffset);

                    Matrix4x4 matrix = Matrix4x4.TRS(pos, rot, scl);

                    if (propertyBlockOverride == null) {
                        Graphics.DrawMesh(mrb.mesh, matrix, mrb.material, 0, Camera.current, mrb.submeshIndex, mrb.materialPropertyBlock);
                    } else {
                        Graphics.DrawMesh(mrb.mesh, matrix, mrb.material, 0, Camera.current, mrb.submeshIndex, propertyBlockOverride);
                    }
                }
            }
        }


        public class MeshVisual {
            public List<MeshRenderBundle> meshRenderBundle;
            GameObject asset;
            public MaterialPropertyBlock globalMatPropertyBlock;
            Func<GameObject, MaterialPropertyBlock> matPropertyBlockExtration;
            bool includeChildren;

            public MeshVisual(GameObject asset, MaterialPropertyBlock materialPropertyBlock = null, Func<GameObject, MaterialPropertyBlock> matPropertyBlockExtration = null, bool includeChildren = true) {
                this.asset = asset;
                globalMatPropertyBlock = materialPropertyBlock;
                this.matPropertyBlockExtration = matPropertyBlockExtration;
                this.includeChildren = includeChildren;

                meshRenderBundle = new List<MeshRenderBundle>();

                Update();
            }

            public void Update() {
                meshRenderBundle.Clear();

                MeshRenderer[] meshRenderer = new MeshRenderer[1];

                if (includeChildren) {
                    meshRenderer = asset.GetComponentsInChildren<MeshRenderer>();
                } else {
                    asset.TryGetComponent(out meshRenderer[0]);
                }
                for (int i = 0; i < meshRenderer.Length; i++) {
                    if (meshRenderer[i] == null) {
                        continue;
                    }
                    Mesh mesh = meshRenderer[i].GetComponent<MeshFilter>().sharedMesh;
                    Material[] materials = meshRenderer[i].sharedMaterials;

                    if (matPropertyBlockExtration != null) {
                        globalMatPropertyBlock = matPropertyBlockExtration.Invoke(meshRenderer[i].gameObject);
                    }

                    for (int j = 0; j < materials.Length; j++) {
                        meshRenderBundle.Add(new MeshRenderBundle(mesh, materials[j], globalMatPropertyBlock, j, meshRenderer[i].transform.position, meshRenderer[i].transform.rotation, meshRenderer[i].transform.lossyScale));
                    }
                }

                SkinnedMeshRenderer[] skinnedMeshRenderer = new SkinnedMeshRenderer[1];
                if (includeChildren) {
                    skinnedMeshRenderer = asset.GetComponentsInChildren<SkinnedMeshRenderer>();
                } else {
                    asset.TryGetComponent(out skinnedMeshRenderer[0]);
                }
                for (int i = 0; i < skinnedMeshRenderer.Length; i++) {
                    if (skinnedMeshRenderer[i] == null) {
                        continue;
                    }
                    Mesh mesh = skinnedMeshRenderer[i].sharedMesh;
                    Material[] materials = skinnedMeshRenderer[i].sharedMaterials;

                    if (matPropertyBlockExtration != null) {
                        globalMatPropertyBlock = matPropertyBlockExtration.Invoke(skinnedMeshRenderer[i].gameObject);
                    }

                    for (int j = 0; j < materials.Length; j++) {
                        meshRenderBundle.Add(new MeshRenderBundle(mesh, materials[j], globalMatPropertyBlock, j, skinnedMeshRenderer[i].transform.position, skinnedMeshRenderer[i].transform.rotation, skinnedMeshRenderer[i].transform.lossyScale));
                    }
                }
            }

            public void ToggleKeyword(bool enable, string keyword) {
                for (int i = 0; i < meshRenderBundle.Count; i++) {
                    if (enable) {
                        meshRenderBundle[i].material.EnableKeyword(keyword);
                    } else {
                        meshRenderBundle[i].material.DisableKeyword(keyword);
                    }
                }
            }
        }

        public class MeshRenderBundle {
            public Mesh mesh;
            public Material material;
            public MaterialPropertyBlock materialPropertyBlock;
            public int submeshIndex;
            public Vector3 positionOffset;
            public Quaternion rotationOffset;
            public Vector3 scaleOffset;

            public MeshRenderBundle(Mesh mesh, Material material, MaterialPropertyBlock materialPropertyBlock, int submeshIndex, Vector3 positionOffset, Quaternion rotationOffset, Vector3 scaleOffset) {
                this.mesh = mesh;
                this.material = material;
                this.materialPropertyBlock = materialPropertyBlock;
                this.submeshIndex = submeshIndex;
                this.positionOffset = positionOffset;
                this.rotationOffset = rotationOffset;
                this.scaleOffset = scaleOffset;
            }
        }
    }
}
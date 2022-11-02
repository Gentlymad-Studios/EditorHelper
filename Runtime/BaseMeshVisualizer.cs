#if UNITY_EDITOR
using UnityEngine;

namespace EditorHelper {
    [ExecuteInEditMode]
    public abstract class BaseMeshVisualizer : MonoBehaviour {

        // Default material, used when no material for a mesh could be found
        private static Material defaultMaterial = null;
        private static Material DefaultMaterial {
            get {
                if (defaultMaterial == null) {
                    defaultMaterial = new Material(Shader.Find("Standard"));
                }
                return defaultMaterial;
            }
        }

        // cached temporary material
        protected Material cachedMaterial;

        // cached temporary camera
        protected Camera currentCamera;

        // Camera valid flag
        protected bool isCameraVaild = false;

        /// <summary>
        /// Cache Material or default it to the standard shader
        /// </summary>
        /// <param name="material"></param>
        private void CacheMaterial(Material material) {
            cachedMaterial = material;
            if (cachedMaterial == null) {
                cachedMaterial = DefaultMaterial;
            }
        }

        /// <summary>
        /// Draw the mesh with localToWorldMatrix
        /// </summary>
        /// <param name="material"></param>
        /// <param name="mesh"></param>
        /// <param name="matrix"></param>
        protected virtual void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material) {
            // cache the material
            CacheMaterial(material);
            // draw the mesh
            Graphics.DrawMesh(mesh, matrix, cachedMaterial, gameObject.layer, currentCamera);
        }

        /// <summary>
        /// Draw the mesh with position and rotation
        /// </summary>
        /// <param name="material"></param>
        /// <param name="mesh"></param>
        /// <param name="matrix"></param>
        protected virtual void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material) {
            // cache the material
            CacheMaterial(material);
            // draw the mesh
            Graphics.DrawMesh(mesh, position, rotation, cachedMaterial, gameObject.layer, currentCamera);
        }

        protected virtual void OnEnable() {
            // hook into precull method of scene view camera
            Camera.onPreCull -= DrawInternal;
            Camera.onPreCull += DrawInternal;
        }

        protected virtual void OnDisable() {
            // unsubscribe from pre cull hook
            Camera.onPreCull -= DrawInternal;
        }

        /// <summary>
        /// Function called when the camera in the scene view is drawn
        /// </summary>
        /// <param name="cam"></param>
        private void DrawInternal(Camera cam) {
            currentCamera = cam;
            isCameraVaild = cam && cam.scene.name == null;
            if (isCameraVaild) {
                Draw(cam);
            }
        }

        protected abstract void Draw(Camera cam);
    }
}
#endif
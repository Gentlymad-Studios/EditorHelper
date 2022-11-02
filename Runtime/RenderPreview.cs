#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorHelper {
    /// <summary>
    /// Editor UI class to render a 3D Mesh for the inspector.
    /// Creates a 2D texture to be drawn with EDITORGUI functions.
    /// </summary>
    public class RenderPreview {
        public PreviewRenderUtility prevRender;
        public RenderPreviewSettings settings = new RenderPreviewSettings();
        public Dictionary<int, RenderObject> renderObjects = new Dictionary<int, RenderObject>();
        private int meshInstanceID;

        public class RenderObject {
            public Mesh mesh;
            public Material material;
            public Vector3 localPositionOffset;
        }

        public class RenderPreviewSettings {
            public int gridSize = 2;
            public bool disabled = false;
            public bool updateEveryDraw = true;
            public bool isOrthographic = true;
            public float farClipPlane = 200;
            public float nearClipPlane = 0;
            public float aspect = 1;
            public float orthographicSize = 16;
            public Vector3 worldPosition = new Vector3(0, 100, 0);
            public Quaternion worldRotation = Quaternion.Euler(90, 0, 0);
            public Vector3 objectPosition = Vector3.zero;
        }

        public RenderPreview() {
            prevRender = new PreviewRenderUtility();
            // setup simple lighting
            prevRender.lights[0].intensity = 1f;
            prevRender.lights[0].transform.rotation = Quaternion.Euler(30f, 30f, 0f);
            prevRender.lights[1].intensity = 1f;
            // update camera & settings values
            Update();  
        }

        protected virtual void Update() {
            prevRender.camera.orthographic = settings.isOrthographic;
            prevRender.camera.aspect = settings.aspect;
            prevRender.camera.orthographicSize = settings.orthographicSize;
            prevRender.camera.transform.rotation = settings.worldRotation;
            prevRender.camera.transform.position = settings.worldPosition;
            prevRender.camera.farClipPlane = settings.farClipPlane;
            prevRender.camera.nearClipPlane = settings.nearClipPlane;
        }

        public void AddMeshAndMaterial(Mesh mesh, Material material, Vector3 localPositionOffset) {
            meshInstanceID = mesh.GetInstanceID();
            if (!renderObjects.ContainsKey(meshInstanceID)) {
                renderObjects.Add(meshInstanceID, new RenderObject() { mesh = mesh, material = material, localPositionOffset = localPositionOffset });
            }
            renderObjects[meshInstanceID].mesh.RecalculateBounds();
        }

        public virtual void Draw(Rect r) {
            if (prevRender == null) {
                return;
            }

            if (settings.updateEveryDraw) {
                Update();
            }

            prevRender.BeginPreview(r, GUIStyle.none);

            foreach (var renderObject in renderObjects.Values) {
                prevRender.DrawMesh(renderObject.mesh, settings.objectPosition + renderObject.localPositionOffset, Quaternion.identity, renderObject.material, 0);
            }

            bool fog = RenderSettings.fog;
            Unsupported.SetRenderSettingsUseFogNoDirty(false);
            prevRender.camera.Render();
            Unsupported.SetRenderSettingsUseFogNoDirty(fog);
            Texture texture = prevRender.EndPreview();

            GUI.DrawTexture(r, texture);
        }

        public void Cleanup() {
            prevRender.Cleanup();
        }
    }
}
#endif

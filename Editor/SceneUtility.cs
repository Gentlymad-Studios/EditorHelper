using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EditorHelper {
    public static class SceneUtility {
        static MeshFilter[] _allMeshFilter;
        static GameObject _dragObject;
        static bool _updateMeshFilter = true;
        static bool _updateDragObject = true;

        /// <summary>
        /// Reset the UpdateMeshFilter field
        /// </summary>
        public static void ResetUpdateMeshFilter() {
            _updateMeshFilter = true;
        }

        /// <summary>
        /// Reset the UpdateDragObject field
        /// </summary>
        public static void ResetUpdateDragObject() {
            _updateDragObject = true;
        }

        /// <summary>
		/// Get the Closest Hit to all meshes in the scene
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="hits"></param>
		/// <param name="ignoreHidden"></param>
		/// <param name="force">if true, it will update the cached meshes</param>
		/// <returns>true if success</returns>
		public static bool GetHits(Ray ray, out List<HitObjects> hits, bool ignoreHidden = true, bool force = false) {
            //gather all meshfilter
            if (_updateMeshFilter || force) {
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage == null) {
                    _allMeshFilter = GameObject.FindObjectsOfType<MeshFilter>();
                } else {
                    _allMeshFilter = StageUtility.GetCurrentStageHandle().FindComponentsOfType<MeshFilter>();
                }

                _updateMeshFilter = false;

                List<MeshFilter> tmp = new List<MeshFilter>();
                for (int i = 0; i < _allMeshFilter.Length; i++) {
                    if (_allMeshFilter[i].hideFlags != HideFlags.HideInHierarchy) {
                        tmp.Add(_allMeshFilter[i]);
                    }
                }

                _allMeshFilter = tmp.ToArray();
            }

            hits = new List<HitObjects>();

            for (int i = 0; i < _allMeshFilter.Length; i++) {
                if (HandleUtilityWrapper.IntersectRayMesh(ray, _allMeshFilter[i], out RaycastHit raycastHit)) {
                    hits.Add(new HitObjects(raycastHit, _allMeshFilter[i]));
                }
            }

            if (hits.Count == 0) {
                return false;
            }

            hits = hits.OrderBy(o => o.distance).ToList();

            return true;
        }

        /// <summary>
		/// Get the Dragged Object from Hierarchy
		/// </summary>
		/// <param name="reference"></param>
		/// <param name="force">if true, it will update the cached objects</param>
		/// <returns></returns>
		public static GameObject GetDraggedObject(Object reference, bool force = false) {
            //gather all meshfilter
            if (_updateDragObject || force) {
                _dragObject = null;

                Transform[] transforms;

                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage == null) {
                    transforms = GameObject.FindObjectsOfType<Transform>();
                } else {
                    transforms = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Transform>();
                }

                _updateDragObject = false;

                for (int i = 0; i < transforms.Length; i++) {
                    if (transforms[i].hideFlags == HideFlags.HideInHierarchy && transforms[i].name == reference.name) {
                        _dragObject = transforms[i].gameObject;
                        return _dragObject;
                    }
                }
            }

            return _dragObject;
        }
    }

    public class HitObjects {
        public RaycastHit hit;
        public float distance;
        public MeshFilter meshFilter;

        public HitObjects(RaycastHit hit, MeshFilter meshFilter) {
            this.hit = hit;
            this.meshFilter = meshFilter;
            this.distance = hit.distance;
        }
    }
}
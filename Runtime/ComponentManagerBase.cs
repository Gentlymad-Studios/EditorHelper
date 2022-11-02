#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorHelper {
    [ExecuteInEditMode]
    public abstract class ComponentManagerBase<T> : MonoBehaviour where T:MonoBehaviour {
        // all, unfiltered components
        protected List<T> components = new List<T>();

        // all valid components that passed validate components
        public static List<T> validComponents = new List<T>();

        // cached version of the active scene
        private UnityEngine.SceneManagement.Scene activeScene;

        // list of all rendercomponents that are somehow faulty
        protected UI.FaultyComponentContainer faultyComponents = new UI.FaultyComponentContainer();

        /// <summary>
        /// Called when the hierachy changed
        /// </summary>
        private void OnHierarchyChanged() {
            if (activeScene.isDirty) {
                FindAllComponents();
            }
        }

        protected virtual void OnEnable() {
            // register for scene view
            SceneViewManager.Register(OnSceneGUI);

            // get the active scene
            activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

            // find all components
            FindAllComponents();

            // listen on hierachy changes
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        protected virtual void OnDisable() {
            // unsubscribe from hierarchyChanged trigger
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            // clear lists
            components.Clear();
            validComponents.Clear();
        }

        /// <summary>
        /// Find all components
        /// </summary>
        private void FindAllComponents() {
            components.Clear();
            components.AddRange(FindObjectsOfType<T>());
            ValidateComponents();
        }

        /// <summary>
        /// Validates the components that we found
        /// </summary>
        protected virtual void ValidateComponents() {
            faultyComponents.Cleanup();
            validComponents.Clear();
        }

        /// <summary>
        /// Setup our editor UI styles.
        /// </summary>
        protected virtual void SetupStyles() {
            faultyComponents.SetupStyles();
        }

        /// <summary>
        /// function called when scene GUI is called
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual int OnSceneGUI(int arg) {
            SetupStyles();
            faultyComponents.Draw();
            return 0;
        }
    }
}
#endif
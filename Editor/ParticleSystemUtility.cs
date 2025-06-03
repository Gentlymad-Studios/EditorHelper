using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System;
using UnityEditor;

namespace EditorHelper {
    /// <summary>
    /// Utility class for managing Particle Systems in the Unity Editor.
    /// </summary>
    public static class ParticleSystemUtility {

        /// <summary>
        /// Dictionary to store the simulation coroutines for each ParticleSystem.
        /// </summary>
        private static Dictionary<ParticleSystem, EditorCoroutine> _simulationCoroutinesByParticleSystem;

        /// <summary>
        /// Dictionary to store the auto random seed values for each ParticleSystem.
        /// </summary>
        private static Dictionary<ParticleSystem, bool> _autoRandomSeedByParticleSystem;

        /// <summary>
        /// Plays the Particle System inside the editor without having to select it. (If called during Play Mode, the Particle System will play as usual).
        /// </summary>
        /// <param name="particleSystem">The Particle System to play.</param>
        /// <param name="withChildren">Play all children particles as well.</param>
        /// <param name="effectFinishedEditorEvent">Action called when the system is finished. [Does Not Work Outside Of Unity Editor].</param>
        /// <param name="useDeltaTime">Use Delta Time instead of FullTime, this is needed if you have SubEmitters.</param>
        /// <param name="ignoreLifeTimeMultiplier">Ignores the startLifetimeMultiplier in the caluculation of the totalTime, just use duration.</param>
        public static void PlayInEditor(ParticleSystem particleSystem, bool withChildren, Action effectFinishedEditorEvent, bool useDeltaTime = false, bool ignoreLifeTimeMultiplier = false) {
            // When playing the system in the editor, we have to control each child individually
            ParticleSystem[] particleSystems = withChildren ? particleSystem.GetComponentsInChildren<ParticleSystem>(true) : new[] { particleSystem.GetComponentInChildren<ParticleSystem>(true) };

            int particleCount = particleSystems.Length;
            int finishedParticleCount = 0;

            if (_simulationCoroutinesByParticleSystem == null) {
                // Store a dictionary of systems so that we do not start multiple coroutines
                _simulationCoroutinesByParticleSystem = new Dictionary<ParticleSystem, EditorCoroutine>();
            }

            if (_autoRandomSeedByParticleSystem == null) {
                // Store a dictionary of systems so that we can reset the random seed value after playing
                _autoRandomSeedByParticleSystem = new Dictionary<ParticleSystem, bool>();
            }

            foreach (ParticleSystem childParticleSystem in particleSystems) {
                if (!childParticleSystem.gameObject.activeInHierarchy) {
                    continue;
                }

                if (_simulationCoroutinesByParticleSystem.TryGetValue(childParticleSystem, out EditorCoroutine editorCoroutine)) {
                    // If the system is currently playing or the coroutine was interrupted previously, make sure to stop the system and remove the dictionary item
                    childParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                    EditorCoroutineUtility.StopCoroutine(editorCoroutine);
                    _simulationCoroutinesByParticleSystem.Remove(childParticleSystem);

                    if (_autoRandomSeedByParticleSystem.TryGetValue(childParticleSystem, out bool isOn)) {
                        // If the system is currently playing or the coroutine was interrupted previously, make sure to set the autoRandomSeed value to the original value
                        childParticleSystem.useAutoRandomSeed = isOn;
                        _autoRandomSeedByParticleSystem.Remove(childParticleSystem);
                    }
                }

                void OnEffectFinished() {
                    finishedParticleCount++;
                    // If we are playing children, only invoke the master callback once all systems finish
                    if (finishedParticleCount == particleCount) {
                        effectFinishedEditorEvent?.Invoke();
                    }
                }

                EditorCoroutine currentEditorCoroutine = EditorCoroutineUtility.StartCoroutine(PlaySingleParticleSystemInEditor(childParticleSystem, OnEffectFinished, useDeltaTime, ignoreLifeTimeMultiplier), childParticleSystem);
                _simulationCoroutinesByParticleSystem.Add(childParticleSystem, currentEditorCoroutine);
            }
        }

        /// <summary>
        /// Stops playing the Particle System in the editor.
        /// </summary>
        /// <param name="particleSystem">The Particle System to stop.</param>
        /// <param name="withChildren">Stop all children particles as well.</param>
        public static void StopPlayingInEditor(ParticleSystem particleSystem, bool withChildren) {
            ParticleSystem[] particleSystems = withChildren ? particleSystem.GetComponentsInChildren<ParticleSystem>(true) : new[] { particleSystem.GetComponentInChildren<ParticleSystem>(true) };

            foreach (ParticleSystem childParticleSystem in particleSystems) {
                if (!childParticleSystem.gameObject.activeInHierarchy) {
                    continue;
                }

                if (_simulationCoroutinesByParticleSystem == null) {
                    // Store a dictionary of systems so that we do not start multiple coroutines
                    _simulationCoroutinesByParticleSystem = new Dictionary<ParticleSystem, EditorCoroutine>();
                }

                if (_simulationCoroutinesByParticleSystem.TryGetValue(childParticleSystem, out EditorCoroutine editorCoroutine)) {
                    // If the system is currently playing or the coroutine was interrupted previously, make sure to stop the system and remove the dictionary item
                    childParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                    EditorCoroutineUtility.StopCoroutine(editorCoroutine);
                    _simulationCoroutinesByParticleSystem.Remove(childParticleSystem);

                    if (_autoRandomSeedByParticleSystem.TryGetValue(childParticleSystem, out bool isOn)) {
                        // If the system is currently playing or the coroutine was interrupted previously, make sure to set the autoRandomSeed value to the original value
                        childParticleSystem.useAutoRandomSeed = isOn;
                        _autoRandomSeedByParticleSystem.Remove(childParticleSystem);
                    }
                }
            }
        }

        /// <summary>
        /// Plays a single Particle System in the editor.
        /// </summary>
        /// <param name="particleSystem">The Particle System to play.</param>
        /// <param name="effectFinished">Action called when the system is finished.</param>
        /// <returns>An IEnumerator for the coroutine.</returns>
        private static IEnumerator PlaySingleParticleSystemInEditor(ParticleSystem particleSystem, Action effectFinished = null, bool useDeltaTime = false, bool ignoreLifeTimeMultiplier = false) {
            bool isAutoRandomSeed = particleSystem.useAutoRandomSeed;
            _autoRandomSeedByParticleSystem.Add(particleSystem, isAutoRandomSeed);
            particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

            yield return null;

            ParticleSystem.MainModule mainSystem = particleSystem.main;
            float totalTime = ignoreLifeTimeMultiplier ? mainSystem.duration : mainSystem.startLifetimeMultiplier * mainSystem.duration;
            particleSystem.randomSeed = (uint)UnityEngine.Random.Range(0, int.MaxValue);
            float curTime = 0.0f;

            yield return null;

            // Update system to play state
            particleSystem.Play(false);

            // Time.deltaTime does not work in editor so we use the Unity Time Since Startup
            float initialTime = (float)UnityEditor.EditorApplication.timeSinceStartup;
            float lastTime = initialTime;

            while (curTime < totalTime && particleSystem != null && particleSystem.gameObject.activeInHierarchy) {
                if (useDeltaTime) {
                    float currentTime = (float)EditorApplication.timeSinceStartup;
                    float deltaTime = currentTime - lastTime;
                    lastTime = currentTime;

                    particleSystem.Simulate(deltaTime, false, false, false);
                } else {
                    // In order to play in edit mode, both Simulate and time have to be changed
                    particleSystem.Simulate(curTime, false, true);
                    particleSystem.time = curTime;
                }

                // Track delta between start and current Unity Time Since Startup
                curTime = (float)UnityEditor.EditorApplication.timeSinceStartup - initialTime;

                yield return null;
            }

            if (particleSystem != null) {
                // Update system to stop state
                particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

                //comment this out because of errors when using shorter totalTimes
                //yield return null;

                if (isAutoRandomSeed) {
                    // Restore random seed value back to the original value
                    particleSystem.randomSeed = 0;
                    particleSystem.useAutoRandomSeed = isAutoRandomSeed;
                }

                // Remove item from dictionaries when system finishes
                _autoRandomSeedByParticleSystem.Remove(particleSystem);
                _simulationCoroutinesByParticleSystem.Remove(particleSystem);

                yield return null;

                effectFinished?.Invoke();
            }
        }
    }
}

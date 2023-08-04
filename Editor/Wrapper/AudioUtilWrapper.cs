using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorHelper {
    public static class AudioUtilWrapper {
        private static MethodInfo playClipMethod = null;
        private static MethodInfo stopClipsMethod = null;

        private static void SetupAudioPlayback() {
            if (playClipMethod != null && stopClipsMethod != null) {
                return;
            }

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            playClipMethod = audioUtilClass.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(AudioClip), typeof(int), typeof(bool) }, null);

            stopClipsMethod = audioUtilClass.GetMethod("StopAllPreviewClips", BindingFlags.Static | BindingFlags.Public, null, new Type[] { }, null);
        }

        public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false) {
            SetupAudioPlayback();

            playClipMethod.Invoke(null, new object[] { clip, startSample, loop });
        }

        public static void StopAllClips() {
            SetupAudioPlayback();

            stopClipsMethod.Invoke(null, new object[] { });
        }
    }
}
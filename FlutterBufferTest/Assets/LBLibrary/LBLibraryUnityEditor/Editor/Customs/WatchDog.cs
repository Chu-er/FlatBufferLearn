using System;
using UnityEditor;
using UnityEngine;

namespace LIBII.CustomEditor
{
    [InitializeOnLoad]
    public class WatchDog
    {
        static WatchDog()
        {
            Initialize();
        }

        public static Action onCompileFinished;
        private static bool _preIsCompiling = false;
        
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            void Watcher()
            {
                if (EditorApplication.isCompiling != _preIsCompiling)
                {
                    _preIsCompiling = EditorApplication.isCompiling;
                    onCompileFinished?.Invoke();
                }
            }

            EditorApplication.update -= Watcher;
            EditorApplication.update += Watcher;
        }
    }
}
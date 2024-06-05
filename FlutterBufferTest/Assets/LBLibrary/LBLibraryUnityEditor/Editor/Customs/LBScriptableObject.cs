using UnityEditor;
using UnityEngine;

namespace LIBII.CustomEditor
{
    [UnityEditor.CustomEditor(typeof(ScriptableObject), true)]
    [CanEditMultipleObjects]
    public class LBScriptableObject : LBInspector
    {
    }
}
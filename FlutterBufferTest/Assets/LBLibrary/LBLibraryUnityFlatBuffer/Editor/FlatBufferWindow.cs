using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FlatBufferWindow : EditorWindow
{
    [MenuItem("LBTools/FlatBuffer")]
    static void GetWindow()
    {
        EditorWindow.GetWindow<FlatBufferWindow>().Show();
    }

    FlatBufferCompileOption option = new FlatBufferCompileOption();

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            option.inputPath = EditorGUILayout.TextField("引用路径:", option.inputPath);

            if (GUILayout.Button("+", GUILayout.Width(32)))
            {
                option.inputPath = EditorUtility.OpenFolderPanel("选择引用目录", "", "");
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        {
            option.files = EditorGUILayout.TextField("文件路径:", option.files);

            if (GUILayout.Button("+", GUILayout.Width(32)))
            {
                option.files = EditorUtility.OpenFilePanel("选择文件", "", "fbs");
            }

            EditorGUILayout.EndHorizontal();
        }

        option.isMutable = EditorGUILayout.Toggle("启用Mutable", option.isMutable);
        option.isObjectAPI = EditorGUILayout.Toggle("启用ObjectAPI", option.isObjectAPI);

        EditorGUILayout.BeginHorizontal();
        {
            option.outputPath = EditorGUILayout.TextField("导出路径:", option.outputPath);

            if (GUILayout.Button("+", GUILayout.Width(32)))
            {
                option.outputPath = EditorUtility.OpenFolderPanel("选择导出目录", "", "");
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(5);
        if (GUILayout.Button("Compile"))
        {
            FlatBufferCompiler.Compile(option);
        }
    }
}
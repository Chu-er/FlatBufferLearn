using System;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace LBLibraryUnityFlatBuffer.Editor
{
    [ScriptedImporter(21300000, "fbs", AllowCaching = true)]
    public class FlatBufferSchemaImporter : ScriptedImporter
    {
        [SerializeField] private bool isMutable;

        [SerializeField] private bool isObjectAPI;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var option = new FlatBufferCompileOption();
            option.files = ctx.assetPath;
            option.outputPath = Directory.GetParent(ctx.assetPath).FullName;
            option.isMutable = isMutable;
            option.isObjectAPI = isObjectAPI;

            var outputPath = FlatBufferCompiler.Compile(option, false);

            var texts = File.ReadAllText(ctx.assetPath);
            var asset = ScriptableObject.CreateInstance<FlatBufferAsset>();
            asset.content = texts;

            ctx.AddObjectToAsset(ctx.assetPath, asset);

            ctx.SetMainObject(asset);

            
            //AssetDatabase.Refresh();

            /*if (!string.IsNullOrEmpty(outputPath)) 
            {
                var path = "Assets" + outputPath.Replace(Application.dataPath.Replace("/", "\\"), "");
                Debug.Log($"reimporting {path}");
                AssetDatabase.ImportAsset(path, 
                    ImportAssetOptions.ForceUpdate
                    |ImportAssetOptions.ImportRecursive
                    |ImportAssetOptions.DontDownloadFromCacheServer
                );
            }*/
        }
    }
}
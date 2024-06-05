using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public struct FlatBufferCompileOption
{
    public bool isMutable;
    public bool isObjectAPI;
    public string outputPath;
    public string inputPath;

    public string files;
    public string flatcPath;
}

public class FlatBufferCompiler
{
    public static string Compile(FlatBufferCompileOption options, bool openOutputFolder = true)
    {
        string flatcPath;

        if (options.flatcPath is null)
        {
            string os;
            string name;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                os = "windows";
                name = "flatc.exe";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                os = "macos";
                name = "flatc";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                os = "linux";
                name = "flatc";
            }
            else
            {
                throw new InvalidOperationException("FlatSharp compiler is not supported on this operating system.");
            }

            string currentProcess = typeof(FlatBufferCompiler).Assembly.Location;
            string currentDirectory = Path.GetDirectoryName(currentProcess)!;
            flatcPath = Path.Combine(currentDirectory, "flatc", os, name);


            if (!File.Exists(flatcPath))
            {
                currentDirectory = Path.GetFullPath("Packages/com.libii.flatbuffer/Editor/ext/");
                flatcPath = Path.Combine(currentDirectory, "flatc", os, name);
            }

            if (!File.Exists(flatcPath))
            {
                currentDirectory = Application.dataPath + "/LBLibrary/LBLibraryUnityFlatBuffer/Editor/ext/";
                flatcPath = Path.Combine(currentDirectory, "flatc", os, name);
            }
        }
        else
        {
            flatcPath = options.flatcPath;
        }

        string outputDir = options.outputPath;
        if (!string.IsNullOrEmpty(outputDir))
            Directory.CreateDirectory(outputDir);

        if (string.IsNullOrEmpty(outputDir))
        {
            outputDir = "./";
        }

        if (!outputDir.EndsWith("/"))
        {
            outputDir += "/";
        }

        outputDir = outputDir.Replace("/", "\\");

        using var p = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = flatcPath.Replace("\\", "/"),
            }
        };

        var args = new List<string>
        {
            "--csharp",
            options.isObjectAPI ? "--gen-object-api" : "",
            options.isMutable ? "--gen-mutable" : "",
        };

        if (!string.IsNullOrEmpty(options.outputPath))
        {
            args.AddRange(new[]
            {
                "-o",
                outputDir,
            });
        }

        if (!string.IsNullOrEmpty(options.inputPath))
        {
            // One or more includes directory has been specified
            foreach (var includePath in options.inputPath.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                args.AddRange(new[]
                {
                    "-I",
                    new DirectoryInfo(includePath).FullName,
                });
            }
        }

        if (!string.IsNullOrEmpty(options.files))
        {
            var files = options.files.Split(";", StringSplitOptions.RemoveEmptyEntries);
            foreach (string file in files)
            {
                var info = new FileInfo(file);
                args.Add(info.FullName.Replace("\\", "/"));
            }
        }


        var lines = new StringBuilder();

        foreach (var arg in args)
        {
            if (string.IsNullOrEmpty(arg))
            {
                continue;
            }

            lines.Append(arg + " ");
            p.StartInfo.ArgumentList.Add(arg);
        }

        try
        {
            p.EnableRaisingEvents = true;
            lines.AppendLine("");

            void OnDataReceived(object sender, DataReceivedEventArgs args)
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    lines.AppendLine(args.Data);
                }
            }

            p.OutputDataReceived += OnDataReceived;
            p.ErrorDataReceived += OnDataReceived;


            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();

            if (p.ExitCode == 0)
            {
                Debug.Log(lines);
                if (openOutputFolder)
                    EditorUtility.OpenWithDefaultApp(outputDir);
                return outputDir;
            }
            else
            {
                throw new InvalidDataException(
                    lines +
                    "\nUnknown error when invoking flatc. Process exited with error, but didn't write any errors.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);

            return string.Empty;
        }
    }
}
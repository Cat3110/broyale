using System;
using System.Collections.Generic;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEditorInternal;
using UnityEngine;

using Debug = UnityEngine.Debug;

public class ScriptBatch 
{
    private static string[] clientScenes = { "Assets/StaticAssets/Scenes/Lobby.unity", "Assets/StaticAssets/Scenes/Client.unity" };
    
    [MenuItem("Build/Client/Windows(64)(il2cpp)")]
    public static void BuildWindowsClient()
    {
        var path = "../bin/client/x86_64/";
        var name = "broyal.exe";
        var target = BuildTarget.StandaloneWindows64;
        var option = BuildOptions.None;

        var scriptingImplementation = ScriptingImplementation.IL2CPP;

        BuildReport report = BuildGame(clientScenes, path, name, target, option, "1", scriptingImplementation);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
        
        //
        // // Copy a file from the project folder to the build folder, alongside the built game.
        // FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");
        //
        // // Run the game (Process class from System.Diagnostics).
        // Process proc = new Process();
        // proc.StartInfo.FileName = path + "/BuiltGame.exe";
        // proc.Start();
    }
   
    [MenuItem("Build/Client/Android(64)(il2cpp)")]
    public static void BuildAndroidClient()
    {
        AddressableAssetSettings.CleanPlayerContent();
        AddressableAssetSettings.BuildPlayerContent();
        
        var path = "../bin/client/android/";
        var name = "broyal-client.apk";
        var target = BuildTarget.Android;
        var option = BuildOptions.None;

        var scriptingImplementation = ScriptingImplementation.IL2CPP;

        BuildReport report = BuildGame(clientScenes, path, name, target, option, "1", scriptingImplementation);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
        
        //
        // // Copy a file from the project folder to the build folder, alongside the built game.
        // FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");
        //
        // // Run the game (Process class from System.Diagnostics).
        // Process proc = new Process();
        // proc.StartInfo.FileName = path + "/BuiltGame.exe";
        // proc.Start();
    }
    
    
    [MenuItem("Build/Server/Windows(64)(il2cpp)")]
    public static void BuildWindowsServer()
    {
        var path = "../bin/server";
        var name = "broyal-server.exe";
        var target = BuildTarget.StandaloneWindows64;
        var option = BuildOptions.EnableHeadlessMode;

        var scenes = new[] {"Assets/StaticAssets/Scenes/Server.unity"};
        var scriptingImplementation = ScriptingImplementation.IL2CPP;

        BuildReport report = BuildGame(scenes, path, name, target, option, "1", scriptingImplementation);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
    
    [MenuItem("Build/Server/Linux(64)(mono2x)")]
    public static void BuildLinuxServer()
    {
        var path = "../bin/server-linux";
        var name = "broyal-server.x86_64";
        var target = BuildTarget.StandaloneLinux64;
        var option = BuildOptions.EnableHeadlessMode;

        var scenes = new[] {"Assets/StaticAssets/Scenes/Server.unity"};
        var scriptingImplementation = ScriptingImplementation.Mono2x;

        BuildReport report = BuildGame(scenes, path, name, target, option, "1", scriptingImplementation);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
    
    static BuildReport BuildGame(string[] scenes, string buildPath, string exeName, BuildTarget target,
        BuildOptions opts, string buildId, ScriptingImplementation scriptingImplementation, bool includeEditorMetadata = false)
    {
        var exePathName = buildPath + "/" + exeName;
        string fullBuildPath = Directory.GetCurrentDirectory() + "/" + buildPath;
        // var levels = new List<string>
        // {
        //     "Assets/Scenes/bootstrapper.unity",
        //     "Assets/Scenes/empty.unity"
        // };

        var levels = scenes.ToList();

        // Add scenes referenced from levelinfo's
        // foreach (var li in LoadLevelInfos())
        //     levels.Add(AssetDatabase.GetAssetPath(li.main_scene));

        Debug.Log("Levels in build:");
        foreach (var l in levels)
            Debug.Log(string.Format(" - {0}", l));

        Debug.Log("Building: " + exePathName);
        Directory.CreateDirectory(buildPath);

        if (scriptingImplementation == ScriptingImplementation.WinRTDotNET)
        {
            throw new Exception(string.Format("Unsupported scriptingImplementation {0}", scriptingImplementation));
        }

        //MakeBuildFilesWritable(fullBuildPath);
        //using (new MakeInstallationFilesWritableScope())
        {
            var il2cpp = scriptingImplementation == ScriptingImplementation.IL2CPP;

            var monoDirs = Directory.GetDirectories(fullBuildPath).Where(s => s.Contains("MonoBleedingEdge"));
            var il2cppDirs = Directory.GetDirectories(fullBuildPath)
                .Where(s => s.Contains("BackUpThisFolder_ButDontShipItWithYourGame"));
            var clearFolder = (il2cpp && monoDirs.Count() > 0) || (!il2cpp && il2cppDirs.Count() > 0);
            if (clearFolder)
            {
                Debug.Log(" deleting old folders ..");
                foreach (var file in Directory.GetFiles(fullBuildPath))
                    File.Delete(file);
                foreach (var dir in monoDirs)
                    Directory.Delete(dir, true);
                foreach (var dir in il2cppDirs)
                    Directory.Delete(dir, true);
                foreach (var dir in Directory.GetDirectories(fullBuildPath).Where(s => s.EndsWith("_Data")))
                    Directory.Delete(dir, true);
            }

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, scriptingImplementation);

            if (il2cpp)
            {
                PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Standalone,
                    Il2CppCompilerConfiguration.Release);
            }

            if (includeEditorMetadata)
            {
                //PerformanceTest.SaveEditorInfo(opts, target);
            }

            Environment.SetEnvironmentVariable("BUILD_ID", buildId, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("BUILD_UNITY_VERSION", InternalEditorUtility.GetFullUnityVersion(),
                EnvironmentVariableTarget.Process);

            var time = DateTime.Now;
            Debug.Log("BuildPipeline.BuildPlayer started");
            var result = BuildPipeline.BuildPlayer(levels.ToArray(), exePathName, target, opts);
            Debug.Log("BuildPipeline.BuildPlayer ended. Duration:" + (DateTime.Now - time).TotalSeconds + "s");

            Environment.SetEnvironmentVariable("BUILD_ID", "", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("BUILD_UNITY_VERSION", "", EnvironmentVariableTarget.Process);

            Debug.Log(" ==== Build Done =====");

            var stepCount = result.steps.Count();
            Debug.Log(" Steps:" + stepCount);
            for (var i = 0; i < stepCount; i++)
            {
                var step = result.steps[i];
                Debug.Log("-- " + (i + 1) + "/" + stepCount + " " + step.name + " " + step.duration.Seconds + "s --");
                foreach (var msg in step.messages)
                    Debug.Log(msg.content);
            }

            return result;
        }
    }
}
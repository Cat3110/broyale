using System.Collections;
using System.Collections.Generic;
using Unity.NetCode.Editor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

[InitializeOnLoad]
public class GhostEditorCompilationSettings : UnityEditor.Editor
{
    static GhostEditorCompilationSettings()
    {
        GhostAuthoringComponentEditor.DefaultRootPath = "/Scripts";
        GhostAuthoringComponentEditor.DefaultSerializerPrefix = "Server/Generated/";
        GhostAuthoringComponentEditor.DefaultSnapshotDataPrefix = "Mixed/Generated/";
        GhostAuthoringComponentEditor.DefaultUpdateSystemPrefix = "Client/Generated/";
        
        //CompilationPipeline.
        //GhostAuthoringComponentEditor.DefaultRootPath = "/Assets/Plugins/NetworkedDOTSTool/Scripts/Generated";
    }

}

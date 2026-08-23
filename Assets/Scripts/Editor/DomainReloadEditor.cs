using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DomainReloadEditor : EditorWindow
{
    [MenuItem("Tools/Project/DomainReload")]
    public static void ShowWindow()
    {
        EditorUtility.RequestScriptReload();

    }

}
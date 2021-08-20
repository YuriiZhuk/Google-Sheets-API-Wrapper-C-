using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GoogleSheetsWrapper))]
public class GoogleSheetsWrapperEditor : Editor
{
    private GoogleSheetsWrapper _instance;
    protected void OnEnable()
    {
        _instance = target as GoogleSheetsWrapper;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Import Google Sheet"))
        {           
             _instance.ImportGoogleSheet();
        }
    }
}

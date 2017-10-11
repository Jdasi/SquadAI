using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CoverPointManager))]
public class CoverPointGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Generate Cover Points"))
        {
            var t = (CoverPointManager)target;
            t.GenerateCoverPoints();
        }

        if (GUILayout.Button("Clear All"))
        {
            var t = (CoverPointManager)target;
            t.ClearCoverPoints();
        }
    }

}

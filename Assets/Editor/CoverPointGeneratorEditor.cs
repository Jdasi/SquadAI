using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CoverPointGenerator))]
public class CoverPointGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Generate Cover Points"))
        {
            var t = (CoverPointGenerator)target;
            t.GenerateCoverPoints();
        }

        if (GUILayout.Button("Clear All"))
        {
            var t = (CoverPointGenerator)target;
            t.Clear();
        }
    }

}

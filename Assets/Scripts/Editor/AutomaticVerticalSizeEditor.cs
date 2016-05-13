using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AutomaticVerticalSize))]
public class AutomaticVerticalSizeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // TODO: See which one of these is more efficient
        //base.OnInspectorGUI();
        DrawDefaultInspector();


    }
}

using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.NetCode;

[CustomEditor(typeof(GameObject))]
public class CustomGameObjectInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Add Ghost Authoring Component"))
        {
            GameObject[] gameObjects = Selection.gameObjects;
            foreach(GameObject g in gameObjects)
            {
                GhostAuthoringComponent gac = g.AddComponent(typeof(GhostAuthoringComponent)) as GhostAuthoringComponent;
            }
        }
    }
}


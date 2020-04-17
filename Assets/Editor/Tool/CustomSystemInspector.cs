using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SystemAsset))]
public class CustomSystemInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Create System Script"))
        {
            // remove whitespace and minus
            string name = target.name.Replace(" ", "_");
            Debug.Log(target.name.ToString());
            name = name.Replace("-", "_");
            string copyPath = "Assets/Scripts/Systems (Jobs)/" + name + ".cs";
            if (File.Exists(copyPath) == false)
            { // do not overwrite
                using (StreamWriter outfile = new StreamWriter(copyPath))
                {
                    ECSAssetTranslator.Write(outfile, name, (SystemAsset)target);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}

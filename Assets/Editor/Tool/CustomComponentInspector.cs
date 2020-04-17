using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(ComponentAsset))]
public class CustomComponentInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Create Component Script"))
        {
            // remove whitespace and minus
            string name = target.name.Replace(" ", "_");
            Debug.Log(target.name.ToString());
            name = name.Replace("-", "_");
            string copyPath = "Assets/Scripts/Components/" + name + ".cs";
            if (File.Exists(copyPath) == false)
            { // do not overwrite
                using (StreamWriter outfile = new StreamWriter(copyPath))
                {
                    ECSAssetTranslator.Write(outfile, name, (ComponentAsset)target);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
    }
}


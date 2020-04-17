
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(EntityAsset))]
public class CustomEntityInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Create Entity Spawner Script"))
        {
            // remove whitespace and minus
            string name = target.name.Replace(" ", "_");
            Debug.Log(target.name.ToString());
            name = name.Replace("-", "_");
            string copyPath = "Assets/Scripts/Entities/" + name + "Spawner.cs";
            if (File.Exists(copyPath) == false)
            { // do not overwrite
                using (StreamWriter outfile = new StreamWriter(copyPath))
                {
                    ECSAssetTranslator.Write(outfile, name, (EntityAsset)target);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
    }
}

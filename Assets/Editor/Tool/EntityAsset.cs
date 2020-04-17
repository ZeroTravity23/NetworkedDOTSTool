using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField, CreateAssetMenu(fileName = "Entity Archetype", menuName = "DOTS ECS/Create Entity Archetype")]
public class EntityAsset : ScriptableObject
{
    public ComponentAsset[] components = new ComponentAsset[1];

    public string ArchetypeToText()
    {
        string toText = "";
        for (int i = 0; i < components.Length; i++)
        {
            if(i != components.Length - 1)
                toText += components[i].name + ", \n";
            else
                toText += components[i].name;
        }
        return toText;
    }
}

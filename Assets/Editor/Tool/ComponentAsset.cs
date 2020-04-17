using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public enum TypeOf
{
    INT,
    FLOAT,
    STRING,
    TRANSLATION,
    TAG,
    COMPOSITE
}

[System.Serializable, CreateAssetMenu(fileName = "Component Asset", menuName = "DOTS ECS/Create Component")]
public class ComponentAsset : ScriptableObject
{
    public TypeOf type;
    public ComponentAsset[] components;

    public string ComponentToText()
    {
        string typeToString = "TYPE";
        switch (type)
        {
            case TypeOf.INT:
                typeToString = "int";
                break;
            case TypeOf.FLOAT:
                typeToString = "float";
                break;
            case TypeOf.STRING:
                typeToString = "NativeString32";
                break;
            case TypeOf.TRANSLATION:
                typeToString = "Translation";
                break;
            default:
                break;
        }
        return typeToString + " " + name.ToLower();
    }
}
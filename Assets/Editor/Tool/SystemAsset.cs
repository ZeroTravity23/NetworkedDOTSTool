using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField, CreateAssetMenu(fileName = "System Asset", menuName = "DOTS ECS/Create System")]
public class SystemAsset : ScriptableObject
{
    public bool isMainThread;
    public bool isNetworked;
    public ComponentAsset[] components = new ComponentAsset[0];
    public EntityAsset[] entities = new EntityAsset[0];
}

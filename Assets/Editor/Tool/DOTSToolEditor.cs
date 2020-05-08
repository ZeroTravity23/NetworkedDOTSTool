using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DOTSToolEditor : EditorWindow
{
    public ComponentAsset[] compositeAssets = new ComponentAsset[1];
    public ComponentAsset[] entityComponents = new ComponentAsset[0];
    public ComponentAsset[] systemComponents = new ComponentAsset[0];
    public EntityAsset[] systemEntities = new EntityAsset[0];
    Vector2 scrollPos;

    public string entityName = "";
    public string componentName = "";
    public string systemName = "";
    public bool isMainThread = true;
    public bool isNetworked = false;
    public TypeOf type;

    [MenuItem("DOTS/Networked DOTS Tool")]
    public static void ShowWindow()
    {
        GetWindow<DOTSToolEditor>("Networked DOTS Tool");
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUIStyle header = CreateHeaderStyle(); 
        EditorGUILayout.Space();
        GUILayout.Label("Entity", header);
        EditorGUILayout.HelpBox("This creates an EntityArchetype asset. EntityArchetypes act as templates for entities. Each EntityArchetype must have at least one component. You must create either a component or composite component to utilize this function.", MessageType.Info);
        entityName = EditorGUILayout.TextField("Entity Name", entityName);

        SerializedObject serial2 = new SerializedObject(this);
        SerializedProperty serialProp2 = serial2.FindProperty("entityComponents");
        EditorGUILayout.PropertyField(serialProp2, true);
        serial2.ApplyModifiedProperties();
        EditorGUI.BeginDisabledGroup(entityName.Equals("") || entityComponents.Length <= 0);
        if (GUILayout.Button("Create Entity Asset")){
            AssetDatabase.CreateAsset(new EntityAsset() {name = entityName, components = entityComponents}, "Assets/Editor/Entities/" + entityName +".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.Space();
        ScriptableObject so = this;
        SerializedObject serial = new SerializedObject(this);
        SerializedProperty serialProp = serial.FindProperty("compositeAssets");

        GUILayout.Label("Component", header);
        EditorGUILayout.HelpBox("This creates a Component asset. Components are the data stores or containers that systems act upon. They can exist as single (individual) stores, or they can be grouped together. In order to create a grouped component, at least two component assets must be added to the 'Composite Assets' array and the type of 'COMPOSITE' selected.", MessageType.Info);
        componentName = EditorGUILayout.TextField("Component Name", componentName);
        type = (TypeOf)EditorGUILayout.EnumPopup("Component Type", type);
        
        

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serialProp, true);
        serial.ApplyModifiedProperties();

        EditorGUI.BeginDisabledGroup(type == TypeOf.COMPOSITE || componentName.Equals(""));
        if (GUILayout.Button("Create Single Component Asset"))
        {
            AssetDatabase.CreateAsset(new ComponentAsset() { name = componentName, type = type }, "Assets/Editor/Components/" + componentName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginDisabledGroup(compositeAssets.Length <= 1 || type != TypeOf.COMPOSITE || componentName.Equals(""));
        if (GUILayout.Button("Create Composite Component Asset"))
        {
            AssetDatabase.CreateAsset(new ComponentAsset() { name = componentName, type = TypeOf.COMPOSITE, components = compositeAssets}, "Assets/Editor/Components/" + componentName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        GUILayout.Label("System", header);
        EditorGUILayout.HelpBox("This creates a System asset. Systems are the logic that act upon components. You must have at least one component. Alternatively, provide an entity asset, and the tool will create a job for all components associated with that archetype. If a 'TAG' component is associated with the job, it will apply to only those with this component. Please decide whether this should run on the main thread or be multithreaded.", MessageType.Info);
        systemName = EditorGUILayout.TextField("System Name", systemName);
        isMainThread = EditorGUILayout.Toggle("Run on Main Thread", isMainThread);
        isNetworked = EditorGUILayout.Toggle("Networked System", isNetworked);
        if (isMainThread == false)
        {
            EditorGUILayout.HelpBox("Disabling this enables multithreaded systems. Mutlithreading is not recommended for simple functions (i.e. input for movement).", MessageType.Warning);
        }
        SerializedObject serial3 = new SerializedObject(this);
        SerializedProperty serialProp3 = serial3.FindProperty("systemComponents");
        EditorGUILayout.PropertyField(serialProp3, true);
        serial3.ApplyModifiedProperties();
        SerializedObject serial4 = new SerializedObject(this);
        SerializedProperty serialProp4 = serial4.FindProperty("systemEntities");
        EditorGUILayout.PropertyField(serialProp4, true);
        serial4.ApplyModifiedProperties();
        EditorGUI.BeginDisabledGroup(systemName.Equals("") || (systemComponents.Length <= 0 && systemEntities.Length <= 0));
        if (GUILayout.Button("Create System Asset"))
        {
            AssetDatabase.CreateAsset(new SystemAsset() { name = systemName, isMainThread = isMainThread, isNetworked = isNetworked, components = systemComponents, entities = systemEntities }, "Assets/Editor/Systems/" + systemName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndScrollView();
    }

    private GUIStyle CreateHeaderStyle()
    {
        GUIStyle header = new GUIStyle(EditorStyles.label);
        header.normal.textColor = Color.white;
        header.alignment = TextAnchor.MiddleCenter;
        header.fontSize = 20;
        header.fontStyle = FontStyle.Bold;
        return header;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Entities;
using UnityEngine;

public static class ECSAssetTranslator
{
    public enum ECSType
    {
        ENTITY,
        COMPONENT,
        SYSTEM
    }

    //Create script for Entity Archetypes spawner
    public static void Write(StreamWriter sw, string name, EntityAsset entityAsset)
    {
        ECSType type = ECSType.ENTITY;
        Header(type, sw);
        Class_StructDefinition(type, sw, name);
        Define(entityAsset, name, sw);
        Terminate(sw);
    }

    //Create script for systems that act on single (non-composite) components, or creates a script for a single (non-composite) component;
    public static void Write(StreamWriter sw, string name, ComponentAsset component)
    {
        ECSType type = ECSType.COMPONENT;
        Header(type, sw);
        Class_StructDefinition(type, sw, name);
        Define(component, sw);
        Terminate(sw);
    }
    //Create script for systems
    public static void Write(StreamWriter sw, string name, SystemAsset system)
    {
        ECSType type = ECSType.SYSTEM;
        Header(type, sw);
        Class_StructDefinition(type, sw, name, system.isMainThread);
        Define(system, sw);
        Terminate(sw);
    }

    //public static void Write(StreamWriter sw, string name, )

    //Establish Unity dependencies
    public static void Header(ECSType type, StreamWriter sw)
    {
        switch (type)
        {
            case ECSType.ENTITY:
                sw.WriteLine("using Unity.Entities;");
                sw.WriteLine("using Unity.Collections;");
                sw.WriteLine("using System.Collections;");
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine("using UnityEngine;");
                sw.WriteLine();
                break;
            case ECSType.COMPONENT:
                sw.WriteLine("using Unity.Entities;");
                sw.WriteLine("using Unity.Collections;");
                sw.WriteLine();
                break;
            case ECSType.SYSTEM:
                sw.WriteLine("using UnityEngine;");
                sw.WriteLine("using Unity.Entities;");
                sw.WriteLine("using Unity.Jobs;");
                sw.WriteLine("using Unity.Burst;");
                sw.WriteLine();
                break;
            default:
                sw.WriteLine("using UnityEngine;");
                sw.WriteLine("using Unity.Entities;");
                sw.WriteLine("using Unity.Collections;");
                sw.WriteLine("using Unity.Jobs;");
                sw.WriteLine("using Unity.Burst;");
                sw.WriteLine("using System.Collections;");
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine();
                break;
        }
    }

    //Create class or struct definition.
    public static void Class_StructDefinition(ECSType type, StreamWriter sw, string name, bool isMainThread)
    {
        if (type != ECSType.SYSTEM)
            isMainThread = false;
        switch (type)
        {
            case ECSType.ENTITY:
                sw.WriteLine("public int quantity = 10;");
                sw.WriteLine();
                sw.WriteLine("public class " + name + "Spawner : MonoBehavior {");
                break;
            case ECSType.COMPONENT:
                sw.WriteLine("[GenerateAuthoringComponent]");
                sw.WriteLine("public struct " + name + " : IComponentData {");
                break;
            case ECSType.SYSTEM:
                if (isMainThread)
                    sw.WriteLine("[AlwaysSynchronizeSystem]");
                sw.WriteLine("public class " + name + " : JobComponentSystem {");
                break;
            default:
                sw.WriteLine("//public struct " + name + " : EntityArchetype {");
                sw.WriteLine("//public struct " + name + " : IComponentData {");
                sw.WriteLine("//public class " + name + " : JobComponentSystem {");
                break;
        }
    }

    public static void Class_StructDefinition(ECSType type, StreamWriter sw, string name)
    {
        switch (type)
        {
            case ECSType.ENTITY:
                sw.WriteLine("public class " + name + "Spawner : MonoBehaviour {");
                sw.WriteLine();
                sw.WriteLine("public int quantity = 10;");
                sw.WriteLine();
                break;
            case ECSType.COMPONENT:
                sw.WriteLine("[GenerateAuthoringComponent]");
                sw.WriteLine("public struct " + name + " : IComponentData {");
                break;
            default:
                sw.WriteLine("//public int quantity = 10; //Only necessary for spawner script.");
                sw.WriteLine();
                sw.WriteLine("//[AlwaysSynchronizeSystem]");
                sw.WriteLine("//public class " + name + " : JobComponentSystem {");
                sw.WriteLine("//public class " + name + "Spawner : MonoBehavior {");
                break;
        }
    }


    //Defines entity spawner script. Creates entity archetype. To be attached to in game game object.
    public static void Define(EntityAsset entity, string name, StreamWriter sw)
    {
        string archName = name.ToLower() + "Archetype";
        sw.WriteLine("void Start() {");
        sw.WriteLine("EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;");
        sw.Write("EntityArchetype " + archName + " = entityManager.CreateArchetype(");
        for (int i = 0; i < entity.components.Length; i++)
        {
            if (i != entity.components.Length - 1)
            {
                sw.Write("typeof(" + entity.components[i].name + "),");
            }
            else
            {
                sw.Write("typeof(" + entity.components[i].name + "));");
            }

        }
        sw.WriteLine();
        sw.WriteLine("NativeArray<Entity> entityArray = new NativeArray<Entity>(quantity, Allocator.Temp);");
        sw.WriteLine("entityManager.CreateEntity(" + archName + ", entityArray);");
        sw.WriteLine();
        sw.WriteLine("//Utilize this portion to iterate through and set values.");
        sw.WriteLine("//for (int j = 0; j < entityArray.Length; j++) {");
        sw.WriteLine("//Entity entity = entityArray[j];");
        sw.WriteLine("//entityManager.SetComponentData(entity, new [COMPONENT TYPE] { value = [SET VALUE], value2 = [SET VALUE], . . . }); ");
        sw.WriteLine("//}");
        sw.WriteLine();
        sw.WriteLine("entityArray.Dispose(); //Always dispose of native arrays when finished.");
        sw.WriteLine("}");
    }

    //Defines entity spawner script. Creates entity archetype. To be attached to in game game object.
    public static void Define(ComponentAsset component, StreamWriter sw)
    {
        if (component.type == TypeOf.COMPOSITE)
        {
            for (int i = 0; i < component.components.Length; i++)
            {
                if (component.components[i].type == TypeOf.TAG)
                {
                    //Do Nothing
                }
                else
                {
                    sw.WriteLine("public " + component.components[i].ComponentToText() + ";");
                }
            }
        }
        else if (component.type == TypeOf.TAG)
        {
            //Do Nothing
        }
        else
        {
            sw.WriteLine("public " + component.ComponentToText() + ";");
        }
    }

    //Defines system script. Creates script based on componets to be altered.
    public static void Define(SystemAsset system, StreamWriter sw)
    {
        sw.WriteLine("protected override JobHandle OnUpdate(JobHandle inputDeps) {");
        sw.WriteLine("Entities");
        //Try and find a component with TAG type.
        ComponentAsset asset = null;
        for (int i = 0; i < system.components.Length; i++)
        {
            if (system.components[i].type == TypeOf.TAG)
            {
                asset = system.components[i];
                break;
            }
            for (int k = 0; k < system.components[i].components.Length; k++)
            {
                if (system.components[i].components[k].type == TypeOf.TAG)
                {
                    asset = system.components[i].components[k];
                    break;
                }
            }
        }

        for (int i = 0; i < system.entities.Length; i++)
        {
            for (int j = 0; j < system.entities[i].components.Length; j++)
            {
                if (system.entities[i].components[j].type == TypeOf.TAG)
                {
                    asset = system.entities[i].components[j];
                    break;
                }
                for (int k = 0; k < system.entities[i].components[j].components.Length; k++)
                {
                    if (system.entities[i].components[j].components[k].type == TypeOf.TAG)
                    {
                        asset = system.entities[i].components[j].components[k];
                        break;
                    }
                }
            }
            
        }
        if (asset != null)
        {
            sw.WriteLine(".WithAll<" + asset.name + ">()");
        }
        sw.Write(".ForEach((Entity entity, ");
        List<ComponentAsset> assets = new List<ComponentAsset>();
        for (int i = 0; i < system.components.Length; i++)
        {
            if (system.components[i].type != TypeOf.TAG && system.components[i].type != TypeOf.COMPOSITE)
            {
                assets.Add(system.components[i]);
            } else if (system.components[i].type == TypeOf.COMPOSITE)
            {
                for (int k = 0; k < system.components[i].components.Length; k++)
                {
                    if (system.components[i].components[k].type != TypeOf.TAG)
                    {
                        assets.Add(system.components[i].components[k]);
                    }
                }
            }
        }

        for (int i = 0; i < system.entities.Length; i++)
        {
            for (int j = 0; j < system.entities[i].components.Length; j++)
            {
                if (system.entities[i].components[j].type != TypeOf.TAG && system.entities[i].components[j].type != TypeOf.COMPOSITE)
                {
                    assets.Add(system.entities[i].components[j]);
                    break;
                }
                else if (system.entities[i].components[j].type == TypeOf.COMPOSITE)
                {
                    for (int k = 0; k < system.entities[i].components[j].components.Length; k++)
                    {
                        if (system.entities[i].components[j].components[k].type != TypeOf.TAG)
                        {
                            assets.Add(system.entities[i].components[j].components[k]);
                        }
                    }
                }
            }
        }
        for (int i = 0; i < assets.Count; i++)
        {
            if (i != assets.Count - 1)
                sw.Write("ref " + assets[i].name + " " + assets[i].name.ToLower() + ", ");
            else
                sw.Write("ref " + assets[i].name + " " + assets[i].name.ToLower() + ") => {");
        }
        sw.WriteLine();
        sw.WriteLine("//Logic goes here!");
        if (system.isMainThread)
        {
            sw.WriteLine("}).Run();");
            sw.WriteLine("return default;");
            sw.WriteLine("}");
        }else
        {
            sw.WriteLine("}).Schedule(inputDeps);");
            sw.WriteLine("}");
        }
        
    }

    

    //Closing brace of class_struct
    public static void Terminate(StreamWriter sw)
    {
        sw.WriteLine("}");
    }
}

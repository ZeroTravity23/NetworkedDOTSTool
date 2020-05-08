using Unity.Entities;
using Unity.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour {

public int quantity = 10;

void Start() {
EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
EntityArchetype testArchetype = entityManager.CreateArchetype(typeof(Level));
NativeArray<Entity> entityArray = new NativeArray<Entity>(quantity, Allocator.Temp);
entityManager.CreateEntity(testArchetype, entityArray);

//Utilize this portion to iterate through and set values.
//for (int j = 0; j < entityArray.Length; j++) {
//Entity entity = entityArray[j];
//entityManager.SetComponentData(entity, new [COMPONENT TYPE] { value = [SET VALUE], value2 = [SET VALUE], . . . }); 
//}

entityArray.Dispose(); //Always dispose of native arrays when finished.
}
}

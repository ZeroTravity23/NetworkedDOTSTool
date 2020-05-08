using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.NetCode;
using Unity.Networking.Transport;

[AlwaysUpdateSystem, UpdateInWorld(UpdateInWorld.TargetWorld.ClientAndServer)]
public class RegenerateHealth : JobComponentSystem {
protected override JobHandle OnUpdate(JobHandle inputDeps) {
Entities
.WithoutBurst() //Remove this if Burst compilation is possible.
.ForEach((Entity entity, ref Health health) => {
//Logic goes here!
}).Run();
return default;
}
}

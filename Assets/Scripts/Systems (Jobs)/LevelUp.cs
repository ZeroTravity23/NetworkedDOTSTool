using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.NetCode;
using Unity.Networking.Transport;

[AlwaysUpdateSystem, UpdateInWorld(UpdateInWorld.TargetWorld.ClientAndServer)]
public class LevelUp : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities
        .WithoutBurst()
        .ForEach((ref PlayerData data) =>
        {
            data.level += 10;
        }).Run();
        return default;
    }
}

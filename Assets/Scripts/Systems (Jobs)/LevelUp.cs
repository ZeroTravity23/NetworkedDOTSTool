using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;

[AlwaysUpdateSystem]
public class LevelUp : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities
        .WithoutBurst()
        .ForEach((ref PlayerData data) =>
        {
            //Logic goes here!
            data.level += 10;
        }).Run();
        return default;
    }
}

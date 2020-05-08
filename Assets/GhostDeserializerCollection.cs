using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct DOTSGhostDeserializerCollection : IGhostDeserializerCollection
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public string[] CreateSerializerNameList()
    {
        var arr = new string[]
        {
            "CubeGhostSerializer",
            "SpawnerGhostSerializer",
        };
        return arr;
    }

    public int Length => 2;
#endif
    public void Initialize(World world)
    {
        var curCubeGhostSpawnSystem = world.GetOrCreateSystem<CubeGhostSpawnSystem>();
        m_CubeSnapshotDataNewGhostIds = curCubeGhostSpawnSystem.NewGhostIds;
        m_CubeSnapshotDataNewGhosts = curCubeGhostSpawnSystem.NewGhosts;
        curCubeGhostSpawnSystem.GhostType = 0;
        var curSpawnerGhostSpawnSystem = world.GetOrCreateSystem<SpawnerGhostSpawnSystem>();
        m_SpawnerSnapshotDataNewGhostIds = curSpawnerGhostSpawnSystem.NewGhostIds;
        m_SpawnerSnapshotDataNewGhosts = curSpawnerGhostSpawnSystem.NewGhosts;
        curSpawnerGhostSpawnSystem.GhostType = 1;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_CubeSnapshotDataFromEntity = system.GetBufferFromEntity<CubeSnapshotData>();
        m_SpawnerSnapshotDataFromEntity = system.GetBufferFromEntity<SpawnerSnapshotData>();
    }
    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        ref DataStreamReader reader, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                return GhostReceiveSystem<DOTSGhostDeserializerCollection>.InvokeDeserialize(m_CubeSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 1:
                return GhostReceiveSystem<DOTSGhostDeserializerCollection>.InvokeDeserialize(m_SpawnerSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    public void Spawn(int serializer, int ghostId, uint snapshot, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                m_CubeSnapshotDataNewGhostIds.Add(ghostId);
                m_CubeSnapshotDataNewGhosts.Add(GhostReceiveSystem<DOTSGhostDeserializerCollection>.InvokeSpawn<CubeSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 1:
                m_SpawnerSnapshotDataNewGhostIds.Add(ghostId);
                m_SpawnerSnapshotDataNewGhosts.Add(GhostReceiveSystem<DOTSGhostDeserializerCollection>.InvokeSpawn<SpawnerSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<CubeSnapshotData> m_CubeSnapshotDataFromEntity;
    private NativeList<int> m_CubeSnapshotDataNewGhostIds;
    private NativeList<CubeSnapshotData> m_CubeSnapshotDataNewGhosts;
    private BufferFromEntity<SpawnerSnapshotData> m_SpawnerSnapshotDataFromEntity;
    private NativeList<int> m_SpawnerSnapshotDataNewGhostIds;
    private NativeList<SpawnerSnapshotData> m_SpawnerSnapshotDataNewGhosts;
}
public struct EnableDOTSGhostReceiveSystemComponent : IComponentData
{}
public class DOTSGhostReceiveSystem : GhostReceiveSystem<DOTSGhostDeserializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableDOTSGhostReceiveSystemComponent>();
    }
}

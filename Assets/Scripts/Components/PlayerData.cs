using Unity.Entities;
using Unity.Collections;

[GenerateAuthoringComponent]
public struct PlayerData : IComponentData {
public float health;
public int level;
}

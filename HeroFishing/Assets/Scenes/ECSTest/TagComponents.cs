using Unity.Entities;

public struct AutoDestroyTag : IComponentData {
    public float LifeTime;
    public float ExistTime;
}
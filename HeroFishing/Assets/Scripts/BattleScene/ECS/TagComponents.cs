using Unity.Entities;

public struct AutoDestroyTag : IComponentData {
    public float LifeTime;//生命週期
    public float ExistTime;//目前存活秒數(預設為0)
}
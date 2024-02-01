using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionBase : MonoBehaviour, IUpdate {
    public int Order { get; } = 1;
    protected virtual void OnEnable() {
        UpdateSystem.Instance.RegisterUpdate(this);
    }

    protected virtual void OnDisable() {
        UpdateSystem.Instance.UnregisterUpdate(this);
    }

    public virtual void OnUpdate(float deltaTime) {

    }
}

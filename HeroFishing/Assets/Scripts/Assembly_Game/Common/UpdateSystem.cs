using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdate {
    int Order { get; }
    void OnUpdate(float deltaTime);
}

public interface IFixedUpdate {
    int Order { get; }
    void OnFixedUpdate(float deltaTime);
}

public interface ILateUpdate {
    int Order { get; }
    void OnLateUpdate(float deltaTime);
}

public class UpdateSystem : MonoBehaviour {
    private static UpdateSystem _instance;
    public static UpdateSystem Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<UpdateSystem>();
                if (_instance == null) {
                    GameObject go = new GameObject("UpdateSystem");
                    go.hideFlags = HideFlags.DontSave;
                    _instance = go.AddComponent<UpdateSystem>();
                }
            }
            return _instance;
        }
    }

    private List<IUpdate> _updates = new List<IUpdate>();
    private List<IFixedUpdate> _fixedUpdates = new List<IFixedUpdate>();
    private List<ILateUpdate> _lateUpdates = new List<ILateUpdate>();

    public void RegisterUpdate(IUpdate update) {
        if (!_updates.Contains(update)) {
            _updates.Add(update);
            _updates.Sort((a, b) => a.Order - b.Order);
        }
    }

    public void RegisterFixedUpdate(IFixedUpdate fixedUpdate) {
        if (!_fixedUpdates.Contains(fixedUpdate)) {
            _fixedUpdates.Add(fixedUpdate);
            _fixedUpdates.Sort((a, b) => a.Order - b.Order);
        }
    }

    public void RegisterLateUpdate(ILateUpdate lateUpdate) {
        if (!_lateUpdates.Contains(lateUpdate)) {
            _lateUpdates.Add(lateUpdate);
            _lateUpdates.Sort((a, b) => a.Order - b.Order);
        }
    }

    public void UnregisterUpdate(IUpdate update) {
        if (_updates.Contains(update)) {
            _updates.Remove(update);
        }
    }

    public void UnregisterFixedUpdate(IFixedUpdate fixedUpdate) {
        if (_fixedUpdates.Contains(fixedUpdate)) {
            _fixedUpdates.Remove(fixedUpdate);
        }
    }

    public void UnregisterLateUpdate(ILateUpdate lateUpdate) {
        if (_lateUpdates.Contains(lateUpdate)) {
            _lateUpdates.Remove(lateUpdate);
        }
    }

    private void Update() {
        float deltaTime = Time.deltaTime;
        for (int i = 0; i < _updates.Count; i++) {
            _updates[i].OnUpdate(deltaTime);
        }
    }

    private void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        for (int i = 0; i < _fixedUpdates.Count; i++) {
            _fixedUpdates[i].OnFixedUpdate(deltaTime);
        }
    }

    private void LateUpdate() {
        float deltaTime = Time.deltaTime;
        for (int i = 0; i < _lateUpdates.Count; i++) {
            _lateUpdates[i].OnLateUpdate(deltaTime);
        }
    }
}

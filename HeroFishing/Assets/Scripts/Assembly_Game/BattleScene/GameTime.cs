using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public static class GameTime {
    static GameTime() {
        Observable.EveryUpdate().Subscribe(_ => {
            Update(Time.deltaTime);
        });
    }
    public static float LastestOverride { get; private set; }
    public static float Current { get; private set; }
    public static float Delta { get; private set; }
    public static void Update(float deltaTime) {
        Delta += deltaTime;
        Current += deltaTime;
    }

    public static void Override(float time) {
        //Debug.Log($"override: time {time} current {Current} Delta {Delta} Last Override {LastestOverride}");
        Delta = 0;
        Current = time;
        LastestOverride = time;
    }
}

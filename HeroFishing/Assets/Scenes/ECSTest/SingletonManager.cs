using HeroFishing.Battlefield;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial struct DirectoryInitSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        var entity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(entity, new BattlefieldSettingSingleton {
            MyAttackState = BattlefieldSettingSingleton.AttackState.Ready,
        });
    }
}
public struct BattlefieldSettingSingleton : IComponentData {
    public enum AttackState {
        StopAttacking,
        Ready,
        Attacking,
    }
    public AttackState MyAttackState;
    public AttackData MyAttackData;

}

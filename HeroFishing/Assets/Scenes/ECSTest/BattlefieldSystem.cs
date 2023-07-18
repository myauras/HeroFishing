using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeroFishing.Battlefield {
    public partial struct BattlefieldSystem : ISystem {

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            CreateSingleton(ref state);
        }

        void CreateSingleton(ref SystemState state) {
            //戰場設定
            state.EntityManager.CreateSingleton<Singleton_BattlefieldSetting>();
            ref var battlefieldSetting = ref SystemAPI.GetSingletonRW<Singleton_BattlefieldSetting>().ValueRW;
            battlefieldSetting.HeroPos = new float3(6.5f, 1, 3f);

            //攻擊參數
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            entityManager.CreateSingleton<Singleton_BattlefieldSetting>();
        }


    }
}

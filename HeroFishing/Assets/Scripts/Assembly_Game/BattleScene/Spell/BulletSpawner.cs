using HeroFishing.Battle;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SpawnBulletInfo {
    public int PrefabID;
    public int SubPrefabID;
    public Vector3 InitPosition;
    public Vector3 FirePosition;
    public Vector3 InitDirection;
    public float ProjectileScale;
    public float ProjectileDelay;
    public float LifeTime;
    public bool IgnoreFireModel;
}

public static class BulletSpawner {
    public static bool Spawn(SpawnBulletInfo spawnBulletInfo, out Bullet bullet) {
        var pool = PoolManager.Instance;
        var bulletGO = pool.PopBullet(spawnBulletInfo.PrefabID, spawnBulletInfo.SubPrefabID);

#if UNITY_EDITOR
        bulletGO.name = "BulletProjectile" + spawnBulletInfo.PrefabID;
        //bulletGO.hideFlags |= HideFlags.HideAndDontSave;
#else
        bulletGO.hideFlags |= HideFlags.HideAndDontSave;
#endif
        if (!bulletGO.TryGetComponent(out bullet)) {
            WriteLog.LogErrorFormat("子彈{0}身上沒有掛Bullet Component", bulletGO.name);
            return false;
        }

        if (!bulletGO.TryGetComponent(out AutoBackPool abp)) {
            abp = bulletGO.AddComponent<AutoBackPool>();
        }
        abp.BackTime = spawnBulletInfo.LifeTime;

        //設定子彈Gameobject的Transfrom
        bullet.transform.SetLocalPositionAndRotation(spawnBulletInfo.InitPosition, Quaternion.LookRotation(spawnBulletInfo.InitDirection));
        bullet.transform.SetParent(null);
        if (spawnBulletInfo.ProjectileScale != 0)
            bullet.transform.localScale = spawnBulletInfo.ProjectileScale * Vector3.one;

        var firePosition = spawnBulletInfo.FirePosition == Vector3.zero ? spawnBulletInfo.InitPosition : spawnBulletInfo.FirePosition;

        //設定子彈模型
        bullet.Create(new BulletInfo {
            PrefabID = spawnBulletInfo.PrefabID,
            SubPrefabID = spawnBulletInfo.SubPrefabID,
            IgnoreFireModel = spawnBulletInfo.IgnoreFireModel,
            FirePosition = firePosition,
            Delay = spawnBulletInfo.ProjectileDelay,
        });
        return true;
    }

    public static void AddCollisionComponent(BulletCollisionInfo info, Bullet bullet) {
        var collision = AddCollisionComponent<BulletCollision>(bullet);
        collision.Init(info);
    }

    public static void AddCollisionComponent(AreaCollisionInfo info, Bullet bullet) {
        var collision = AddCollisionComponent<AreaCollision>(bullet);
        collision.Init(info);
    }

    private static T AddCollisionComponent<T>(Bullet bullet) where T : MonoBehaviour {
        if (!bullet.gameObject.TryGetComponent<T>(out var collision))
            collision = bullet.gameObject.AddComponent<T>();
        return collision;
    }
}

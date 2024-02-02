using HeroFishing.Battle;
using HeroFishing.Main;
using Scoz.Func;
using UnityEngine;

public struct SpawnHitParticleInfo {
    public string SpellID;
    public Monster Monster;
    public Vector3 HitPos;
    public Quaternion HitRot;
}

public static class HitParticleSpawner
{
    public static void Spawn(SpawnHitParticleInfo info) {
        var spellData = HeroSpellJsonData.GetData(info.SpellID);
        if (spellData == null)
            throw new System.Exception("spell data is null in id: " + info.SpellID);
        int prefabID = spellData.PrefabID;

        Vector3 position = Vector3.zero;
        switch (info.Monster.MyData.HitEffectPos) {
            case MonsterJsonData.HitEffectPosType.HitPos:
                position = info.HitPos;
                break;
            case MonsterJsonData.HitEffectPosType.Self:
                float y = GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY) / 2;
                position = info.Monster.transform.position;
                position.y = y;
                break;
            default:
                break;
        }
        PoolManager.Instance.Pop(prefabID, 0, PoolManager.PopType.Hit, position, info.HitRot);
    }
}

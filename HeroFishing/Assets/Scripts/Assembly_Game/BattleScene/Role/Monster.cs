using DG.Tweening;
using HeroFishing.Main;
using Scoz.Func;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HeroFishing.Battle {
    public class Monster : Role {
        public MonsterJsonData MyData { get; private set; }
        public int MonsterID => MyData.ID;
        public int MonsterIdx { get; private set; }

        public int KillHeroIndex { get; private set; } = -1;

        private MonsterSpecialize MyMonsterSpecialize;
        private MonsterExplosion Explosion;

        private Vector3 _lastHitDirection;

        private GameObject LockObj;

        private static readonly List<Monster> s_aliveMonsters = new List<Monster>();
        private static readonly Dictionary<int, Monster> s_idxToMonsterMapping = new Dictionary<int, Monster>();
        private static readonly Dictionary<int, List<Material>> s_originMatDic = new();
        private static readonly Dictionary<int, List<Material>> s_frozenMatDic = new();

        private const string MAT_FREEZE_OPAQUE = "FreezeMonOp";
        private const string MAT_FREEZE_TRANSPARENT = "FreezeMonTr";

        private bool _inField;
        public bool InField {
            get => _inField;
            set {
                _inField = value;
                if (!_inField) {
                    Lock(false);
                    if (s_aliveMonsters.Contains(this)) {
                        s_aliveMonsters.Remove(this);
                        s_idxToMonsterMapping.Remove(MonsterIdx);
                    }
                }
            }
        }
        public bool IsAlive => s_aliveMonsters.Contains(this) && InField;

        public void SetData(int _monsterID, int _monsterIdx, Action _ac) {
            MyData = MonsterJsonData.GetData(_monsterID);
            MonsterIdx = _monsterIdx;
            LoadModel(_ac);
        }

        void LoadModel(Action _ac) {
            string path = string.Format("Monster/{0}/{0}.prefab", MyData.Ref);
            AddressablesLoader.GetPrefabResourceByPath<GameObject>(path, (prefab, handle) => {
                var go = Instantiate(prefab, transform);
                go.transform.localPosition = prefab.transform.localPosition;
                go.transform.localRotation = prefab.transform.localRotation;
                go.transform.localScale = prefab.transform.localScale;
                var monsterSpecialize = go.GetComponent<MonsterSpecialize>();
                MyMonsterSpecialize = monsterSpecialize;
                Explosion = go.GetComponent<MonsterExplosion>();
                AddressableManage.SetToChangeSceneRelease(handle);//切場景再釋放資源
                SetModel();
                LoadDone();
                if (!s_aliveMonsters.Contains(this)) {
                    s_aliveMonsters.Add(this);
                    s_idxToMonsterMapping.Add(MonsterIdx, this);
                }
                _ac?.Invoke();
            });
        }

        public void OnHit(string _spellID, float3 hitDirection) {
            var spellData = HeroSpellJsonData.GetData(_spellID);
            _lastHitDirection = hitDirection;
            if (spellData == null) return;
            HitShaderEffect(spellData);
        }


        void HitShaderEffect(HeroSpellJsonData _spellData) {
            if (MySkinnedMeshRenderers == null || MySkinnedMeshRenderers.Length == 0) return;
            //Color color = TextManager.ParseTextToColor32(GameSettingJsonData.GetStr(GameSetting.HitEffect_OutlineColor)); 已不使用Gamesetting的設定
            //MySkinnedMaterial.SetFloat("_FresnelPower", GameSettingJsonData.GetFloat(GameSetting.HitEffect_FresnelPower));已不使用Gamesetting的設定

            //設定怪物被擊中Shader效果
            Color32 color = new Color32((byte)_spellData.HitMonsterShaderSetting[0], (byte)_spellData.HitMonsterShaderSetting[1], (byte)_spellData.HitMonsterShaderSetting[2], (byte)_spellData.HitMonsterShaderSetting[3]);
            float hdrIntensity = _spellData.HitMonsterShaderSetting[4]; // hdr color intensity設定
            //設定HDR Color
            Color32 hdrColor = new Color32(
               (byte)(color.r * hdrIntensity),
               (byte)(color.g * hdrIntensity),
               (byte)(color.b * hdrIntensity),
               color.a
            );
            PropertyBlock.SetVector("_OutlineColor", new Vector3(color.r, color.g, color.b));
            PropertyBlock.SetFloat("_FresnelPower", _spellData.HitMonsterShaderSetting[5]);
            PropertyBlock.SetFloat("_Opacity", _spellData.HitMonsterShaderSetting[6]);
            PropertyBlock.SetFloat("_Smoothness", _spellData.HitMonsterShaderSetting[7]);
            PropertyBlock.SetFloat("_Metallic", _spellData.HitMonsterShaderSetting[8]);
            DOTween.To(() => 1f, x => {
                PropertyBlock.SetFloat("_Opacity", x);
                SetPropertyBlock(PropertyBlock);
            }, 0f, GameSettingJsonData.GetFloat(GameSetting.HitEffect_DecaySec));
        }

        public void Lock(bool active) {
            if (active) {
                if (LockObj == null) {
                    AddressablesLoader.GetParticle("OtherEffect/Script_LockEffect", (go, handle) => {
                        AddressableManage.SetToChangeSceneRelease(handle);
                        LockObj = Instantiate(go, transform);
                        LockObj.transform.localPosition = new Vector3(0, 1.19f, 0);
                        Lock(active);
                    });
                }
            }

            if (LockObj != null) {
                LockObj.SetActive(active);
            }
        }

        public void Explode(int heroIndex) {
            if (Explosion != null)
                Explosion.Explode();
            DieInternal(heroIndex);
        }

        public void Die(int heroIndex) {
            Observable.Timer(TimeSpan.FromMilliseconds(150)).Subscribe(_ => {
                for (int i = 0; i < MySkinnedMeshRenderers.Length; i++) {
                    MySkinnedMeshRenderers[i].enabled = false;
                }
            });
            DieInternal(heroIndex);
        }

        private void DieInternal(int heroIndex) {
            KillHeroIndex = heroIndex;

            if (AllocatedRoom.Instance == null && KillHeroIndex == 0) {
                var hero = BattleManager.Instance.GetHero(KillHeroIndex);
                hero.HoldStoredPoints(MonsterIdx, (int)MyData.Odds * BattleManager.Instance.Bet);
            }

            if (MyData.MyMonsterType == MonsterJsonData.MonsterType.Boss) MonsterScheduler.BossExist = false;
            if (MyMonsterSpecialize != null) {
                MyMonsterSpecialize.CloseAddOnObjs();
                MyMonsterSpecialize.PlayCoinEffect(MyData.MyMonsterSize, MySkinnedMeshRenderers[0], KillHeroIndex, MonsterIdx);
                if (MyData.DropID > 0) {
                    if (MyData.DropID == 5)
                        MyMonsterSpecialize.PlayDropEffect(MyData.DropID, heroIndex);
                }
            }
            if (s_aliveMonsters.Contains(this)) {
                s_aliveMonsters.Remove(this);
                s_idxToMonsterMapping.Remove(MonsterIdx);
            }

            Lock(false);
        }

        // 如果要每個Instance都創建一個材質球會再第一次創建過久。所以讓材質球的創建跟怪物ID綁定
        public void Freeze() {
            int id = MyData.ID;
            // 如果原始材質尚未存過，做一次儲存。
            if (!s_originMatDic.ContainsKey(id)) {
                var matList = new List<Material>();
                s_originMatDic.Add(id, matList);

                foreach (var renderer in MySkinnedMeshRenderers) {
                    matList.AddRange(renderer.materials);
                }
            }

            // 冰凍材質，如果沒有的話就要new後填入Dictionary裡，有的話直接取用
            int startIndex = 0;
            if (!s_frozenMatDic.TryGetValue(id, out var allMatList)) {
                allMatList = new List<Material>();
                s_frozenMatDic.Add(id, allMatList);
                SetupFrozenDic();
            }
            else {
                for (int i = 0; i < MySkinnedMeshRenderers.Length; i++) {
                    var renderer = MySkinnedMeshRenderers[i];
                    var matList = allMatList.GetRange(startIndex, renderer.materials.Length);
                    renderer.SetSharedMaterials(matList);
                    Explosion.SetMaterials(i, matList);
                    startIndex += renderer.materials.Length;
                }
            }

            if (MyAni != null)
                MyAni.enabled = false;
            FreezeSmoothly();
        }

        public void UnFreeze() {
            if (MyAni != null)
                MyAni.enabled = true;
            var allMatList = s_originMatDic[MyData.ID];
            int matIndex = 0;
            foreach (var renderer in MySkinnedMeshRenderers) {
                for (int j = 0; j < renderer.sharedMaterials.Length; j++) {
                    var material = renderer.sharedMaterials[j];
                    material.SetFloat("_EdgePower", 0);
                    material.SetFloat("_FreezeMask", 0);
                    material.SetFloat("_IceAmount", 0);
                    material.SetFloat("_IcicleAmount", 0);
                    material.SetFloat("_Metallic", 0);
                    material.SetFloat("_Smooth", 0);
                }
                var matList = allMatList.GetRange(matIndex, renderer.materials.Length);
                renderer.SetSharedMaterials(matList);
                matIndex += renderer.materials.Length;
            }
        }
        private void FreezeSmoothly() {
            var materials = MySkinnedMeshRenderers.SelectMany(r => r.sharedMaterials);
            DOTween.To(() => 0f, value => {
                //Debug.Log(value);
                foreach (var material in materials) {
                    material.SetFloat("_EdgePower", value * 2.8f);
                    material.SetFloat("_FreezeMask", value * 2f);
                    material.SetFloat("_IceAmount", value * 0.25f);
                    material.SetFloat("_IcicleAmount", value);
                    material.SetFloat("_Metallic", value * 0.2f);
                    material.SetFloat("_Smooth", value * 0.4f);
                }
            }, 1.0f, 1.5f);
        }

        private void SetupFrozenDic() {
            int startIndex = 0;

            var frozenMatOpaque = ResourcePreSetter.GetMaterial(MAT_FREEZE_OPAQUE);
            var frozenMatTransparent = ResourcePreSetter.GetMaterial(MAT_FREEZE_TRANSPARENT);

            for (int index = 0; index < MySkinnedMeshRenderers.Length; index++) {
                var renderer = MySkinnedMeshRenderers[index];
                var matList = new List<Material>();
                for (int i = 0; i < renderer.sharedMaterials.Length; i++) {
                    var oldMat = renderer.sharedMaterials[i];
                    if (oldMat != null) {
                        var texture = oldMat.mainTexture;
                        var tag = oldMat.GetTag("RenderType", false);
                        //Debug.Log("tag " + tag);
                        Material newMat;
                        if (tag == "Transparent")
                            newMat = new Material(frozenMatTransparent);
                        else
                            newMat = new Material(frozenMatOpaque);
                        //if (newMat == null) {
                        //    // 存失敗，為了不打亂list順序，還是要存個null
                        //    WriteLog.LogErrorFormat("new mat is null. " + tag);
                        //    matList.Add(null);
                        //    continue;
                        //}
                        newMat.mainTexture = texture;
                        matList.Add(newMat);
                    }
                }
                // 設定renderer
                renderer.SetSharedMaterials(matList);
                Explosion.SetMaterials(index, matList);
                s_frozenMatDic[MyData.ID].AddRange(matList);
                startIndex += renderer.materials.Length;
            }
        }

        public static int GetMonstersInRange(Vector3 position, float range, Monster[] monsters, Monster exclusiveMonster = null) {
            int index = 0;
            for (int i = 0; i < s_aliveMonsters.Count; i++) {
                var monster = s_aliveMonsters[i];
                if (monster == exclusiveMonster) continue;
                var monsterPos = monster.transform.position;
                monsterPos.y = position.y;

                var sqrDistance = Vector3.SqrMagnitude(position - monsterPos);
                var radius = monster.MyData.Radius + range;
                if (sqrDistance < radius * radius) {
                    monsters[index] = monster;
                    index++;
                    if (index == monsters.Length)
                        break;
                }
            }
            return index;
        }

        public static int GetMonstersInRangeWithAngle(Vector3 position, float range, Vector3 direction, float angle, Monster[] monsters, Monster exclusiveMonster = null) {
            int index = 0;
            for (int i = 0; i < s_aliveMonsters.Count; i++) {
                var monster = s_aliveMonsters[i];
                if (monster == exclusiveMonster) continue;
                var monsterPos = monster.transform.position;
                monsterPos.y = position.y;

                if (Vector3.Angle(monsterPos - position, direction) > angle / 2) continue;

                var sqrDistance = Vector3.SqrMagnitude(position - monsterPos);
                var radius = monster.MyData.Radius + range;
                if (sqrDistance < radius * radius) {
                    monsters[index] = monster;
                    index++;
                    if (index == monsters.Length)
                        break;
                }
            }
            return index;
        }

        public static bool TryGetMonsterByIdx(int idx, out Monster monster) {
            return s_idxToMonsterMapping.TryGetValue(idx, out monster);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            var position = transform.position + (Vector3)BattleManager.MonsterCollisionPosOffset;
            Gizmos.DrawWireSphere(position, MyData.Radius);
            UnityEditor.Handles.Label(position, $"id {MonsterID}\nidx {MonsterIdx}");
        }
#endif
    }

}
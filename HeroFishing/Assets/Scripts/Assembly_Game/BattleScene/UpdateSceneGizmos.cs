using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeroFishing.Socket.Matchgame;
using HeroFishing.Main;
using HeroFishing.Battle;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UpdateSceneGizmos : MonoBehaviour {
    private static UpdateSceneGizmos _instance;
    public static UpdateSceneGizmos Instance => _instance;

    //[SerializeField]
    //private float _testAngle;

    private Spawn[] _spawns;
    private SceneEffect[] _effects;
    private void Start() {
        _instance = this;
    }

    public void SetUpdateScene(UPDATESCENE_TOCLIENT packet) {
        _spawns = packet.Spawns;
        _effects = packet.SceneEffects;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (_spawns == null || _spawns.Length == 0) return;
        for (int i = 0; i < _spawns.Length; i++) {
            var spawn = _spawns[i];
            var routeData = RouteJsonData.GetData(spawn.RID);
            for (int j = 0; j < spawn.Ms.Length; j++) {
                var monster = spawn.Ms[j];
                if (monster.Death) {
                    Gizmos.color = Color.red;
                }
                else {
                    Gizmos.color = Color.green;
                }
                var monsterData = MonsterJsonData.GetData(monster.ID);
                var frozenTime = GetFrozenTime((float)spawn.STime, GameTime.Current);
                var deltaTime = GameTime.Current - spawn.STime - frozenTime;
                var rotation = Quaternion.AngleAxis(BattleManager.Instance.Index * 90f, Vector3.up);
                var direction = (routeData.TargetPos - routeData.SpawnPos).normalized;
                var position = rotation * (routeData.SpawnPos + (float)deltaTime * monsterData.Speed * direction);
                //Debug.Log($"gizmo {monster.Idx}: deltaTime: {deltaTime} deltaPosition: {position}");
                Gizmos.DrawSphere(position, monsterData.Radius);
            }
        }
    }

    private float GetFrozenTime(float startTime, float currentTime) {
        float totalTime = 0;
        if (_effects == null) return 0;
        for (int i = 0; i < _effects.Length; i++) {
            var effect = _effects[i];
            // �O�_��Ǫ����ɶ����| (�Ǫ����}�l�ɶ���ĪG�������ɶ����N�����|�C���M�o���p�q�`�|��ĪG���}�l�ɶ��ߡA�]���B��ɤ��|���Ǫ��C)
            if (startTime < effect.AtTime + effect.Duration) {
                // �p�G���O�Ĥ@��effect�A�h��e�@��effect
                if (i > 0) {
                    var prevEffect = _effects[i - 1];
                    // �p�G��e�@��effect���|�ɶ�
                    if (effect.AtTime < prevEffect.AtTime + prevEffect.Duration) {
                        var sTime = Mathf.Max((float)(prevEffect.AtTime + prevEffect.Duration), startTime);
                        var eTime = Mathf.Min((float)(effect.AtTime + effect.Duration), currentTime);
                        totalTime += eTime - sTime;
                    }
                    // �S�����ܴN�@�몺���
                    else {
                        var sTime = Mathf.Max((float)effect.AtTime, startTime);
                        var eTime = Mathf.Min((float)(effect.AtTime + effect.Duration), currentTime);
                        totalTime += eTime - sTime;
                    }
                }
                else {
                    var sTime = Mathf.Max((float)effect.AtTime, startTime);
                    var eTime = Mathf.Min((float)(effect.AtTime + effect.Duration), currentTime);
                    totalTime += eTime - sTime;
                }
            }
        }
        return totalTime;
    }
#endif
}

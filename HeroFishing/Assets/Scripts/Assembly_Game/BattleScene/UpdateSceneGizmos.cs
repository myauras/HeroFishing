using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeroFishing.Socket.Matchgame;
using HeroFishing.Main;
using HeroFishing.Battle;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UpdateSceneGizmos : MonoBehaviour
{
    private static UpdateSceneGizmos _instance;
    public static UpdateSceneGizmos Instance => _instance;

    //[SerializeField]
    //private float _testAngle;

    private Spawn[] _spawns;
    private void Start() {
        _instance = this;
    }

    public void SetUpdateScene(UPDATESCENE_TOCLIENT packet) {
        _spawns = packet.Spawns;        
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if(_spawns == null || _spawns.Length == 0) return;
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
                var deltaTime = GameTime.LastestOverride - spawn.STime;
                var rotation = Quaternion.AngleAxis(BattleManager.Instance.Index * 90f, Vector3.up);
                var direction = (routeData.TargetPos - routeData.SpawnPos).normalized;
                var position = rotation * (routeData.SpawnPos + (float)deltaTime * monsterData.Speed * direction);
                Gizmos.DrawSphere(position, monsterData.Radius);
            }
        }
    }
#endif
}

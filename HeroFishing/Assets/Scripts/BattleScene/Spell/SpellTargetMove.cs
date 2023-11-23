using Cysharp.Threading.Tasks;
using HeroFishing.Battle;
using HeroFishing.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellTargetMove : SpellMoveBase {
    private float _delay;
    private float _moveTime;
    private float _jumpHeight;
    private float _backTime;

    private HeroSpellJsonData _data;
    public SpellTargetMove(HeroSpellJsonData data) {
        _data = data;

        var values = _data.MoveTypeValues;
        _delay = float.Parse(values[0]);
        _moveTime = float.Parse(values[1]);
        _jumpHeight = float.Parse(values[2]);
        _backTime = float.Parse(values[3]);

    }

    public override void Play(Vector3 position, Vector3 heroPosition, Vector3 direction, HeroMoveBehaviour moveBehaviour) {
        moveBehaviour.BeginMoving(_delay, _moveTime, _jumpHeight, position, _backTime);
    }
}

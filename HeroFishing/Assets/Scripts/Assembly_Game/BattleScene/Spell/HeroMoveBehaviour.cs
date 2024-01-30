using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMoveBehaviour : MonoBehaviour {
    private float _delay;
    private float _height;
    private float _moveTime;
    private float _backTime;
    private float _a, _b;

    private float _startTime;
    private float _startMovingTime;
    private Vector3 _initPosition;
    private Vector3 _targetPosition;
    private Quaternion _initRotation;

    private Transform _moveTarget;

    private bool _isMoving;
    public bool IsMoving => _isMoving;

    private bool _isMovingBack;
    public void BeginMoving(float delay, float moveTime, float height, Vector3 targetPosition, float backTime) {
        if (_moveTarget == null) {
            _moveTarget = transform.GetChild(0);
            if (_moveTarget == null)
                throw new System.Exception("move target is null");
        }

        _delay = delay;
        _moveTime = moveTime;
        _backTime = backTime;
        //var distance = (targetPosition - transform.position).magnitude;
        _height = height;
        GetABValue();
        _initPosition = _moveTarget.position;
        _targetPosition = targetPosition;
        _initRotation = _moveTarget.rotation;
        _startTime = Time.time;
        _startMovingTime = Time.time + _delay;

        _isMoving = true;
    }

    private void GetABValue() {
        // ax^2 + bx = y
        // x => 0, y => 0
        // x => 0.5, y => _height
        // x => 1, y => 0
        // 求得 a = -4 * _height，b = 4 * _height
        _a = -4 * _height;
        _b = 4 * _height;
    }

    private void Update() {
        float currentTime = Time.time;
        float deltaTime = Time.deltaTime;
        if (_isMoving) {
            if (!_isMovingBack) {

                // 時間小於delay，不做動作
                if (currentTime - _startTime < _delay) return;

                // 將經過的時間標準化，開始為0，結束為1
                var normalizedTime = (currentTime - _startMovingTime) / _moveTime;
                // 如果不做跳躍，直接lerp起始位置與終點位置
                if (_height == 0) {
                    _moveTarget.position = Vector3.Lerp(_initPosition, _targetPosition, normalizedTime);
                }
                // 需要跳躍，位置的y帶入 ax^2 + bx
                else {
                    var position = Vector3.Lerp(_initPosition, _targetPosition, normalizedTime);
                    position.y = _a * normalizedTime * normalizedTime + _b * normalizedTime;
                    _moveTarget.position = position;
                    //Debug.Log("position " + position);
                }

                // 超過移動時間，重置位置，並且往回頭走
                if (currentTime - _startMovingTime >= _moveTime) {
                    _isMovingBack = true;
                    _moveTarget.position = _targetPosition;
                    //Debug.Log("finish");
                }
            }
            else {
                // 還沒旋轉到定位前，先不移動
                if (_moveTarget.rotation != Quaternion.Euler(0, 180, 0) * _initRotation) {
                    _moveTarget.rotation = Quaternion.RotateTowards(_moveTarget.rotation, Quaternion.Euler(0, 180, 0) * _initRotation, 600 * deltaTime);
                    if (_moveTarget.rotation == Quaternion.Euler(0, 180, 0) * _initRotation) {
                        _startMovingTime = currentTime;
                    }
                }
                // 已經旋轉定位，回到原位
                else {
                    var normalizedTime = (currentTime - _startMovingTime) / _backTime;
                    _moveTarget.position = Vector3.Lerp(_targetPosition, _initPosition, normalizedTime);

                    if (currentTime - _startMovingTime >= _backTime) {
                        _isMovingBack = false;
                        _isMoving = false;
                        _moveTarget.position = _initPosition;
                        _moveTarget.rotation = _initRotation;
                    }
                }

            }
        }
    }
}

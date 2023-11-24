using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMoveBehaviour : MonoBehaviour {
    private float _delay;
    private float _vHeight;
    private float _moveTime;
    private float _backTime;
    private float _a, _b;

    private float _startTime;
    private float _startMovingTime;
    private Vector3 _initPosition;
    private Vector3 _targetPosition;
    private Quaternion _initRotation;

    private bool _isMoving;
    public bool IsMoving => _isMoving;

    private bool _isMovingBack;
    public void BeginMoving(float delay, float moveTime, float vHeight, Vector3 targetPosition, float backTime) {
        _delay = delay;
        _moveTime = moveTime;
        _backTime = backTime;
        //var distance = (targetPosition - transform.position).magnitude;
        _vHeight = vHeight;
        GetABValue();
        _initPosition = transform.position;
        _targetPosition = targetPosition;
        _initRotation = transform.rotation;
        _startTime = Time.time;
        _startMovingTime = Time.time + _delay;

        _isMoving = true;
    }

    private void GetABValue() {
        // ax^2+bx = y
        // x => 0, y => 0
        // x => 0.5, y => _yHeight
        // x => 1, y => 0
        // 求得 a = -4y，b = 4y
        _a = -4 * _vHeight;
        _b = 4 * _vHeight;
    }

    private void Update() {
        if (_isMoving) {
            if (!_isMovingBack) {
                // 時間小於delay，不做動作
                if (Time.time - _startTime < _delay) return;

                var deltaDuration = (Time.time - _startMovingTime) / _moveTime;
                // 如果不做跳躍
                if (_vHeight == 0) {
                    transform.position = Vector3.Lerp(_initPosition, _targetPosition, deltaDuration);
                }
                else {
                    var position = Vector3.Lerp(_initPosition, _targetPosition, deltaDuration);
                    position.y = _a * deltaDuration * deltaDuration + _b * deltaDuration;
                    transform.position = position;
                    //Debug.Log("position " + position);
                }

                if (Time.time - _startMovingTime >= _moveTime) {
                    _isMovingBack = true;
                    transform.position = _targetPosition;
                    _startMovingTime = Time.time;
                    //Debug.Log("finish");
                }
            }
            else {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 180, 0) * _initRotation, 600 * Time.deltaTime);

                var deltaDuration = (Time.time - _startMovingTime) / _backTime;
                transform.position = Vector3.Lerp(_targetPosition, _initPosition, deltaDuration);

                if (Time.time - _startMovingTime >= _backTime) {
                    _isMovingBack = false;
                    _isMoving = false;
                    transform.position = _initPosition;
                }

            }
        }
    }
}

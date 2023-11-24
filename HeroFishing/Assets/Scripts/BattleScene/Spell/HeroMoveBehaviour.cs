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

    private bool _isMoving;
    public bool IsMoving => _isMoving;

    private bool _isMovingBack;
    public void BeginMoving(float delay, float moveTime, float height, Vector3 targetPosition, float backTime) {
        _delay = delay;
        _moveTime = moveTime;
        _backTime = backTime;
        //var distance = (targetPosition - transform.position).magnitude;
        _height = height;
        GetABValue();
        _initPosition = transform.position;
        _targetPosition = targetPosition;
        _initRotation = transform.rotation;
        _startTime = Time.time;
        _startMovingTime = Time.time + _delay;

        _isMoving = true;
    }

    private void GetABValue() {
        // ax^2 + bx = y
        // x => 0, y => 0
        // x => 0.5, y => _height
        // x => 1, y => 0
        // �D�o a = -4 * _height�Ab = 4 * _height
        _a = -4 * _height;
        _b = 4 * _height;
    }

    private void Update() {
        float currentTime = Time.time;
        float deltaTime = Time.deltaTime;
        if (_isMoving) {
            if (!_isMovingBack) {
               
                // �ɶ��p��delay�A�����ʧ@
                if (currentTime - _startTime < _delay) return;

                // �N�g�L���ɶ��зǤơA�}�l��0�A������1
                var normalizedTime = (currentTime - _startMovingTime) / _moveTime;
                // �p�G�������D�A����lerp�_�l��m�P���I��m
                if (_height == 0) {
                    transform.position = Vector3.Lerp(_initPosition, _targetPosition, normalizedTime);
                }
                // �ݭn���D�A��m��y�a�J ax^2 + bx
                else {
                    var position = Vector3.Lerp(_initPosition, _targetPosition, normalizedTime);
                    position.y = _a * normalizedTime * normalizedTime + _b * normalizedTime;
                    transform.position = position;
                    //Debug.Log("position " + position);
                }

                // �W�L���ʮɶ��A���m��m�A�åB���^�Y��
                if (currentTime - _startMovingTime >= _moveTime) {
                    _isMovingBack = true;
                    transform.position = _targetPosition;
                    _startMovingTime = currentTime;
                    //Debug.Log("finish");
                }
            }
            else {
                // �٨S�����w��e�A��������
                if (transform.rotation != Quaternion.Euler(0, 180, 0) * _initRotation)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 180, 0) * _initRotation, 600 * deltaTime);
                // �w�g����w��A�^����
                else {
                    var normalizedTime = (currentTime - _startMovingTime) / _backTime;
                    transform.position = Vector3.Lerp(_targetPosition, _initPosition, normalizedTime);

                    if (currentTime - _startMovingTime >= _backTime) {
                        _isMovingBack = false;
                        _isMoving = false;
                        transform.position = _initPosition;
                    }
                }

            }
        }
    }
}

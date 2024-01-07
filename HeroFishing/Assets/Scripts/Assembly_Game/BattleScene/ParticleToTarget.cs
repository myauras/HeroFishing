using HeroFishing.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleToTarget : MonoBehaviour {
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private ParticleSystem _particleSys;
    [SerializeField, Range(0, 1)]
    private float _delay;
    [SerializeField]
    private AnimationCurve _moveCurve;
    [SerializeField]
    private float _speed = 15f;
    [SerializeField]
    private float _removeDistance = 1;
    private ParticleSystem.Particle[] _particles = new ParticleSystem.Particle[800];
    private Vector3 _targetPos;

    private void Start() {
        _target = BattleManager.Instance.GetHero(0).transform;
        _targetPos = _target.position;
        //Debug.Log(_target.position);
        _targetPos.y += 1;
        //Debug.Log(_particleSys.main.simulationSpace.ToString());
    }

    private void LateUpdate() {
        UpdateParticleWithWaitTime();
        //UpdateParticleWithoutWaitTime();
    }

    private void UpdateParticleWithoutWaitTime() {
        int length = _particleSys.GetParticles(_particles);
        for (int i = 0; i < length; i++) {
            var particle = _particles[i];

            var normalizedLifeTime = 1 - (particle.remainingLifetime / particle.startLifetime);
            if (normalizedLifeTime <= _delay) {
                Vector3 deltaPos = _targetPos - _particles[i].position;
                //var velocity = deltaPos / particle.remainingLifetime;
                var velocity = deltaPos.normalized * _speed * Time.deltaTime;
                particle.velocity += velocity;
            }
            else {
                var value = _moveCurve.Evaluate((normalizedLifeTime - _delay) / (1 - _delay));
                particle.position = Vector3.Lerp(particle.position, _targetPos, value);
            }

            if (Vector3.SqrMagnitude(particle.position - _targetPos) < _removeDistance * _removeDistance) {
                particle.remainingLifetime = -1;
            }
            //Debug.Log($"index: {i} remaining time: {particle.remainingLifetime} start time: {particle.startLifetime}");
            _particles[i] = particle;
        }
        _particleSys.SetParticles(_particles, length);
    }

    private void UpdateParticleWithWaitTime() {
        if (_particleSys.time < 1.0f) return;
        int length = _particleSys.GetParticles(_particles);
        for (int i = 0; i < length; i++) {
            var particle = _particles[i];

            var normalizedLifeTime = 1 - (particle.remainingLifetime / particle.startLifetime);
            if (normalizedLifeTime > _delay) {
                var value = _moveCurve.Evaluate((normalizedLifeTime - _delay) / (1 - _delay));
                particle.position = Vector3.Lerp(particle.position, _targetPos, value);
            }

            if (Vector3.SqrMagnitude(particle.position - _targetPos) < _removeDistance * _removeDistance) {
                particle.remainingLifetime = -1;
            }
            //Debug.Log($"index: {i} remaining time: {particle.remainingLifetime} start time: {particle.startLifetime}");
            _particles[i] = particle;
        }
        _particleSys.SetParticles(_particles, length);
    }
}

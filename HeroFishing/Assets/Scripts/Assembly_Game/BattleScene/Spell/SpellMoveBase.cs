using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellMoveBase {
    public abstract void Play(Vector3 position, Vector3 heroPosition, Vector3 direction, HeroMoveBehaviour moveBehaviour);
}

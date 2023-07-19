using HeroFishing.Battlefield;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AttackTrigger {
    public enum AttackState {
        StopAttacking,
        Ready,
        Attacking,
    }
    public static AttackData MyAttackData;
    public static AttackState MyAttackState= AttackState.Ready;

    public static void Attack(AttackData _data) {
        if (MyAttackState != AttackState.Ready) return;
        MyAttackData = _data;
        MyAttackState = AttackState.Attacking;
    }
    public static void EndAttack() {
        MyAttackState = AttackState.Ready;
    }
}

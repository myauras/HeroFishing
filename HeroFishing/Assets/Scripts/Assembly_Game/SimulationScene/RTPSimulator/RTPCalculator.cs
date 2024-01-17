using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTPCalculator {
    public static decimal GetKP(decimal _attackRTP, decimal _targetOdds) {
        return _attackRTP / _targetOdds;
    }

    public static decimal GetFreeAmmoDropP(decimal _rtp, decimal _attackRTP, decimal _skillRTP, decimal _targetOdds) {

        decimal freeAmmoDropP = ((_rtp - _attackRTP) / (_skillRTP - _attackRTP)) / (_rtp / _targetOdds);

        return freeAmmoDropP;
    }
}

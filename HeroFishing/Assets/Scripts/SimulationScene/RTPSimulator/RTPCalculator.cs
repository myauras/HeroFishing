using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTPCalculator {
<<<<<<< HEAD
    public static decimal GetKP(decimal _attackRTP, decimal _targetOdds) {
        return _attackRTP / _targetOdds;
    }

    public static decimal GetFreeAmmoDropP(decimal _rtp, decimal _attackRTP, decimal _skillRTP, decimal _targetOdds) {

        decimal freeAmmoDropP = ((_rtp - _attackRTP) / (_skillRTP - _attackRTP)) / (_rtp / _targetOdds);
=======
    public static decimal GetKP(decimal _rtp, decimal _givenRTP, decimal _targetOdds) {
        return (_rtp - _givenRTP) / _targetOdds;
    }
    public static Tuple<double, double> GetKPAndDP(double _rtp, double _givenDropProb, double _dropRTP, int _o) {
        double killProb = (_rtp - _givenDropProb) / _o;
        double dropProb = (_givenDropProb / _o) / (_dropRTP / _rtp) / killProb;
        return new Tuple<double, double>(killProb, dropProb);
    }

    public static decimal GetFreeAmmoDropP(decimal _rtp, decimal _givenRTP, decimal _freeAmmoRTP, decimal _targetOdds) {

        //先求出免費子彈佔所有子彈比例
        decimal freeAmmoPercentage = _givenRTP / (_freeAmmoRTP - _rtp + _givenRTP);
        Debug.LogError("freeAmmoPercentage=" + freeAmmoPercentage);

        //付費子彈擊殺魚的次數，我們可以假設付費子彈的數量為1
        decimal paidBulletKillCount = 1 * (_rtp / _targetOdds);

        //付費子彈擊殺魚時掉落免費子彈的機率
        decimal freeAmmoDropP = freeAmmoPercentage / paidBulletKillCount;
        Debug.LogError("freeAmmoDropP=" + freeAmmoDropP);
>>>>>>> 32cccfb5b31911efd229849f70e346c4d83643bb

        return freeAmmoDropP;
    }
}

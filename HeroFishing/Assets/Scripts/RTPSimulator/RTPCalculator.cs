using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTPCalculator 
{
    public static double GetKP(double _rtp, int _o) {
        return _rtp / _o;
    }
    public static Tuple<double,double> GetKPAndDP(double _rtp,double _givenDropProb,double _dropRTP, int _o) {
        //Debug.LogError("_rtp=" + _rtp);
        //Debug.LogError("_givenDropProb=" + _givenDropProb);
        //Debug.LogError("_dropRTP=" + _dropRTP);
        //Debug.LogError("_o=" + _o);
        double killProb = (_rtp - _givenDropProb) / _o;
        double dropProb = (_givenDropProb / _o) / (_dropRTP / _rtp)/ killProb;
        return new Tuple<double,double>(killProb, dropProb);
    }
}

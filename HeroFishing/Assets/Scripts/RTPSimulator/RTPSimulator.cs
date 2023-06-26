using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTPSimulator : MonoBehaviour
{
    [SerializeField]
    double AmmoRTP;
    [SerializeField]
    int TargetOdds;
    [SerializeField]
    int Balance = 10000;
    [SerializeField]
    double GivenDropProb=0.2d;
    [SerializeField]
    double DropRTP = 1.2d;

    int SkillCount = 0;

    int OriginalBalance = 0;
    int PlayTimes = 0;
    private void Start() {
        OriginalBalance = Balance;
    }

    public void Update() {
        if (Input.GetKeyDown("q")) {
            Hit();
        }
    }
    void Hit() {
        Tuple<double,double> kpdp= RTPCalculator.GetKPAndDP(AmmoRTP,GivenDropProb, DropRTP,TargetOdds);
        Debug.Log("擊殺率為:" + kpdp.Item1);
        Debug.Log("掉落充能機率為:" + kpdp.Item2);
        System.Random random = new System.Random();
        for(int i = 0; i < 100; i++) {
            Balance--;
            PlayTimes++;
            var killCheck = random.NextDouble();

            if (killCheck < kpdp.Item1) {
                Balance += TargetOdds;
                Debug.LogError("擊殺!");
                var dropCheck = random.NextDouble();
                if (dropCheck < kpdp.Item2) {
                    SkillCount++;
                    Debug.LogError("掉落!");
                }
            } else {
            }
        }
        Debug.LogError("PlayerTimes="+ PlayTimes+ " Balance=" + Balance);

    }
}

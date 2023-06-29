using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTPSimulator : MonoBehaviour {
    [SerializeField]
    double GameRTP = 0.95d;
    [SerializeField]
    double GivenRTP = 0.05d;
    [SerializeField]
    int TargetOdds = 100;
    [SerializeField]
    double ChargeRate = 0.1d;
    [SerializeField]
    double FreeAmmoRTP = 10d;
    [SerializeField]
    int PlayTimes = 0;
    [SerializeField]
    int Balance = 10000;

    int PlayCount = 0;
    int OriginalBalance = 0;
    double CurCharge = 0d;
    private void Start() {
        OriginalBalance = Balance;
    }

    public void Update() {
        if (Input.GetKeyDown("q")) {
            Hit();
        }
    }
    void Hit() {

        System.Random random = new System.Random();
        decimal kp = RTPCalculator.GetKP((decimal)GameRTP, (decimal)GivenRTP, TargetOdds);
        decimal freeAmmoDropP = RTPCalculator.GetFreeAmmoDropP((decimal)GameRTP, (decimal)GivenRTP, (decimal)FreeAmmoRTP, TargetOdds);
        freeAmmoDropP = freeAmmoDropP / (decimal)ChargeRate;
        if (freeAmmoDropP >= 1) {
            Debug.LogError("免費子彈掉落率不可>=1 這樣代表玩家實際RTP被吃掉 freeAmmoDropP=" + freeAmmoDropP);
        }
        Debug.LogError("目標魚賠率=" + TargetOdds);
        Debug.LogError("免費子彈掉落率=" + freeAmmoDropP);
        Debug.LogError("一般子彈RTP=" + (GameRTP - GivenRTP));
        Debug.LogError("免費子彈RTP=" + FreeAmmoRTP);

        for (int i = 0; i < PlayTimes; i++) {
            PlayCount++;
            Balance--;//一般攻擊會消耗金幣
            var killCheck = random.NextDouble();
            if ((decimal)killCheck < kp) {//擊殺
                Balance += TargetOdds;
                //Debug.LogError("擊殺!");
                var dropCheck = random.NextDouble();
                if ((decimal)dropCheck < freeAmmoDropP) {//掉落
                    CurCharge += ChargeRate;
                    if (CurCharge >= 1) {
                        SkillAttack();
                        CurCharge = 0;
                    }

                }
            } else {
            }
        }
        decimal espectRTP = 1 - (((decimal)OriginalBalance - (decimal)Balance) / (decimal)PlayCount);
        Debug.LogError("模擬次數=" + PlayCount / 10000 + "萬次");
        Debug.LogError("跑模擬後反推RTP=" + espectRTP);
    }
    void SkillAttack() {
        decimal kp = RTPCalculator.GetKP((decimal)FreeAmmoRTP, 0, TargetOdds);
        if (kp >= 1) {
            Debug.LogError("免費子彈對目標的擊殺率不可以大於>=1 這樣代表玩家實際RTP被吃掉 kp=" + kp);
        }
        System.Random random = new System.Random();
        var killCheck = random.NextDouble();
        if ((decimal)killCheck < kp) {//擊殺
            Balance += TargetOdds;
        } else {
        }
    }
}

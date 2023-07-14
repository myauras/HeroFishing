using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTPSimulator : MonoBehaviour {
    [SerializeField]
    double GameRTP = 0.95d;
    [SerializeField]
<<<<<<< HEAD
    double AttackRTP = 0.945d;
    [SerializeField]
    int TargetOdds = 100;
    [SerializeField]
    double Energy = 10;
    [SerializeField]
    double SkillRTP = 10d;
=======
    double GivenRTP = 0.05d;
    [SerializeField]
    int TargetOdds = 100;
    [SerializeField]
    double ChargeRate = 0.1d;
    [SerializeField]
    double FreeAmmoRTP = 10d;
>>>>>>> 32cccfb5b31911efd229849f70e346c4d83643bb
    [SerializeField]
    int PlayTimes = 0;
    [SerializeField]
    int Balance = 10000;
<<<<<<< HEAD
    [SerializeField]
    int BuffAddRTP = 1;
    [SerializeField]
    int BuffAttackTimes = 30;
=======
>>>>>>> 32cccfb5b31911efd229849f70e346c4d83643bb

    int PlayCount = 0;
    int OriginalBalance = 0;
    double CurCharge = 0d;
    private void Start() {
        OriginalBalance = Balance;
    }

    public void Update() {
        if (Input.GetKeyDown("q")) {
            Hit();
<<<<<<< HEAD
        } else if (Input.GetKeyDown("w")) {
            Hit2();
        }
    }


    void Hit2() {
        System.Random random = new System.Random();
        int totalOdds = TargetOdds + (BuffAddRTP * BuffAttackTimes);
        Debug.Log("totalOdds=" + totalOdds);
        int curBuffCount = 0;
        for (int i = 0; i < PlayTimes; i++) {
            decimal kp = 0;
            decimal freeAmmoDropP = 0;
            if (curBuffCount <= 0) {
                kp = RTPCalculator.GetKP((decimal)AttackRTP, totalOdds);
                freeAmmoDropP = RTPCalculator.GetFreeAmmoDropP((decimal)GameRTP, (decimal)AttackRTP, (decimal)SkillRTP, totalOdds);
            } else {
                kp = RTPCalculator.GetKP((decimal)(AttackRTP + BuffAddRTP), totalOdds);
                freeAmmoDropP = RTPCalculator.GetFreeAmmoDropP((decimal)GameRTP, (decimal)AttackRTP, (decimal)SkillRTP, totalOdds);
                curBuffCount--;
            }
            freeAmmoDropP = freeAmmoDropP * (decimal)Energy;
            if (freeAmmoDropP >= 1) {
                Debug.LogError("免費子彈掉落率不可>=1 這樣代表玩家實際RTP被吃掉 freeAmmoDropP=" + freeAmmoDropP);
            }
            PlayCount++;
            Balance--;//一般攻擊會消耗金幣
            var killCheck = random.NextDouble();
            if ((decimal)killCheck < kp) {//擊殺
                Balance += TargetOdds;
                curBuffCount += BuffAttackTimes;
                //Debug.LogError("擊殺!");
                var dropCheck = random.NextDouble();
                if ((decimal)dropCheck < freeAmmoDropP) {//掉落
                    CurCharge += 1;
                    if (CurCharge >= Energy) {
                        SkillAttack2();
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
    void SkillAttack2() {
        int totalOdds = TargetOdds + (BuffAddRTP * BuffAttackTimes);
        decimal kp = RTPCalculator.GetKP((decimal)SkillRTP, totalOdds);
        if (kp >= 1) {
            Debug.LogError("免費子彈對目標的擊殺率不可以大於>=1 這樣代表玩家實際RTP被吃掉 kp=" + kp);
        }
        System.Random random = new System.Random();
        var killCheck = random.NextDouble();
        if ((decimal)killCheck < kp) {//擊殺
            Balance += TargetOdds;
        } else {
=======
>>>>>>> 32cccfb5b31911efd229849f70e346c4d83643bb
        }
    }
    void Hit() {

        System.Random random = new System.Random();
<<<<<<< HEAD
        decimal kp = RTPCalculator.GetKP((decimal)AttackRTP, TargetOdds);
        decimal freeAmmoDropP = RTPCalculator.GetFreeAmmoDropP((decimal)GameRTP, (decimal)AttackRTP, (decimal)SkillRTP, TargetOdds);
        freeAmmoDropP = freeAmmoDropP * (decimal)Energy;
=======
        decimal kp = RTPCalculator.GetKP((decimal)GameRTP, (decimal)GivenRTP, TargetOdds);
        decimal freeAmmoDropP = RTPCalculator.GetFreeAmmoDropP((decimal)GameRTP, (decimal)GivenRTP, (decimal)FreeAmmoRTP, TargetOdds);
        freeAmmoDropP = freeAmmoDropP / (decimal)ChargeRate;
>>>>>>> 32cccfb5b31911efd229849f70e346c4d83643bb
        if (freeAmmoDropP >= 1) {
            Debug.LogError("免費子彈掉落率不可>=1 這樣代表玩家實際RTP被吃掉 freeAmmoDropP=" + freeAmmoDropP);
        }
        Debug.LogError("目標魚賠率=" + TargetOdds);
        Debug.LogError("免費子彈掉落率=" + freeAmmoDropP);
<<<<<<< HEAD
        Debug.LogError("一般子彈RTP=" + AttackRTP);
        Debug.LogError("免費子彈RTP=" + SkillRTP);


=======
        Debug.LogError("一般子彈RTP=" + (GameRTP - GivenRTP));
        Debug.LogError("免費子彈RTP=" + FreeAmmoRTP);
>>>>>>> 32cccfb5b31911efd229849f70e346c4d83643bb

        for (int i = 0; i < PlayTimes; i++) {
            PlayCount++;
            Balance--;//一般攻擊會消耗金幣
            var killCheck = random.NextDouble();
            if ((decimal)killCheck < kp) {//擊殺
                Balance += TargetOdds;
                //Debug.LogError("擊殺!");
                var dropCheck = random.NextDouble();
                if ((decimal)dropCheck < freeAmmoDropP) {//掉落
<<<<<<< HEAD
                    CurCharge += 1;
                    if (CurCharge >= Energy) {
=======
                    CurCharge += ChargeRate;
                    if (CurCharge >= 1) {
>>>>>>> 32cccfb5b31911efd229849f70e346c4d83643bb
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
<<<<<<< HEAD
        decimal kp = RTPCalculator.GetKP((decimal)SkillRTP, TargetOdds);
=======
        decimal kp = RTPCalculator.GetKP((decimal)FreeAmmoRTP, 0, TargetOdds);
>>>>>>> 32cccfb5b31911efd229849f70e346c4d83643bb
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

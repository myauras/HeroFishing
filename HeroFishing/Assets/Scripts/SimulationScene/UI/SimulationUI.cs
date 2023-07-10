using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationUI : MonoBehaviour {
    public void OnAttackBtnClick() {
        SimulationSceneManager.Instance.MyHero.PlayAttackMotion();
    }
    public void OnSpellClick(string _name) {
        SimulationSceneManager.Instance.MyHero.SetAniTrigger(_name);
    }
}

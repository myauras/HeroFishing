using HeroFishing.Battle;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropUI : BaseUI
{
    [SerializeField]
    private DropBtn[] _dropBtns;

    public override void RefreshText() {
        
    }

    public override void Init() {
        base.Init();

        for (int i = 0; i < _dropBtns.Length; i++) {
            _dropBtns[i].Init();
        }
    }

    public void OnDropAdd(int dropID) {
        for (int i = 0; i < _dropBtns.Length; i++) {
            var dropBtn = _dropBtns[i];
            if (dropBtn.IsEmpty) {
                dropBtn.SetContent(dropID);
                break;
            }                
        }
    }

    public void OnDropPlay(int dropID, float duration = 0) {
        for (int i = 0; i < _dropBtns.Length; i++) {
            if(_dropBtns[i].DropID == dropID) {
                _dropBtns[i].Play(duration);
                break;
            }
        }
    }

    public void Press(int index) {
        if (_dropBtns[index].IsEmpty) return;
        _dropBtns[index].Press();
        DropManager.Instance.PlayDrop(_dropBtns[index].DropID);
        //Debug.Log("press");
    }

    public void Drag() {
        //Debug.Log("drag");
    }

    public void Release() {
        //Debug.Log("release");
    }
}

using HeroFishing.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCircle : DropSpellBase {
    public DropCircle(DropJsonData data, DropSpellJsonData spellData) : base(data, spellData) {
    }

    public override bool PlayDrop() {
        Debug.Log("play circle");
        return true;
    }
}

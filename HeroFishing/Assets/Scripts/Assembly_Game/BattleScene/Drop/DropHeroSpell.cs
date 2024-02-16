using HeroFishing.Battle;
using HeroFishing.Main;
using HeroFishing.Socket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropHeroSpell : DropSpellBase {
    private float _radius;
    private Monster[] _monsters = new Monster[100];
    private const string HERO_SPELL_ID = "{0}_drop_spell";
    public DropHeroSpell(DropJsonData data, DropSpellJsonData spellData) : base(data, spellData) {
        _radius = spellData.EffectValue1;
    }

    public override bool PlayDrop(int heroIndex) {
        Debug.Log("play hero spell");
        if (heroIndex == 0)
            _dropUI.OnDropPlay(_data.ID);
        string heroSpellID = string.Format(HERO_SPELL_ID, _data.ID);
        var heroSpellData = HeroSpellJsonData.GetData(heroSpellID);
        if (heroSpellData == null) {
            Debug.LogError("no hero spell data");
            return false;
        }
        //Hero hero = BattleManager.Instance.GetHero(heroIndex);
        int attackID = heroIndex == 0 ? PlayerAttackController.AttackID : 0;
        Debug.Log(attackID);
        int count = Monster.GetMonstersInRange(Vector3.zero, _radius, _monsters);
        int[] idxs = new int[count];
        for (int i = 0; i < count; i++) {
            var monster = _monsters[i];
            monster.OnHit(heroSpellID, monster.transform.position);
            idxs[i] = monster.MonsterIdx;

            if (!GameConnector.Connected) {
                float value = Random.value;
                if (value < 0.9f) {
                    monster.Die(heroIndex);
                }
            }
        }

        if (GameConnector.Connected && heroIndex == 0) {
            GameConnector.Instance.Hit(attackID, idxs, heroSpellID);
        }
        //SpellPlayData playData = new SpellPlayData {
        //    attackPos = Vector3.zero,
        //    direction = Vector3.forward,
        //    heroIndex = heroIndex,
        //    IsDrop = true,
        //    heroPos = hero.transform.position,
        //    attackID = attackID,
        //};

        //heroSpellData.Spell.Play(playData);
        return true;
    }
}

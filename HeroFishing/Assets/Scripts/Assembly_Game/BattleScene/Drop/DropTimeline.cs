using HeroFishing.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTimeline : DropSpellBase {
    private DropSpellBase _childDropSpell;
    private TimelinePlayer _player;
    private int _heroIndex;
    public override float Duration => _player == null ? 0 : _player.Duration;
    private const string DROP_TIMELINE = "OtherEffect/DropsSpell_{0:000}/Script_Timeline DropsSpell_{0:000}";
    public DropTimeline(DropJsonData data, DropSpellJsonData spellData) : base(data, spellData) {
    }
    public void SetDropSpell(DropSpellBase dropSpell) {
        _childDropSpell = dropSpell;
    }

    public override bool PlayDrop(int heroIndex) {
        if (_childDropSpell == null) {
            Debug.LogError("timeline child drop is null");
            return false;
        }
        _heroIndex = heroIndex;
        string timelineName = string.Format(DROP_TIMELINE, _spellData.ID);
        PoolManager.Instance.Pop(timelineName, popCallback: OnTimelinePlay);

        return true;
    }

    private void OnTimelinePlay(GameObject go) {
        if (_player == null)
            _player = go.GetComponent<TimelinePlayer>();
        _player.OnTimelineTriggred += OnTimelineTriggered;
    }

    private void OnTimelineTriggered() {
        _player.OnTimelineTriggred -= OnTimelineTriggered;
        _childDropSpell.PlayDrop(_heroIndex);
    }
}

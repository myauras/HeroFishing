using Cinemachine;
using HeroFishing.Battle;
using Scoz.Func;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static PlasticPipe.Server.MonitorStats;

public class TimelinePlayer : MonoBehaviour {
    [SerializeField]
    private PlayableDirector _director;
    [SerializeField]
    private bool _playOnEnable = true;
    [SerializeField]
    private bool _autoBackPoolOnFinish = true;
    [SerializeField]
    private string _cinemachineTrack;
    [SerializeField]
    private string[] _mutedTrack;

    private const string CAMERA_TRACK_NAME = "Cinemachine Track";

    private bool _isInit;
    private bool _isMuted;
    private TimelineAsset _timelineAsset;
    public float Duration => (float)_director.playableAsset.duration;

    private static bool s_isPlaying;
    public static bool IsPlaying => s_isPlaying;

    public event Action OnTimelinePlayed;
    public event Action OnTimelinePaused;
    public event Action OnTimelineStopped;
    public event Action OnTimelineTriggred;

    private void Awake() {
        _director.stopped += OnStopped;
        _director.paused += OnPaused;
        _director.played += OnPlayed;
    }

    private void OnDestroy() {
        _director.stopped -= OnStopped;
        _director.paused -= OnPaused;
        _director.played -= OnPlayed;
    }

    private void OnEnable() {
        if (_playOnEnable) {
            Init();
            Play();
        }
    }

    public void Init() {
        if (_isInit) return;
        _timelineAsset = (TimelineAsset)_director.playableAsset;
        foreach (var output in _director.playableAsset.outputs) {
            if (output.sourceObject != null) {
                if (output.streamName == _cinemachineTrack) {
                    var brain = BattleManager.Instance.BattleCam.GetComponent<CinemachineBrain>();
                    _director.SetGenericBinding(output.sourceObject, brain);
                    break;
                }
            }
        }

    }

    [ContextMenu("Play")]
    public void Play(bool mute = false) {
        if (s_isPlaying) return;
        //gameObject.SetActive(true);
        mute = true;
        //mute = UnityEngine.Random.value > 0.5f;
        if (_isMuted != mute) {
            for (int j = 0; j < _mutedTrack.Length; j++) {
                for (int i = 0; i < _timelineAsset.rootTrackCount; i++) {
                    var track = _timelineAsset.GetRootTrack(i);
                    if (track.name == _mutedTrack[j]) {
                        track.muted = mute;
                        break;
                    }
                }
            }

            _isMuted = mute;
        }

        if (!_isInit) {
            Observable.TimerFrame(1).Subscribe(_ => {
                _director.Play();
            });
            _isInit = true;
        }
        else {
            _director.Play();
        }
    }

    public void Trigger() {
        OnTimelineTriggred?.Invoke();
    }

    private void OnPlayed(PlayableDirector director) {
        Debug.Log("played");
        s_isPlaying = true;
        if (!_isMuted)
            UICam.Instance.MyCam.gameObject.SetActive(false);
        OnTimelinePlayed?.Invoke();
    }

    private void OnPaused(PlayableDirector director) {
        Debug.Log("paused");
        s_isPlaying = false;
        if (!_isMuted)
            UICam.Instance.MyCam.gameObject.SetActive(true);
        OnTimelinePaused?.Invoke();
    }

    private void OnStopped(PlayableDirector director) {
        Debug.Log("stopped");
        s_isPlaying = false;
        if (!_isMuted)
            UICam.Instance.MyCam.gameObject.SetActive(true);
        OnTimelineStopped?.Invoke();
        if (_autoBackPoolOnFinish) {
            PoolManager.Instance.Push(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game_core;

/// <summary>
/// Ground Types.
/// </summary>
public enum GroundTypes
{
    empty = 0,
    grassPine = 1,
    grassTree=2,
    grassFlower=3,
    grassPineAutumn=4,
    grassRock = 5,
    dirtyRock=6,
    snowMountain = 7,
    snowRock = 8,
    water =9,
}


/// <summary>
/// Ground transitions.
/// </summary>
public enum GroundTransitions
{
    NullTransition = 0, // Use this transition to represent a non-existing transition in your system
    init = 1,
    idle=2,
    startingUpdate = 3,
    updating = 4,
    updated = 5,
    destroy = 6,
}

/// <summary>
/// Ground states.
/// </summary>
public enum GroundStates
{
    NullStateID = 0, // Use this ID to represent a non-existing State in your system	
    init = 1,
    idle = 2,
    updateStarted = 3,
    updating= 4,
    updated = 5,
    destroyed = 6,
}
/// <summary>
/// This class controls the ground behaviour.
/// Gets the click/tap event and calls to the gameBehaviour.
/// </summary>
public class groundBehaviour : TouchBehaviour {

    /// <summary>
    /// AUDIO VARIABLES
    /// </summary>
    public  float       lowPitchRange   = .95f;              //The lowest a sound effect will be randomly pitched.
    public  float       highPitchRange  = 1.05f;            //The highest a sound effect will be randomly pitched.
    private AudioSource sfxAdded;

    /// <summary>
    /// CONTROL VARIABLES
    /// </summary>
    public  int playerLevel = 0;
    private int _currentLevel   = 0;
    private int _maxLevel       = 5;
    private float _timeCycle    = 1.0f;//SECS
    public GroundTypes type = GroundTypes.empty;
    private bool OnClickBegan = false;
    public int sortingOrderOffset = 0;
    private bool _sortinfOffsetApplied = false;

    /// <summary>
    /// FINITE STATE MACHINE
    /// </summary>
    private FSMSys<GroundTransitions, GroundStates> _fsm;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    /// <param name="level"></param>
    public override void OnEnable()
    {
    
        initializeValues();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    public override void Update()
    {
        _fsm.CurrentState.Reason();
        _fsm.CurrentState.Act();
    }
    
    /// <summary>
    /// Plays sound effect
    /// </summary>
    public void playSoundEffect(string aSource)
    {
        GameObject pAdded = GameObject.Find(aSource);
        if (pAdded != null)
        {
            sfxAdded = pAdded.GetComponent<AudioSource>();
        }
        else { return; }
        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        sfxAdded.pitch = randomPitch;

        //Play the clip.
        sfxAdded.Play();
    }

    /// <summary>
    /// Used for initialization.
    /// </summary>
    public void initializeValues()
    {
        //LEVEL
        _currentLevel = 0;
        updateLevel(_currentLevel);

        //FSM
        InitFSM();

        //Sorting ORder
        updateSortingOrder();
    }

    /// <summary>
    /// 
    /// </summary>
    public int currentLevel
    {
        get { return _currentLevel; }
        set {
            _currentLevel = value % _maxLevel;
            updateLevel(_currentLevel);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public int maxLevel {
        get { return _maxLevel; }
    }

    /// <summary>
    /// 
    /// </summary>
    public float timeCycle {
        get { return _timeCycle; }
        set { _timeCycle = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    public void updateLevel(int level)
    {
        string currentLevelStr = "level_" + level;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);
            t.gameObject.SetActive(false);
            if (t.name == currentLevelStr)
            {
                t.gameObject.SetActive(true);
            }
        }
    }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="prevValue"></param>
   /// <param name="currentValue"></param>
    public void updateSortingOrder()
    {
        if (!_sortinfOffsetApplied)
        {

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform t = transform.GetChild(i);
                SpriteRenderer sRenderer = t.gameObject.GetComponent<SpriteRenderer>();
                if (sRenderer != null)
                {
                    sRenderer.sortingOrder += sortingOrderOffset;
                }
                for (int j = 0; j < t.childCount; j++)
                {
                    Transform childTransform = t.GetChild(j);
                    sRenderer = childTransform.gameObject.GetComponent<SpriteRenderer>();
                    if (sRenderer != null)
                    {
                        sRenderer.sortingOrder += sortingOrderOffset;
                    }
                }
            }
            _sortinfOffsetApplied = !_sortinfOffsetApplied;
        }
      
    }
    /// <summary>
	/// Raises the touch down event.
	/// </summary>
	public override void OnTouchBegan(Vector3 value)
    {
        OnClickBegan = true;
        SetTransition(GroundTransitions.startingUpdate);
        playSoundEffect("sfx_Clicked");
        Debug.Log("CURRENT STATE "+_fsm.CurrentStateID.ToString());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="v"></param>
    public override void OnTouchEnded(Vector3 v)
    {
        OnClickBegan = false;
    }

    #region FSM
    /// <summary>
    /// SetTransition
    /// </summary>
    /// <param name="t"></param>
    public void SetTransition(GroundTransitions t) { _fsm.PerformTransition(t); }

    /// <summary>
    /// Inits the FSM.
    /// </summary>
    void InitFSM()
    {
        //init
        _fsm = new FSMSys<GroundTransitions, GroundStates>();
        _fsm.nullStateID = GroundStates.NullStateID;
        _fsm.nullTransition = GroundTransitions.NullTransition;

        //STATE 1 INITSTATE
        initState iState = new initState(this);
        iState.nullStateID = _fsm.nullStateID;
        iState.nullTransition = _fsm.nullTransition;
        iState.AddTransition(GroundTransitions.idle, GroundStates.idle);
        iState.AddTransition(GroundTransitions.startingUpdate, GroundStates.updateStarted);
        iState.AddTransition(GroundTransitions.updated, GroundStates.updated);

        //STATE 2 IDLESTATE
        idleState idle = new idleState(this);
        idle.nullStateID = _fsm.nullStateID;
        idle.nullTransition = _fsm.nullTransition;
        idle.AddTransition(GroundTransitions.startingUpdate , GroundStates.updateStarted);
        idle.AddTransition(GroundTransitions.destroy        , GroundStates.destroyed);

        //STATE 3 EVOLUTIONSTARTEDSTATE
        updateStartedState eStarted = new updateStartedState(this);
        eStarted.nullStateID = _fsm.nullStateID;
        eStarted.nullTransition = _fsm.nullTransition;
        eStarted.AddTransition(GroundTransitions.updating, GroundStates.updating);
        eStarted.AddTransition(GroundTransitions.idle, GroundStates.idle);

        //STATE 4 EVOLVINGSTATE
        updatingState uState = new updatingState(this);
        uState.nullStateID = _fsm.nullStateID;
        uState.nullTransition = _fsm.nullTransition;
        uState.AddTransition(GroundTransitions.updated, GroundStates.updated);
        uState.AddTransition(GroundTransitions.idle, GroundStates.idle);

        //STATE 5 EVOLUTIONENDEDSTATE
        updatedState upState = new updatedState(this);
        upState.nullStateID = _fsm.nullStateID;
        upState.nullTransition = _fsm.nullTransition;
        upState.AddTransition(GroundTransitions.idle, GroundStates.idle);

        //STATE 6 DESTROYEDSTATE
        destroyedState dState = new destroyedState(this);
        dState.nullStateID = _fsm.nullStateID;
        dState.nullTransition = _fsm.nullTransition;
        dState.AddTransition(GroundTransitions.idle, GroundStates.idle);


        //ADD STATES
        _fsm.AddState(iState);
        _fsm.AddState(idle);
        _fsm.AddState(eStarted);
        _fsm.AddState(uState);
        _fsm.AddState(upState);
        _fsm.AddState(dState);
    }


    /// <summary>
    /// initState
    /// </summary>
    public class initState: State<GroundTransitions, GroundStates>
    {
        private groundBehaviour _parent;
        private bool _initializedFlag = false;
        public initState(groundBehaviour parent)
        {
            _parent = parent;
            stateID = GroundStates.init;
        }

        public override void Reason()
        {
            if (_initializedFlag)
            {
                _parent.SetTransition(GroundTransitions.idle);
            }
        }

        public override void Act()
        {
            _initializedFlag = true;
        }

        public override void DoBeforeEntering()
        {
            Debug.Log("GAME STATE " + stateID.ToString());
            _initializedFlag = false;
        }

        public override void DoBeforeLeaving()
        {

        }
    }

    /// <summary>
    /// idleState
    /// </summary>
    public class idleState : State<GroundTransitions, GroundStates>
    {
        private groundBehaviour _parent;

        public idleState(groundBehaviour parent)
        {
            _parent = parent;
            stateID = GroundStates.idle;
        }

        public override void Reason()
        {
            if ( _parent.currentLevel>=(_parent.maxLevel-1))
            {
                _parent.SetTransition(GroundTransitions.destroy);
            }
        }

        public override void Act()
        {
          
        }

        public override void DoBeforeEntering()
        {
            Debug.Log("GAME STATE " + stateID.ToString());
          
        }

        public override void DoBeforeLeaving()
        {

        }
    }

    /// <summary>
    /// updateStartedState
    /// </summary>
    public class updateStartedState : State<GroundTransitions, GroundStates>
    {
        private groundBehaviour _parent;
        public updateStartedState(groundBehaviour parent)
        {
            _parent = parent;
            stateID = GroundStates.updateStarted;
        }

        public override void Reason()
        {
            _parent.SetTransition(GroundTransitions.updating);
        }

        public override void Act()
        {
          
        }

        public override void DoBeforeEntering()
        {
            Debug.Log("GAME STATE " + stateID.ToString());
     
        }

        public override void DoBeforeLeaving()
        {

        }
    }

    /// <summary>
    /// updatingState
    /// </summary>
    public class updatingState : State<GroundTransitions, GroundStates>
    {
        private groundBehaviour _parent;
        private float _timeCycle = 0.0f;
        private float _timeCrono = 0.0f;
        public updatingState(groundBehaviour parent)
        {
            _parent     =   parent;
            stateID = GroundStates.updating;
        }

        public override void Reason()
        {
            if (_timeCrono    >=  _timeCycle)
            {
                _parent.SetTransition(GroundTransitions.updated);
                return;
            }
            if (!_parent.OnClickBegan)
            {
                _parent.SetTransition(GroundTransitions.idle);
                return;
            }
        }

        public override void Act()
        {
            _timeCrono += Time.deltaTime;
        }

        public override void DoBeforeEntering()
        {
            Debug.Log("GAME STATE " + stateID.ToString());
            _timeCycle  =   _parent.timeCycle;
            _timeCrono  = .0f;
        }

        public override void DoBeforeLeaving()
        {

        }
    }

    /// <summary>
    /// updatedState
    /// </summary>
    public class updatedState : State<GroundTransitions, GroundStates>
    {
        private groundBehaviour _parent;
        private float _timeCorno = 0.0f;
        private float _effectDuration = 0.25f;
        private Vector3 _initialScale = Vector3.one;
        public updatedState(groundBehaviour parent)
        {
            _parent = parent;
            stateID = GroundStates.updated;
        }

        public override void Reason()
        {
            if ( _timeCorno >= _effectDuration )
            {
                _parent.SetTransition(GroundTransitions.idle);
            }
        }

        public override void Act()
        {
            _timeCorno += Time.deltaTime;
        }

        public override void DoBeforeEntering()
        {
            Debug.Log("GAME STATE " + stateID.ToString());
            _parent.currentLevel++;
            _timeCorno          =   0.0f;
            _initialScale       =   _parent.gameObject.transform.localScale;
            _parent.gameObject.transform.localScale = _parent.gameObject.transform.localScale * 1.25f;
        }

        public override void DoBeforeLeaving()
        {
            _parent.playSoundEffect("sfx_Notification");
            _parent.gameObject.transform.localScale =   _initialScale;
        }
    }

    /// <summary>
    /// destroyedState
    /// </summary>
    public class destroyedState : State<GroundTransitions, GroundStates>
    {
        private groundBehaviour _parent;
        private float _timeCrono        = 0.0f;
        private float _effectDuration   = 2.0f;
        public destroyedState(groundBehaviour parent)
        {
            _parent = parent;
            stateID = GroundStates.destroyed;
        }

        public override void Reason()
        {
            if (_timeCrono>=_effectDuration)
            {
                _parent.SetTransition(GroundTransitions.idle);
            }
        }

        public override void Act()
        {
            _timeCrono += Time.deltaTime;
        }

        public override void DoBeforeEntering()
        {
            Debug.Log("GAME STATE " + stateID.ToString());
            _timeCrono = 0.0f;
        }

        public override void DoBeforeLeaving()
        {
            _parent.currentLevel = 0;
            _parent.playSoundEffect("sfx_Destroyed");
        }
    }
    #endregion
}

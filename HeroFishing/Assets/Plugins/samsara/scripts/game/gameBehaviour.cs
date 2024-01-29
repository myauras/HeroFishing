using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using game_core;


/// <summary>
/// Game transitions.
/// </summary>
public enum GameTransitions
{
    NullTransition      = 0, // Use this transition to represent a non-existing transition in your system
    startGame = 1,
    initGame = 2,
    idle = 3,
    check = 4,
    youWin = 5,
    youLose = 6,
}

/// <summary>
/// Game states.
/// </summary>
public enum GameStates
{
    NullStateID = 0, // Use this ID to represent a non-existing State in your system	
    startGame   = 1,
    initGame=2,
    idle = 3,
    check = 4,
    youWin = 5,
    youLose = 6,
}


/// <summary>
/// Defines the game behaviour. Here is where all the action
/// is controlled and executed.
/// </summary>
public class gameBehaviour : MonoBehaviour {
    
    //SCENE OBJECTS
    public GameObject youWinPanel;
    public GameObject youLosePanel;
    public int scoreFactor  = 10;
    public int maxLevel     = 99;
    public int level        = 0;
    public int score        = 0;
    private GameObject[] tiles;
    public     float       timeCycle =   1.0f;//SECONDS
  
    //FSM
    private FSMSys<GameTransitions, GameStates> _fsm;

    #region INIT_VARIABLES
    /// <summary>
    /// Initializes the sample map tiles.
    /// </summary>
    public void initTiles()
    {
        for ( int i=0; i<transform.childCount;i++)
        {
            groundBehaviour gBehaviour = transform.GetChild(i).GetComponent<groundBehaviour>();
            if (level >= gBehaviour.playerLevel)
            {
                gBehaviour.gameObject.SetActive(true);
                switch (gBehaviour.type)
                {
                    case GroundTypes.grassFlower:
                    case GroundTypes.grassPine:
                    case GroundTypes.grassRock:
                    case GroundTypes.grassTree:
                    case GroundTypes.snowMountain:
                    case GroundTypes.grassPineAutumn:
                    case GroundTypes.dirtyRock:
                    case GroundTypes.snowRock:
                        gBehaviour.currentLevel = (int)Random.Range(0.0f, (float)(gBehaviour.maxLevel - 1));
                        break;
                }
            }
            else {
                gBehaviour.gameObject.SetActive(false);
            }
           
        }

      
    }

    /// <summary>
    /// Initializes the player map tiles.
    /// </summary>
    public void initPlayerTiles()
    {
        GameObject playerMap = GameObject.Find("playerMap");

        if (playerMap != null)
        {
            Transform playerMapTransform    =   playerMap.transform;
            for (int i = 0; i < playerMapTransform.childCount; i++)
            {
               
                groundBehaviour gBehaviour = playerMapTransform.GetChild(i).GetComponent<groundBehaviour>();
                if (level >= gBehaviour.playerLevel)
                {
                    gBehaviour.gameObject.SetActive(true);
                    gBehaviour.currentLevel = 0;
                }
                else{
                    gBehaviour.gameObject.SetActive(false);
                }
            }
        }
       
    }

    /// <summary>
    /// Initializes the panels .i.e. youWinPanel & youLosePanel
    /// </summary>
    public void initPanels() {
        youWinPanel.SetActive(false);
        youLosePanel.SetActive(false);
        
        //SCORE PANELS 
        GameObject scorePanel   =   GameObject.Find("indicatorMaxLevel");
        Text panelContent       = null;
        if (scorePanel!=null)
        {
            panelContent = scorePanel.GetComponent<Text>();
            if (panelContent != null)
            {
                panelContent.text =maxLevel.ToString("00");
            }
        }
        scorePanel = GameObject.Find("indicatorLevel");
        if (scorePanel != null)
        {
            panelContent = scorePanel.GetComponent<Text>();
            if (panelContent != null)
            {
                panelContent.text = level.ToString("00");
            }
        }

    }
    /// <summary>
    /// Initializes the score var(S).
    /// </summary>
    public void resetScore()
    {
        level = 0;
        score = 0;
    }

    #endregion
    #region LOGIC
    /// <summary>
    /// Checks if the answer is right or not.
    /// </summary>
    /// <returns></returns>
    public bool checkResult()
    {
        GameObject playerMap            =   GameObject.Find("playerMap");
        Transform playerMapTransform    =   playerMap.transform;
        for (int i = 0; i < transform.childCount; i++)
        {
            groundBehaviour gBehaviour          =   transform.GetChild(i).GetComponent<groundBehaviour>();
            groundBehaviour gBehaviourPlayer    =   playerMapTransform.GetChild(i).GetComponent<groundBehaviour>();

            if (gBehaviour.gameObject.activeSelf)
            {
                if (gBehaviour.currentLevel != gBehaviourPlayer.currentLevel)
                { 
                    return false;
                }
            }
        }
        return true;
    }


    /// <summary>
    /// Sets the time of a life cycle.
    /// </summary>
    /// <returns></returns>
    public bool setTimeCycle()
    {
        GameObject playerMap = GameObject.Find("playerMap");
        Transform playerMapTransform = playerMap.transform;
        for (int i = 0; i < playerMapTransform.childCount; i++)
        {
            groundBehaviour gBehaviourPlayer = playerMapTransform.GetChild(i).GetComponent<groundBehaviour>();
            gBehaviourPlayer.timeCycle = timeCycle;
        }
        return true;
    }

    /// <summary>
    /// When the check button is pressed this function is triggered. 
    /// </summary>
    public void onCheckPressed()
    {
        SetTransition(GameTransitions.check);
    }

    /// <summary>
    /// Updates the player level.
    /// </summary>
    public void updateLevel()
    {
        score++;
        if (score % scoreFactor == 0)
        {
            score = 0;
            level++;
            level = (int)Mathf.Clamp((float)level, .0f, (float)maxLevel);
        }
        if (level > StatsController.level)
        {
            StatsController.level = level;
        }
    }

    #endregion
    #region FSM

    /// <summary>
    /// Resets the game.
    /// </summary>
    public void resetGame()
    {
        resetScore();
        SetTransition(GameTransitions.startGame);
    }

    /// <summary>
    /// Takes the player to the next level.
    /// </summary>
    public void nextLevel()
    {
        SetTransition(GameTransitions.startGame);
    }
    /// <summary>
    /// Inits the FSM.
    /// </summary>
    void InitFSM()
    {
        //init
        _fsm = new FSMSys<GameTransitions, GameStates>();
        _fsm.nullStateID = GameStates.NullStateID;
        _fsm.nullTransition = GameTransitions.NullTransition;

        //STATE 1 STARTGAME
        startGame startGame         = new startGame(this);
        startGame.nullStateID       = _fsm.nullStateID;
        startGame.nullTransition    = _fsm.nullTransition;
        startGame.AddTransition(GameTransitions.initGame, GameStates.initGame);
        startGame.AddTransition(GameTransitions.startGame, GameStates.startGame);

        //STATE 2 INITSTATE
        initState iState = new initState(this);
        iState.nullStateID      = _fsm.nullStateID;
        iState.nullTransition   = _fsm.nullTransition;
        iState.AddTransition(GameTransitions.idle,           GameStates.idle);
        iState.AddTransition(GameTransitions.startGame, GameStates.startGame);

        //STATE 3 IDLESTATE
        idleState  idle   = new idleState(this);
        idle.nullStateID      = _fsm.nullStateID;
        idle.nullTransition   = _fsm.nullTransition;
        idle.AddTransition(GameTransitions.check,GameStates.check);
        idle.AddTransition(GameTransitions.startGame, GameStates.startGame);

        //STATE 4 IDLESTATE
        checkState cState = new checkState(this);
        cState.nullStateID = _fsm.nullStateID;
        cState.nullTransition = _fsm.nullTransition;
        cState.AddTransition(GameTransitions.youWin, GameStates.youWin);
        cState.AddTransition(GameTransitions.youLose, GameStates.youLose);
        cState.AddTransition(GameTransitions.startGame, GameStates.startGame);

        //STATE 5 YOUWINSTATE
        youWinState uWinState        = new youWinState(this);
        uWinState.nullStateID         = _fsm.nullStateID;
        uWinState.nullTransition      = _fsm.nullTransition;
        uWinState.AddTransition(GameTransitions.startGame, GameStates.startGame);

        //STATE 6 YOUWINSTATE
        youLoseState uLoseState = new youLoseState(this);
        uLoseState.nullStateID = _fsm.nullStateID;
        uLoseState.nullTransition = _fsm.nullTransition;
        uLoseState.AddTransition(GameTransitions.startGame, GameStates.startGame);
        //ADD STATES
        _fsm.AddState(startGame);
        _fsm.AddState(iState);
        _fsm.AddState(idle);
        _fsm.AddState(cState);
        _fsm.AddState(uLoseState);
        _fsm.AddState(uWinState);
    }


    /// <summary>
    /// Sets a new transition.
    /// </summary>
    /// <param name="t"></param>
    public void SetTransition(GameTransitions t) { _fsm.PerformTransition(t); }

    /// <summary>
    /// Initializes the game flow.
    /// </summary>
    public class startGame : State<GameTransitions, GameStates>
    {
        private gameBehaviour _parent;

        public startGame(gameBehaviour parent)
        {
            _parent = parent;
            stateID = GameStates.startGame;
        }

        public override void Reason()
        {
        }

        public override void Act()
        {
            _parent.initTiles();
            _parent.initPlayerTiles();
            _parent.setTimeCycle();
            _parent.initPanels();
            _parent.SetTransition(GameTransitions.initGame);
        }

        public override void DoBeforeEntering()
        {
           
        }

        public override void DoBeforeLeaving()
        {

        }
    }

    /// <summary>
    /// pre-game state. It could be used to show ready, steady go message for example.
    /// </summary>
    public class initState : State<GameTransitions, GameStates>
    {
        private gameBehaviour _parent;
        public initState(gameBehaviour parent)
        {
            _parent = parent;
            stateID = GameStates.initGame;
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Reason()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        public override void Act()
        {
            _parent.SetTransition(GameTransitions.idle);
        }
        /// <summary>
        /// 
        /// </summary>
        public override void DoBeforeEntering()
        {
           Debug.Log("GAME STATE " + stateID.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        public override void DoBeforeLeaving()
        {
         
        }
    }

    /// <summary>
    /// idle state
    /// </summary>
    public class idleState : State<GameTransitions, GameStates>
    {
        private gameBehaviour _parent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        public idleState(gameBehaviour parent)
        {
            _parent = parent;
            stateID = GameStates.idle;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Reason()
        {


        }

        /// <summary>
        /// 
        /// </summary>
        public override void Act()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public override void DoBeforeEntering()
        {
            Debug.Log("GAME STATE " + stateID.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        public override void DoBeforeLeaving()
        {
        }
    }
    /// <summary>
    /// Checks the result.
    /// </summary>
    public class checkState : State<GameTransitions, GameStates>
    {
        private gameBehaviour _parent;

        public checkState(gameBehaviour parent)
        {
            _parent = parent;
            stateID = GameStates.check;
        }

        public override void Reason()
        {
            
        }

        public override void Act()
        {

        }

        public override void DoBeforeEntering()
        {
            Debug.Log("GAME STATE " +   stateID.ToString());
            Debug.Log("RESULT "     +   _parent.checkResult());
            if (_parent.checkResult())
            {
                _parent.SetTransition(GameTransitions.youWin);
            }
            else {
                _parent.SetTransition(GameTransitions.youLose);
            }
          
        }

        public override void DoBeforeLeaving()
        {
           
        }
    }

    /// <summary>
    /// Tells the player he/she is the winner and increases the difficulty level.
    /// </summary>
    public class youWinState : State<GameTransitions, GameStates>
    {
        private gameBehaviour _parent;
        public youWinState(gameBehaviour parent)
        {
            _parent = parent;
            stateID = GameStates.youWin;
        }

        public override void Reason()
        {
           
        }

        public override void Act()
        {

        }

        public override void DoBeforeEntering()
        {
            Debug.Log("GAME STATE " + stateID.ToString());
            _parent.youWinPanel.SetActive(true);
            _parent.updateLevel();
           
        }

        public override void DoBeforeLeaving()
        {
        }
    }

    /// <summary>
    /// Tells the player he/she is a loser and resets the score vars.
    /// </summary>
    public class youLoseState : State<GameTransitions, GameStates>
    {
        private gameBehaviour _parent;
        public youLoseState(gameBehaviour parent)
        {
            _parent = parent;
            stateID = GameStates.youLose;
        }

        public override void Reason()
        {

        }

        public override void Act()
        {

        }

        public override void DoBeforeEntering()
        {
            Debug.Log("GAME STATE " + stateID.ToString());
            _parent.youLosePanel.SetActive(true);
            _parent.resetScore();
        }

        public override void DoBeforeLeaving()
        {
        }
    }
    #endregion

    #region UNITY_EVENTS
    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void OnEnable()
    {
        InitFSM();
    }


    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () {
        _fsm.CurrentState.Reason();
        _fsm.CurrentState.Act();
	}
    #endregion
}

//
//GameWorldData Script
//Global control script for the entire game. Handles game update, game stages and spawning of enemies.
//Contains message functions for game objects for "world" events.
//
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameWorldData : MonoBehaviour {

    #region ConstantsAndTypes
    public const int MAX_LIVES = 4;                //Max number of lives for the player
    #region ENEMY_SPAWN_IMPLEMENT
    //Enemy flags to determine the status of certain enemies
    public const int ENEMY_FLAG_NONE                = 0x0000;
    public const int ENEMY_FLAG_BIG_DESTROYED       = 0x0001;
    public const int ENEMY_FLAG_BOSS_DESTROYED      = 0x0010;
    #endregion

    #region GAME_STATE_IMPLEMENT
    //Stages of Game
    public enum GameStateType { STAGE_INIT = -1, STAGE1, STAGE2, STAGE3, STAGE4, STAGE5, STAGE_BIG_ENEMY, STAGE_BOSS, STAGE_END, MAX_STAGES }
    public GameStateType startStage = GameStateType.STAGE1;
    #endregion

    #endregion

    #region PublicMembers
    public float STAGE_MAX_TIME1 = 10.0f;
    public float STAGE_MAX_TIME2 = 10.0f;
    public float STAGE_MAX_TIME3 = 10.0f;
    public float STAGE_MAX_TIME4 = 10.0f;
    public float STAGE_MAX_TIME5 = 10.0f;    //Time between big enemy and boss fight

    #region UI_IMPLEMENT
    //GUI Text fields 
    public Text debugText;
    public Text timeText;
    public Text scoreText;
    public Image[] LivesImages;
    public Text endGameScoreText;
    public Text endGameText;
    #endregion

    //Constraints for  play area
    public static float MIN_X = -8;
    public static float MAX_X = 8;
    public static float MIN_Y = -4;
    public static float MAX_Y = 4;

    public float meteoriteSpawnInterverval  = 6.04f;
    public GameObject MeteoritePrefab;


    #endregion

    #region PrivateMembers
    //A list of enemy spawn scripts (each script will spawn a certain type enemy via the specified way points) 
    private EnemySimpleSpawn[] enemySimpleSpawnArray;

    #region GAME_STATE_IMPLEMENT
    //Timers for stages and game as well as enemy spawn times/coun
    float stageTimer;
    float gameTimer;
    float enemySimpleSpawnTime;
    float enemySimpleSpawnTimeB;
    #endregion

    float metoriteSpawnTime;
    #region ENEMY_SPAWN_IMPLEMENT
   
    int spiralEnemyCount;           //Number of currently spawned spiral enemies
    int enemyStatusFlag;            //Enemy status flag (keep tracks of status of certain enemies bigenemy, boss etc)

    bool spawnEnemy;                //Time to spawnEnemy(?)
    #endregion
    int gameScore;                  //Points scored by player killing enemies

    #region GAME_STATE_IMPLEMENT
    static GameStateType eGameState;       //Defines what state/stage the game is currently in
    static GameStateType eLastGameState;   //Stores the previous game state
    #endregion

    //Player
    GameObject playerGO;            //Player game object
    PlayerController playerControl; //Player control script
    private GameObject[] metSpawnPoints;

    GameUI gameUIScript;
    #endregion
    //-------------------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start () {
       
        playerGO = GameObject.FindGameObjectWithTag("Player");
        playerControl = playerGO.GetComponent<PlayerController>(); ;

        //Init all game data
        InitializeGame();
    }

    //-------------------------------------------------------------------------------------------------------
    //InitializeGame() initialized all game data for the game and is called on start and re-start of the game
    void InitializeGame()
    {
        #region GAME_STATE_IMPLEMENT
        //Init data per stage
        InitStageData();
        #endregion
        #region UI_IMPLEMENT
        //Text initialization
        if(timeText != null)
            timeText.text   = FormatTimeString(0);
        if(scoreText != null)
            scoreText.text  = gameScore.ToString();
        #endregion
        //Get player information
        playerGO        = GameObject.FindGameObjectWithTag("Player");
        playerControl   = playerGO.GetComponent<PlayerController>(); ;
        playerControl.Initialize();

        metSpawnPoints = GameObject.FindGameObjectsWithTag("MeteoriteSpawnPoint");

       
        #region CHECK_FOR_DEATH_IMPLEMENT
        //Update life icons
        UpdateLivesGUI(MAX_LIVES, MAX_LIVES);
        #endregion
        #region ENEMY_SPAWN_IMPLEMENT
        //Init game data
        enemyStatusFlag = ENEMY_FLAG_NONE;
        #endregion
        #region GAME_STATE_IMPLEMENT
        gameTimer = 0.0f;
        //eGameState = GameStateType.STAGE_INIT;
        //eLastGameState = GameStateType.STAGE_INIT;
        eGameState = startStage;
        eLastGameState = startStage;
        #endregion

        gameScore = 0;
        metoriteSpawnTime       = 0.0f;

        gameUIScript = GetComponent<GameUI>();

    }
    #region GAME_STATE_IMPLEMENT
    
    //-----------------------------------------------------------------------------------------------------------
    //InitStageData() Inits data at the beginning of each stage
    void InitStageData()
    {
        stageTimer = 0.0f;
        enemySimpleSpawnTime = 0.0f;
        enemySimpleSpawnTimeB = 0.0f;
        spiralEnemyCount = 0;
        spawnEnemy = false;
    }
    #endregion

    Vector3 GetRandomMeteoriteSpawnPoint()
    {
        Vector3 spawnPoint = new Vector3(-7.5f,5f,0f);

        if (metSpawnPoints.Length == 0)
        {
            metSpawnPoints = GameObject.FindGameObjectsWithTag("MeteoriteSpawnPoint");
        }
        else
        {
            //NOTE ASSUMES TWO SPAWN POINTS FOR METEORTIES EXIST!
            if (Random.Range(0, 10) > 5) //Spawn on opposite side... 
            {
                spawnPoint =  metSpawnPoints[0].transform.position;
            }
            else
            {
                spawnPoint = metSpawnPoints[1].transform.position;
            }
        }
        return spawnPoint;
    }
    // Update() Handles game update (takes game through all stages)
    void Update () {
        #region CHECK_FOR_DEATH_IMPLEMENT
        
        //Check for game over 
        if (GameOverByPlayer() && eGameState != GameStateType.STAGE_END)
        {
            HandleGameOverByPCDeath();
        }
        else if(playerControl.IsPlayerDead())
        {
            //If the player is dead 
            //(i.e. has lost a life and the game is not over, do nothing in the game until player is reset)
            return;
        }
        #endregion

        #region GAME_STATE_IMPLEMENT
      
        //Update timer for proper stage when game is being played
        UpdateTimersForStages();
        
        //Update actions for each state 
        UpdateActionsForStages();
      
        #endregion

       
        //Exit on Escape (build only)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
    #region GAME_STATE_IMPLEMENT
    //----------------------------------------------------------------------------------------------
    //UpdateTimersForStages() Updates the timer for stage and sets stage to next stage when necessary
    void UpdateTimersForStages()
    {
        if (eGameState > GameStateType.STAGE_INIT && eGameState < GameStateType.STAGE_END)
        {
            //Update state
            stageTimer += Time.deltaTime;
            gameTimer += Time.deltaTime;
            #region UI_IMPLEMENT
            if(timeText != null)
                timeText.text = FormatTimeString((int)gameTimer);
            if(scoreText != null)
                scoreText.text = gameScore.ToString();
            if (debugText != null)
                debugText.text = eGameState.ToString();
            #endregion
        }
        if (stageTimer >= STAGE_MAX_TIME1 && eGameState == GameStateType.STAGE1)
        {
            //Simple enemies spawn from top in STAGE 1
            //Move to next state
            SetNextState(GameStateType.STAGE2);
            InitStageData();
        }
        else if (stageTimer >= STAGE_MAX_TIME2 && eGameState == GameStateType.STAGE2)
        {
            //Simple enemies spawn from left/right in STAGE 2
            //Move to next state
            SetNextState(GameStateType.STAGE3);
            InitStageData();
        }
        else if (stageTimer >= STAGE_MAX_TIME3 && eGameState == GameStateType.STAGE3)
        {
            //Sprial enemies spawn from left and then sometimes (random) from right, Spiral from left in STAGE 3
            //Move to next state
            SetNextState(GameStateType.STAGE4);
            InitStageData();
        }
        else if (stageTimer >= STAGE_MAX_TIME4 && eGameState == GameStateType.STAGE4)
        {
            //Sprial enemies spawn from left and then sometimes (random) from right, Spiral from right in STAGE 4
            //Move to next state
            SetNextState(GameStateType.STAGE_BIG_ENEMY);
            InitStageData();
        }
        else if (eGameState == GameStateType.STAGE_BIG_ENEMY && CheckEnemyStatusFlag(ENEMY_FLAG_BIG_DESTROYED))
        {
            //Move to stage before boss fight
            SetNextState(GameStateType.STAGE5);
            InitStageData();
        }
        else if (stageTimer >= STAGE_MAX_TIME5 && eGameState == GameStateType.STAGE5)
        {
            //Time before boss (after big enemy has died)
            SetNextState(GameStateType.STAGE_BOSS);
            InitStageData();
        }
        else if (eGameState == GameStateType.STAGE_BOSS && CheckEnemyStatusFlag(ENEMY_FLAG_BOSS_DESTROYED))
        {
            //Game is over because boss has been destroyed
            HandleGameOverByBossDeath();
            InitStageData();
        }

    }

    //-----------------------------------------------------------------------------------------------------------
    //UpdateActionsForStages() Performs actions (i.e. spawn enemies) for each stage
    void UpdateActionsForStages()
    {
        //Meteorites and Turrets exist in most stages so spawn here (if not in boss stage)
        if (eGameState > GameStateType.STAGE_INIT && eGameState < GameStateType.STAGE_BOSS)
        {
            //
            //Spawn Meteorites
            //
            metoriteSpawnTime += Time.deltaTime;   //update date time for spawning
            if (metoriteSpawnTime >= meteoriteSpawnInterverval) //check if time to spawn a meteorite
            {
                Vector3 position = GetRandomMeteoriteSpawnPoint();
                position.z = 0.0f;
                GameObject meteorGO = Instantiate(MeteoritePrefab, position, Quaternion.identity) as GameObject;
                metoriteSpawnTime = 0.0f;
            }
        }
    }
   
    static public GameStateType GetGameState()
    {
        return eGameState;
    }
    //-----------------------------------------------------------------------------------------------------------
    //SetNextState() Sets the next state and stores the last state in eLastGameState
    static void SetNextState(GameStateType nextState)
    {
        eLastGameState = eGameState;
        eGameState = nextState;
    }

    //-----------------------------------------------------------------------------------------------------------
    //GameOverByPlayer() Returns true if the game has ended because the player lost all 4 lives
    bool GameOverByPlayer()
    {
        bool gameOver = false;
        if (playerControl.lives <= 0)
        {
            gameOver = true;
        }
        return gameOver;
    }

    //-----------------------------------------------------------------------------------------------------------
    //HandleGameOverByPCDeath() Handles the case when player loses.
    void HandleGameOverByPCDeath()
    {
        //Adjust text
        #region UI_IMPLEMENT
        if(endGameText != null)
          endGameText.text = "Good Effort, but you lost!";
        if(endGameScoreText != null)
            endGameScoreText.text = "Your score is: " + gameScore.ToString();
        #endregion
        //Switch to end game stage
        SetNextState(GameStateType.STAGE_END);
        //send message to all objects who need to destroy themselves
        SendGameOverMessage();
        //Switch to end game menu
        #region UI_IMPLEMENT
        gameUIScript.OnEndGame();
        #endregion

    }
    //-----------------------------------------------------------------------------------------------------------
    //HandleGameOverByBossDeath() Handles the case when player wins.
    void HandleGameOverByBossDeath()
    {
        #region UI_IMPLEMENT
        //Adjust text
        if (endGameText != null)
            endGameText.text = "YOU WIN!!! Great Job!";
        if (endGameScoreText != null)
            endGameScoreText.text = "Your score is: " + gameScore.ToString();
        #endregion
        //Switch to end game stage
        SetNextState(GameStateType.STAGE_END);
        //send message to all objects who need to destroy themselves
        SendGameOverMessage();
        ////Switch to end game menu
        #region UI_IMPLEMENT
        gameUIScript.OnEndGame();
        #endregion
    }

    #endregion
    #region CHECK_FOR_DEATH_IMPLEMENT
    //-----------------------------------------------------------------------------------------------------------
    //isGameInPlay() Checks if game is in "play" mode stage (after init screen and before result screen)
    public bool isGameInPlay()
    {
        bool bPlaying = false;
        if (eGameState > GameStateType.STAGE_INIT && eGameState < GameStateType.STAGE_END)
        {
            bPlaying = true;
        }
        return bPlaying;
    }
    #endregion

    #region GAME_STATE_IMPLEMENT 
         
   //-----------------------------------------------------------------------------------------------------------
   //StartGame() Moves game from init state to STAGE 1
   public void StartGame()
   {
       InitializeGame();
       SetNextState(GameStateType.STAGE1);
        SoundManager.instance.musicSourceGame.Play();
   }

   //-----------------------------------------------------------------------------------------------------------------
   //RestartGame() Used to restart (initialize) the game (called before sending game to init stage after game is over)
   public void RestartGame()
   {
       InitializeGame();
   }
    #endregion
    #region UI_IMPLEMENT
    //-----------------------------------------------------------------------------------------------------------------
    //UpdateLivesGUI() Updates the GUI life icons 
    public void UpdateLivesGUI(int lives, int MaxLives)
    {
        //make sure we loaded the lives textures
        if (LivesImages.Length == MaxLives)
        {
            int livesMin = Mathf.Min(lives, MaxLives);
            for (int i = 0; i < MaxLives; i++)
            {
                LivesImages[i].enabled = false;
            }
            for (int i = 0; i < livesMin; i++)
            {
                LivesImages[i].enabled = true;
            }
        }
    }
    #endregion

    #region GAME_STATE_IMPLEMENT 
    
    //-----------------------------------------------------------------------------------------------------------------
    //AdjustGameScore()
    public void AdjustGameScore(int amount)
    {
        gameScore += amount;
    }

    //-----------------------------------------------------------------------------------------------------------------
    //SendGameOverMessage() sends OnGameOver message to all parent game objects 
    public void SendGameOverMessage()
    {
        SendMessageToAllGameObjects("OnGameOver",0);
    }

    //-----------------------------------------------------------------------------------------------------------------
    //SendPlayerLostLife() sends PlayerLostLife message to all parent game objects 
    public void SendPlayerLostLife()
    {
        SendMessageToAllGameObjects("PlayerLostLife", 0);
    }

    //-----------------------------------------------------------------------------------------------------------------
    //Sends message (funcName) to all parent game objects in the game
    //NOTE: games objects must listen for the message (funcName)
    public void SendMessageToAllGameObjects(string funcName, System.Object msg)
    {
        GameObject[] gameObjects = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
        foreach(GameObject go in gameObjects)
        {
            //if this is a "top" parent object in the hierarchy
            if (go && go.transform.parent == null)
            {
                //Broadcast message to all game objects listening
                go.gameObject.BroadcastMessage(funcName, msg, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
   
    //TODO: END
    #endregion

    #region ENEMY_SPAWN_IMPLEMENT
    //-----------------------------------------------------------------------------------------------------------------
    //Sets the enemy status flag
    public void SetEnemyStatusFlag(int enemyStatus)
    {
        enemyStatusFlag |= enemyStatus;
    }
    //-----------------------------------------------------------------------------------------------------------------
    //Clear the enemy status flag
    public void ClearEnemyStatusFlag(int enemyStatus)
    {
        enemyStatusFlag &= ~(enemyStatus);
    }
    //-----------------------------------------------------------------------------------------------------------------
    //Checks to see of the enemyStatus  has been set in the enemyStatusFlag
    public bool CheckEnemyStatusFlag(int enemyStatus)
    {
        return ((enemyStatusFlag & enemyStatus) >= 1);
    }
    #endregion
    #region UtilityFunctions
    //-------------------------------------------------------------------------------------------------------
    //AngleWindLeft(): Returns the angle (0-360 degrees) between two vectors (from and to). 
    public static float AngleWindLeft(Vector3 from, Vector3 to)
    {
        float angle = 0.0f;
        //Vector3.Angle returns the smallest angle between to vectors, 
        //we are rotating to the left so we want to wind to the left (use larger angle) if necessary
        angle = Vector3.Angle(from, to);
        if (from.x < to.x)
        {
            angle = 360 - angle;
        }
        return angle;
    }
    //-------------------------------------------------------------------------------------------------------
    //FormatTimeString(): Formats the time string to triple digits
    string FormatTimeString(int time)
    {
        string timeString = "0";

        if (time < 10)
        {
            timeString = "00" + time;
        }
        else if (time < 100)
        {
            timeString = "0" + time;
        }
        else if (time < 1000)
        {
            timeString = "" + time;
        }

        return timeString;
    }
    #endregion
}

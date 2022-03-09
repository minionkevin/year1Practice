//
//BigEnemyController Script
//This script controls the BigEnemyController (it is the mind of the object)
//
using UnityEngine;
using System.Collections;

public class BigEnemyController : MonoBehaviour {

    #region PublicMembers
    public GameObject bigBodyPrefab;  //Holds the body of enemy (sprites and BigEnemyMovement scripts)
    #endregion

    #region PrivateMembers

    //States of stages of BigEnemy fight
    private enum bigStateType { FLY_IN, STAGE1, STAGE2, STAGE3, STAGE4, FLY_OUT, WAIT_FOR_PLAYER, OVER };

    private GameObject gameWorld;               //GameWorld object
    private GameWorldData gameWorldDataScript;  //GameWorld script
    private GameObject playerBody;              //holds the player body
    private GameObject playerGO;                //holds the player control
    private GameObject bigBody;                 //holds the body of the enemy 
    private BigEnemyMovement bigMovementScript; //holds the movement script for the body
    private PlayerController playerControl;     //holds the player control script  
    private Vector3 direction = new Vector3(0.00f, -1.0f, 0.0f); //overall direction of enemy

    [SerializeField]
    float highOffset = 10.0f;
    [SerializeField]
    float lowOffset = 7.0f;


    bigStateType currentState;                  //current state of enemy
    float timer;                                //keeps track of time for enemy fight
    float flyInTime;                            //keeps track of initial fly in time of enemy
    float stage1Time;                           //Timers for stages of enemy fight
    float stage2Time;
    float stage3Time;
    float stage4Time;

    float flyOutTime;                           //keeps track of fly out time
    float speed;                                //keeps track of enemy speed

    #endregion
    //----------------------------------------------------------------------------------------
	//Start() Use this for initialization
	void Start () {
        //create the body game object and set name
        //bigBody = Instantiate(bigBodyPrefab, new Vector3(-0.05f, 12.75f, 0.0f), transform.rotation) as GameObject;
        bigBody = Instantiate(bigBodyPrefab, transform.position, transform.rotation) as GameObject;

        bigBody.name = "BigBody";
        bigBody.SetActive(true);

        //init data for enemy
        playerBody = GameObject.Find("PlayerBody");
        currentState = bigStateType.FLY_IN;
        timer = 0;
        flyInTime = 3.0f;
        stage1Time = 4.5f;
        stage2Time = 2.5f;
        stage3Time = 0.5f;
        stage4Time = 0.5f;
        flyOutTime = 3.0f;
      
        speed = 3.0f;
     

        bigMovementScript = bigBody.GetComponent<BigEnemyMovement>();

        currentState = bigStateType.FLY_IN;
        gameWorld = GameObject.Find("GameWorld");
        gameWorldDataScript = gameWorld.GetComponent<GameWorldData>();

        playerGO = GameObject.FindGameObjectWithTag("Player");
        playerControl = playerGO.GetComponent<PlayerController>(); ;

    }

    //----------------------------------------------------------------------------------------
    // OnGameOver() Listen for game over message
    void OnGameOver(int input)
    {
        Destroy(gameObject);
        Destroy(bigBody);
    }

    //----------------------------------------------------------------------------------------
    // Update() is called once per frame
    void Update () {

        if (bigMovementScript == null)
        {
            bigMovementScript = bigBody.GetComponent<BigEnemyMovement>();
        }
        if (playerBody == null)
        {
            playerBody = GameObject.Find("PlayerBody");
        }
        
        //if the enemy is about to die, set the gameworld flag and destroy this object
        if (bigMovementScript.GetTimeToDie())
        {
            gameWorldDataScript.SetEnemyStatusFlag(GameWorldData.ENEMY_FLAG_BIG_DESTROYED);
            DestroyObject(this.gameObject);
            return;
        }

        //If the player lost his life -- wait for player to respawn
        if( playerControl.IsPlayerDead() && currentState != bigStateType.WAIT_FOR_PLAYER)
        {
            timer = 0;
            currentState = bigStateType.WAIT_FOR_PLAYER;
        }

        //Updating states
        if (currentState == bigStateType.FLY_IN)
        {
            //Wait for enemy to fly into position
            timer += Time.deltaTime;
            if (timer > flyInTime)
            {
                timer = 0;
                currentState = bigStateType.STAGE1;
            }
            //Set  states for the movement script and set destination position
            bigMovementScript.SetScatterOnOff(false);
            bigMovementScript.SetReady(false);
            bigMovementScript.SetDestinationPoint(playerBody.transform.position.x, playerBody.transform.position.y + lowOffset);
        
        }
        else if(currentState == bigStateType.STAGE1 )
        {
            //Set states for the movement script and set destination position
            timer += Time.deltaTime;
            bigMovementScript.SetDestinationPoint(playerBody.transform.position.x, playerBody.transform.position.y + lowOffset);
            bigMovementScript.SetScatterOnOff(true);
            bigMovementScript.SetReady(true);

            if (timer > stage1Time)
            {
                timer = 0;
                currentState = bigStateType.STAGE2;
            }
        
        }
        else if (currentState == bigStateType.STAGE2)
        {
            //Set states for the movement script and set destination position
            timer += Time.deltaTime;
            bigMovementScript.SetDestinationPoint(playerBody.transform.position.x, playerBody.transform.position.y + highOffset);
            if (timer > stage2Time)
            {
                timer = 0;
                currentState = bigStateType.STAGE1;
            }
            bigMovementScript.SetScatterOnOff(true);
            bigMovementScript.SetReady(true);

        }
        else if (currentState == bigStateType.WAIT_FOR_PLAYER)
        {
            //Player has lost a life during battle
            //Move out of the way and wait for player to respawn
            bigMovementScript.SetDestinationPoint(playerBody.transform.position.x, playerBody.transform.position.y + highOffset);
            if (!playerControl.IsPlayerDead())
            {
                timer = 0;
                currentState = bigStateType.STAGE2;
            }
            bigMovementScript.SetScatterOnOff(false);
            bigMovementScript.SetReady(false);
        }
        #region UnusedCode
        /*  else if (currentState == bigStateType.STAGE3)
          {
              timer += Time.deltaTime;
              bigMovementScript.SetDestinationPoint(playerBody.transform.position.x, playerBody.transform.position.y + 6);
              if (timer > stage3Time)
              {
                  timer = 0;
                  currentState = bigStateType.STAGE4;
              }
          }
          else if (currentState == bigStateType.STAGE4)
          {
              timer += Time.deltaTime;
              bigMovementScript.SetDestinationPoint(playerBody.transform.position.x, playerBody.transform.position.y + 6);
              if (timer > stage4Time)
              {
                  timer = 0;
                  currentState = bigStateType.STAGE1;
              }
          }
          else if (currentState == bigStateType.FLY_OUT)
          {
              timer += Time.deltaTime;
              if (timer > flyOutTime)
              {
                  timer = 0;
                  Destroy(bigBody, 0.01f);

                  Destroy(gameObject,0.01f);
                  currentState = bigStateType.OVER;
              }
          }*/
        #endregion

    }
}

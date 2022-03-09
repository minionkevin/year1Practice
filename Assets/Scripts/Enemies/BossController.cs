//
//BossController Script
//This script controls the BossController (it is the mind of the object)
//

using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    #region PublicMembers
    public GameObject bossBodyPrefab;               //prefab of boss object
    #endregion
    #region PrivateMembers
    //Hold possible stages of boss 
    private enum bossStateType { FLY_IN, STAGE1, STAGE2, STAGE3, STAGE4, WAIT_FOR_PLAYER };

    private GameObject gameWorld;                   //GameWorld Object
    private GameObject playerBody;                  //holds the player body object
    private GameObject playerGO;                    //holds the player control object
    private GameObject bossBody;                    //holds the boss body object
    [SerializeField]
    private GameObject turretLeft;                  //holds Left turret object
    [SerializeField]
    private GameObject turretRight;                 //holds Right turret object
    [SerializeField]
    private GameObject turretCenter;                //holds Center turret object

    private PlayerController playerControl;         //holds the player controller script (from player GO object)
    private GameWorldData gameWorldDataScript;      //GameWorld Script
    private TurretMovement turretLeftScript;        //Turret Scripts
    private TurretMovement turretRightScript;
    private TurretMovement turretCenterScript;
    private BossMovement bossMovementScript;        //bossMovementScript (from bossBody object)

    private Vector3 finalPos;                       //final position to move to
    private Vector3 waitPos;                        //wait position until player is ready (after player life is lost)
    private Vector3 direction = new Vector3(0.00f, -1.0f, 0.0f);
   
    bossStateType currentState;                     //tracks current state of boss
    
    //Timers for stages
    float timer;
    float flyInTime;
    float stage1Time;
    float stage2Time;
    float stage3Time;
    float stage4Time;
    float speed;                                    //speed of boss

    #endregion
    //---------------------------------------------------------------------------------------
    //Start() Use this for initialization
    void Start () {
        
        bossBody = Instantiate(bossBodyPrefab, new Vector3(-0.05f, 12.75f,0.0f), transform.rotation) as GameObject;
        bossBody.name = "BossBody";
        bossBody.SetActive(true);
        playerBody = GameObject.Find("PlayerBody");
        currentState = bossStateType.FLY_IN;
        timer = 0;
        flyInTime = 3.0f;
        stage1Time = 3.0f;
        stage2Time = 3.0f;
        stage3Time = 3.0f;
        stage4Time = 3.0f;
        speed = 2.0f;
        finalPos = new Vector3(-0.05f, 5.78f, 0.0f);
        waitPos = new Vector3(-0.05f, 8.78f, 0.0f);

        CheckRefGameObjects();

        bossMovementScript = bossBody.GetComponent<BossMovement>();

        gameWorld = GameObject.Find("GameWorld");
        gameWorldDataScript = gameWorld.GetComponent<GameWorldData>();

        playerGO = GameObject.FindGameObjectWithTag("Player");
        playerControl = playerGO.GetComponent<PlayerController>(); ;

    }

    //---------------------------------------------------------------------------------------
    // OnGameOver() Listen for game over message
    void OnGameOver(int input)
    {
        Destroy(gameObject);
        Destroy(bossBody);
    }

    //---------------------------------------------------------------------------------------
    //CheckRefGameObjects() Function to check if references have been found
  
    void CheckRefGameObjects()
    {
        if (turretLeft != null)
        {
            turretLeftScript = turretLeft.GetComponent<TurretMovement>();
            turretLeftScript.SetGunsOnOff(false);
        }
        if (turretRight != null)
        {
            turretRightScript = turretRight.GetComponent<TurretMovement>();
            turretRightScript.SetGunsOnOff(false);
        }
        if (turretCenter != null)
        {
            turretCenterScript = turretCenter.GetComponent<TurretMovement>();
            turretCenterScript.SetGunsOnOff(false);
        }
    }
    //---------------------------------------------------------------------------------------
    // Update() is called once per frame, controls movement of boss body and stages of boss
    void Update () {

      
        if (playerBody == null)
            playerBody = GameObject.Find("PlayerBody");

        if (bossMovementScript.GetTimeToDie())
        {
            //if it is time to die, set the flag in the game world script so game knows 
            //boss is destroyed
            gameWorldDataScript.SetEnemyStatusFlag(GameWorldData.ENEMY_FLAG_BOSS_DESTROYED);
            DestroyObject(this.gameObject);
            return;
        }
        //If the player lost his life -- wait for player to respawn
        if (playerControl.IsPlayerDead() && currentState != bossStateType.WAIT_FOR_PLAYER)
        {
            timer = 0;
            currentState = bossStateType.WAIT_FOR_PLAYER;
        }

        //Handle actions for stages
        if (currentState ==  bossStateType.FLY_IN)
        {
            //Fly in to final position 
            float distance = Vector3.Distance(bossBody.transform.position, finalPos);
            if (distance > 0.5)
            {
                bossBody.transform.Translate(direction * Time.deltaTime * speed);
            }
            timer += Time.deltaTime;
            if(timer > flyInTime)
            {
                //time to move to next stage
                timer = 0;
                currentState = bossStateType.STAGE1;
                if (bossMovementScript != null)
                    bossMovementScript.SetReady(true);
            }
            //Set guns to off while flying in 
            if (turretLeft != null)
                turretLeftScript.SetGunsOnOff(false);
            if (turretRight != null)
                turretRightScript.SetGunsOnOff(false);
            if (turretCenter != null)
                turretCenterScript.SetGunsOnOff(false);
            //Set data for boss movement script
            if (bossMovementScript != null)
            {
                bossMovementScript.SetReady(true);
                bossMovementScript.SetDestinationPoint(waitPos.x, waitPos.y); //set destination to wait position
            }
        }
        else if (currentState == bossStateType.STAGE1)
        {
            //Stage 1 sets the left turret on and all other turrets off
            if (turretLeft != null)
                turretLeftScript.SetGunsOnOff(true);
            if (turretRight != null)
                turretRightScript.SetGunsOnOff(false);
            if (turretCenter != null)
                turretCenterScript.SetGunsOnOff(false);
            if (bossMovementScript != null)
            {
                bossMovementScript.SetScatterOnOff(false);
                bossMovementScript.SetReady(true);
            }
            timer += Time.deltaTime;
            if (timer > stage1Time)
            {
                //Time to move to stage 2
                timer = 0;
                currentState = bossStateType.STAGE2;
            }
        }
        else if (currentState == bossStateType.STAGE2)
        {
            //Stage 2 sets the right turret on and all other turrets off
            if (turretLeft != null)
                turretLeftScript.SetGunsOnOff(false);
            if (turretRight != null)
                turretRightScript.SetGunsOnOff(true);
            if (turretCenter != null)
                turretCenterScript.SetGunsOnOff(false);
            if (bossMovementScript != null)
                bossMovementScript.SetScatterOnOff(false);

            timer += Time.deltaTime;
            if (timer > stage2Time)
            {
                //Time to move to stage 3
                timer = 0;
                currentState = bossStateType.STAGE3;
            }
        }
        else if (currentState == bossStateType.STAGE3)
        {
            //Stage 3 sets the center turret on and all other turrets off
            if (turretLeft != null)
                turretLeftScript.SetGunsOnOff(false);
            if (turretRight != null)
                turretRightScript.SetGunsOnOff(false);
            if (turretCenter != null)
                turretCenterScript.SetGunsOnOff(true);
            if (bossMovementScript != null)
                bossMovementScript.SetScatterOnOff(false);

            timer += Time.deltaTime;
            if (timer > stage3Time)
            {
                //Time to move to stage 4
                timer = 0;
                currentState = bossStateType.STAGE4;
            }
        }
        else if (currentState == bossStateType.STAGE4)
        {
            //Stage 4 sets the scatter shot on and ALL turrets off
            if (turretLeft != null)
                turretLeftScript.SetGunsOnOff(false);
            if (turretRight != null)
                turretRightScript.SetGunsOnOff(false);
            if (turretCenter != null)
                turretCenterScript.SetGunsOnOff(false);
            if (bossMovementScript != null)
                bossMovementScript.SetScatterOnOff(true);

            timer += Time.deltaTime;
            if (timer > stage4Time)
            {
                //Time to go back to stage 1 and start cycle of turrets shooting then scatter shot over again
                timer = 0;
                currentState = bossStateType.STAGE1;
            }
        }
        else if (currentState == bossStateType.WAIT_FOR_PLAYER)
        {
            //Player has lost a life so we need to wait until player has respawned 
            //Shut everything off until we are ready to play again
            if (turretLeft != null)
                turretLeftScript.SetGunsOnOff(false);
            if (turretRight != null)
                turretRightScript.SetGunsOnOff(false);
            if (turretCenter != null)
                turretCenterScript.SetGunsOnOff(false);
            if (bossMovementScript != null)
                bossMovementScript.SetScatterOnOff(false);

            //Set the currentState back to Stage1 when the player has respawned
            if (!playerControl.IsPlayerDead())
            {
                timer = 0;
                currentState = bossStateType.STAGE1;
            }
            bossMovementScript.SetDestinationPoint(waitPos.x, waitPos.y);
        }

        //Check the player position and set the destination position for the boss movement script
        if (currentState != bossStateType.FLY_IN && currentState != bossStateType.WAIT_FOR_PLAYER)
        {
            Vector3 tempDest = new Vector3(finalPos.x, playerBody.transform.position.y + 9.0f, 0.0f);
            float rangeX = 2.5f;
            float distance = Vector3.Distance(playerBody.transform.position, finalPos);
            if (distance > 1.5f)
            {
                //check which side we are on...
                if (playerBody.transform.position.x - finalPos.x > 0)
                {
                    //right side
                    tempDest.x += rangeX;
                }
                else
                {
                    //left side
                    tempDest.x -= rangeX;
                }
            }
            if (bossMovementScript != null)
            {
                bossMovementScript.SetDestinationPoint(tempDest.x, tempDest.y);
            }
        }
	}
    
}

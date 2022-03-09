//
//PlayerController Script
//This script handles decisions for the movement, shots, death and pickups for the player
//It works with PlayerMovement script to peform actual movements.
//
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    #region PublicMembers
    //Define the types of shots available to player
    public enum shotTypes { REGULAR, THREE, LASER, CANNON, SCATTER };
    public shotTypes intialShotType = shotTypes.REGULAR;                    //Holds the shot type to start player with
    public int lives = 4;                               //Defines the total possible lives for player
    public bool invinceable = false;                    //If set to true,  player will not take damage
    public GameObject playerBodyPreFab;                 //Holds the prefab for the body game object for player
    #endregion

    #region PrivateMembers
    const float TIME_BETWEEN_LIVES = 4.0f;                  //Defines amount of time to wait between loss of life and respawn
    private shotTypes currentShotType;                      //Holds current shot type for player (will change with a pick up)
    private float rotSpeed;                                 //Rotation speed of object
    private float speed;                                    //Speed of object
    private bool playerDead;                                //Set to true when player has lost a life (other game objects use this to wait for player to respawn)
    private bool gameOver;                                  //If player loses all 4 lives then game is over 

    private GameObject playerBody;                          //Holds player body object 
    private GameObject gameWorld;                           //Holds GameWorld object
    private GameWorldData gameWorldDataScript;              //Holds GameWorld script
    private PlayerMovement playerMove;                      //Holds player move script (to set proper movement for player)
   
    public void SetCurrentShotType(shotTypes inShotType ) { currentShotType = inShotType; }
    public bool IsPlayerDead() { return playerDead; }
#endregion
    //-------------------------------------------------------------------------------------
    // Start() Use this for initialization
    void Start()
    {
        rotSpeed = 150.0f;
        speed = 10f;
        Initialize();
    }
    //-------------------------------------------------------------------------------------
    //Initialize() data members
    public void Initialize()
    {
        if(playerBody == null)
            playerBody = Instantiate(playerBodyPreFab, transform.position, transform.rotation) as GameObject;

        playerBody.SetActive(true);
        playerBody.name = "PlayerBody";
        playerBody.transform.position = transform.position;

        playerMove = playerBody.GetComponent<PlayerMovement>();
        gameWorld = GameObject.Find("GameWorld");
        gameWorldDataScript = gameWorld.GetComponent<GameWorldData>();
        playerDead = false;
        gameOver = false;
        playerMove.Initialize();
        InitPlayerBody();
        lives = GameWorldData.MAX_LIVES;
    }


    //-------------------------------------------------------------------------------------
    // Update() Updates player control 
    void Update()
    {
        if (!playerDead && !gameOver && gameWorldDataScript.isGameInPlay())
        {
            UpdateControl();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    //-------------------------------------------------------------------------------------
    //UpdateControl() Updates player movement based on input by user
    private void UpdateControl()
    {
        //get direction in x and y via the vertical and horizontal inputs
        float dy = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float dx = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        //translate in the proper direction based on inputs
        playerBody.transform.Translate(new Vector3(dx, dy, 0));

        if (dx < 0)
        {
            //If we are moving to the left rotate thrusters to left
            playerMove.RotateThrustersLeft();
        }
        else if (dx > 0)
        {
            //If we are moving to the right rotate thruster to the right
            playerMove.RotateThrustersRight();
        }
        else
        {
            //If we are not moving in the x direction then return thrusters to neutral position
            playerMove.RotateThrustersNeutral();
        }

        //Take a shot if it is time
        TakeAShot();

        //Check if player is in bounds
        CheckBounds();

        //Check if player is dead
        CheckForDeath();
    }

    //-------------------------------------------------------------------------------------
    //TakeAShot() Allows the user to take a shot at enemy when fire button is pressed
    private void TakeAShot()
    {
        //if user is pressing fire button
        if (Input.GetButtonDown("Fire1"))
        {
            //Determine shot to take based on shot type
            switch(currentShotType)
            {
                case shotTypes.CANNON:
                {
                    playerMove.ShootCannonBullet();
                    break;
                }
                case shotTypes.LASER:
                {
                    playerMove.ShootLaserBullet();
                    break;
                }
                case shotTypes.THREE:
                {
                    playerMove.ShootThreeBullet();
                    break;
                }
                case shotTypes.SCATTER:
                {
                    playerMove.ShootScatterBullet();
                    break;
                }
                default:
                {
                    playerMove.ShootRegularBullet();
                    break;
                }
            }

        }
    }

    //-------------------------------------------------------------------------------------
    //CheckBounds() Clamps the player's position to be in bounds of play area
    private void CheckBounds()
    {
        float x = playerBody.transform.position.x;
        float y = playerBody.transform.position.y;

        x = Mathf.Clamp(x, GameWorldData.MIN_X, GameWorldData.MAX_X);
        y = Mathf.Clamp(y, GameWorldData.MIN_Y, GameWorldData.MAX_Y);

        playerBody.transform.position = new Vector3(x, y, transform.position.z);
        //print("position " + x + y + z);
    }

    //-------------------------------------------------------------------------------------
    //CheckForDeath() Check if the player has died (lost one life)
    private void  CheckForDeath()
    {
        //if the player is not invinceable at the moment
        if (!invinceable)
        {
            if (playerMove.GetHealth() <= 0)
            {
                //player has lost a life
                playerDead = true;
                //reset health for next life span
                playerMove.SetHealth(100);
                //trigger the death animation upon loss of life

                //decrment life count
                lives -= 1;
                playerMove.TriggerDeath(lives);
               
                Debug.Log("LIVES: " + lives);
                if (lives <= 0)
                {
                    //all lives are gone so game is over
                    Debug.Log("GameOver");
                    gameOver = true;
                    if(gameOver)
                    {
                        SoundManager.instance.musicSourceGame.Stop();
                    }
                    //Comment the two lines below if you want real death after 4 lives
                    //lives = GameWorldData.MAX_LIVES;
                   // StartCoroutine(ResetPlayer());
                }
                else
                {
                   //we still have lives so game is not over, reset player
                   StartCoroutine(ResetPlayer());
                }
                //update the world script to show proper number of lives
                gameWorldDataScript.UpdateLivesGUI(lives, GameWorldData.MAX_LIVES);
            }
        }
    }

    //-------------------------------------------------------------------------------------
    //ResetPlayer()
    public IEnumerator ResetPlayer()
    {
        yield return new WaitForSeconds(TIME_BETWEEN_LIVES);
        InitPlayerBody();
    }

    //-------------------------------------------------------------------------------------
    //InitPlayerBody()
    public void InitPlayerBody()
    {
        playerBody.transform.position = transform.position;
        playerMove.SetHealth(100);
        //set all rocket parts to active
        playerMove.SetAllActive();
        //set player back to living via flag
        playerDead = false;
        //reset shot type //currentShotType
        currentShotType = intialShotType;
        SetCurrentShotType(currentShotType);
    }

    //------------------------------------------------------------------------------------
    //NOTE: The following functions are used to start the specific coroutine for pick ups
    //-------------------------------------------------------------------------------------
    //StartOneUp()
    public void StartOneUp()
    {
        playerMove.PlayPickUpSound(PickUp.pickUpTypes.ONE_UP);
        StartCoroutine(PickUpOneUp());
    }

    //-------------------------------------------------------------------------------------
    //StartInvince()
    public void StartInvince()
    {
        playerMove.PlayPickUpSound(PickUp.pickUpTypes.INVINCE);
        StartCoroutine(PickUpInvince());
    }

    //-------------------------------------------------------------------------------------
    //StartThree()
    public void StartThree()
    {
        playerMove.PlayPickUpSound(PickUp.pickUpTypes.THREE);
        StartCoroutine(PickUpThree());
    }

    //-------------------------------------------------------------------------------------
    //StartScatter
    public void StartScatter()
    {
        playerMove.PlayPickUpSound(PickUp.pickUpTypes.SCATTER);
        StartCoroutine(PickUpScatter());
    }

    //-------------------------------------------------------------------------------------
    //StartLaser()
    public void StartLaser()
    {
        playerMove.PlayPickUpSound(PickUp.pickUpTypes.LASER);
        StartCoroutine(PickUpLaser());
    }

    //-------------------------------------------------------------------------------------
    //StartCannon()
    public void StartCannon()
    {
        playerMove.PlayPickUpSound(PickUp.pickUpTypes.CANNON);
        StartCoroutine(PickUpCannon());
    }

    //-------------------------------------------------------------------------------------
    //PickUpLaser() Picked up a laser shot, set current shot type
    IEnumerator PickUpLaser()
    {
        SetCurrentShotType(shotTypes.LASER);
        yield return new WaitForSeconds(10.0f);
       //Originaly was created as a timed event, now stays on until another pick up or death
       // SetCurrentShotType(shotTypes.REGULAR); //DON"T SWITCH BACK TO REGULAR
    }

    //-------------------------------------------------------------------------------------
    //PickUpScatter() Picked up a scatter shot, set current shot type
    IEnumerator PickUpScatter()
    {
        SetCurrentShotType(shotTypes.SCATTER);
        yield return new WaitForSeconds(10.0f);
        //Originaly was created as a timed event, now stays on until another pick up or death
        //SetCurrentShotType(shotTypes.REGULAR); //DON"T SWITCH BACK TO REGULAR
    }

    //-------------------------------------------------------------------------------------
    //PickUpCannon() Picked up a cannon shot, set current shot type
    IEnumerator PickUpCannon()
    {
        SetCurrentShotType(shotTypes.CANNON);
        yield return new WaitForSeconds(10.0f);
        //Originaly was created as a timed event, now stays on until another pick up or death
        //SetCurrentShotType(shotTypes.REGULAR); //DON"T SWITCH BACK TO REGULAR
    }

    //-------------------------------------------------------------------------------------
    //PickUpThree()  Picked up a three shot, set current shot type
    IEnumerator PickUpThree()
    {
        SetCurrentShotType(shotTypes.THREE);
        yield return new WaitForSeconds(10.0f);
        //Originaly was created as a timed event, now stays on until another pick up or death
        // SetCurrentShotType(shotTypes.REGULAR); //DON"T SWITCH BACK TO REGULAR
    }

    //-------------------------------------------------------------------------------------
    //PickUpInvince() Enter invinceble mode
    IEnumerator PickUpInvince()
    {
        invinceable = true;
        playerMove.SetInvincible(invinceable);
        playerMove.SetInvincibleEffects(invinceable);
        Debug.Log("PLAYER IS  INVINCEABLE");
        yield return new WaitForSeconds(10.0f);

        //After 10 secs the invinceablity turns off....
        invinceable = false;
        playerMove.SetInvincible(invinceable);
        playerMove.SetInvincibleEffects(invinceable);
        Debug.Log("PLAYER IS  NOT INVINCEABLE");
    }

    //-------------------------------------------------------------------------------------
    //PickUpOneUp() Update life count and HUD
    IEnumerator PickUpOneUp()
    {
     
        lives += 1;  
        lives = Mathf.Min(lives, GameWorldData.MAX_LIVES);
        Debug.Log("PLAYER PICKED UP LIFE" + lives);
        gameWorldDataScript.UpdateLivesGUI(lives, GameWorldData.MAX_LIVES);
        yield return new WaitForSeconds(10.0f);
    }

}

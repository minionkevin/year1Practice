//
//TurretMovement Script
//This script controls the movement of the Turrets, specifically the rotation of the gun
//
using UnityEngine;
using System.Collections;

public class TurretMovement : MonoBehaviour {


    #region Public Members
    public GameObject BulletPrefab;                 //Bullet to shoot from turret gun
    public float lifeTime = 20.0f;                  //LifeTime of Turret (NOTE: Turrets on boss must be shot/destroyed to die)
    public bool dropAtPos = false;                  //If true a pick up is drop at position of Turret at time of destruction
    public void SetGunsOnOff(bool bOnOff) { gunsOn = bOnOff; }  //Set gun on turret to on/off
    public float fireRate = 1.0f;                                 //Rate of shots
    public int numberOfShots = 1;

    #endregion

    #region Private Members
    const int TURRET_POINTS = 2;                            //Shooting a turret gives this amount of points to player
    private GameObject gun;                                 //Turret gun is controlled separately
    private GameObject playerControlGO;                     //Holds the player controller
    private GameObject playerBody;                          //Holds the player body
    private PlayerController playerControl;                 //Holds the player controller script (mind)
    private GameObject shootEmitter;                        //Emitter for start pos of shot
    private GameObject boss;                                //Boss game object (used for only when Turret is attached to boss)
    private GameObject[] turretsInGame;                     //Holds an list of all turrets in game at time of creation of this script
    private TestDrops dropScript;                           //Allows turrets to drop a random pickup at time of destruction
    private GameObject gameWorld;                           //GameWorld object 
    private GameWorldData gameWorldDataScript;              //GameWorld script

    private float shootTime;                                //Time to shoot
  
    private float rotSpeed;                                 //Rotation speed
    private Vector3 upDirection;                            //Starting up direction of placed turret 
    private bool gunsOn;                                    //Guns on/off flag controls whether to shoot
    private GOAudio audioScript;                            //Audio script

    #endregion

    //---------------------------------------------------------------------------------
    //Start() Use this for initialization
    void Start () {

        gunsOn = true;
        gun = this.transform.Find("Gun").gameObject;

        boss = GameObject.Find("Boss");
        shootEmitter = gun.transform.Find("ShootEmitter").gameObject;

        turretsInGame = GameObject.FindGameObjectsWithTag("Turret");

        playerBody      = GameObject.Find("PlayerBody");   //holds body (sprites of player)
        playerControlGO = GameObject.FindGameObjectWithTag("Player");
        playerControl   = playerControlGO.GetComponent<PlayerController>(); ;

        rotSpeed    = 2.0f;
        shootTime   = 0.0f;
        
        upDirection = new Vector3(0, 10, 0); //Starts in the +ve up direction
  
        if (lifeTime > 0)
        {
            Destroy(gameObject, lifeTime);
        }

        gameWorld = GameObject.Find("GameWorld");
        gameWorldDataScript = gameWorld.GetComponent<GameWorldData>();
        dropScript = gameWorld.GetComponent<TestDrops>();
        audioScript = GetComponent<GOAudio>();
    }

    //-----------------------------------------------------------------------------------
    //OnGameOver() Listen for game over message
    void OnGameOver(int input)
    {
        Destroy(gameObject);
    }

    //------------------------------------------------------------------------------------
    // Update() is called once per frame
    void Update () {
        //If player has lost a life (game as not ended), do nothing until the player starts again
        if (playerControl.IsPlayerDead())
        {
            return;
        }
        UpdateGunDirection();
	}

    //---------------------------------------------------------------------------------------
    //UpdateGunDirection() Updates the direction of the Turret gun to face towards the player
    static float angletest = 0.0f;
    public void UpdateGunDirection()
    {
        if ( playerBody != null)
        {
            float angle = 0.0f;
            //get the direction towards the player

            Vector3 moveDirection = playerBody.transform.position - this.transform.position;
            moveDirection.z = 0.0f;
            moveDirection.Normalize();
            
           
            if (moveDirection != Vector3.zero)
            {
                //get the angle from the starting up direction and the direction we need to rotate to
                //NOTE: Vector3.Anglereturns the smallest angle (so value is always 0 ..180 )
                angle = Vector3.Angle(upDirection, moveDirection);

                //adjust for angles between 180 - 360 
                if (moveDirection.x > 0)
                {
                  angle = 180 + (180 - angle); 
                }
            }
            //rotate the gun 
            RotateTurretToAngle(angle);
            #region DebugCode
            //Debug.DrawRay(gun.transform.position, moveDirection * 10, Color.red);
            //Debug.DrawRay(gun.transform.position, upDirection, Color.blue);
            //Vector3 currentDirection = gun.transform.rotation * upDirection;
            //Debug.DrawRay(gun.transform.position, currentDirection * 100, Color.yellow);
            #endregion


            if (gunsOn)
            {
                shootTime += Time.deltaTime;
                if (shootTime >= fireRate)
                {
                    //if guns are on and its time to shoot, fire a scatterbullets
                    StartCoroutine(ShootScatterBullet(angle));
                    shootTime = 0;
                }
            }
        }
        else
        {
            playerBody = GameObject.Find("PlayerBody");
        }
    }

    //--------------------------------------------------------------------------------------
    //ShootScatterBullet() fires bullets with a delay
    IEnumerator ShootScatterBullet(float inAngle)
    {
        //fires shots in a delay using the a direction given the inAngle
        for (int delay = 0; delay < numberOfShots; delay++)
        {
            Quaternion angleRotation = Quaternion.AngleAxis(inAngle, upDirection);
            Vector3 bulletDirection = angleRotation * upDirection;
            bulletDirection.Normalize();
            ShootBulletInDirection(bulletDirection, gun.transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }
    }

    //--------------------------------------------------------------------
    //RotateTurretToAngle() Rotates the gun turret to the angle passed in
    public void RotateTurretToAngle(float angle)
    {
        //Find the rotation of the angle about the z axis
        Quaternion angleRotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        gun.transform.rotation = Quaternion.Slerp(gun.transform.rotation, angleRotation, Time.deltaTime * rotSpeed);
    }

    //--------------------------------------------------------------------
    //ShootBulletInDirection() Shoots bullet in given direction, facing the quaternion rotation passed in
    public void ShootBulletInDirection(Vector3 bulletDirection, Quaternion rotateInit)
    {
        //create the bullet using the bullet prefab
        GameObject bulletGO = Instantiate(BulletPrefab, shootEmitter.transform.position, rotateInit) as GameObject;
        //Get the bulletscript of the created bullet game object
        Bullet bulletScript = bulletGO.GetComponent<Bullet>();
        //set the bullet direction on the script
        bulletDirection.Normalize();
        bulletScript.SetDirection(bulletDirection);
        bulletScript.SetSpeed(30);
        //ignore bullet collisions with this object
        Physics2D.IgnoreCollision(bulletGO.transform.GetComponent<BoxCollider2D>(), this.transform.GetComponent<BoxCollider2D>());

        //The boss may or may not exist because turret can be on the final boss or 
        //placed in the environment on scrolling background, If boss exists we don't want
        //boss to get hit by turret bullet.
        if(boss != null)
            Physics2D.IgnoreCollision(bulletGO.transform.GetComponent<BoxCollider2D>(), boss.transform.GetComponent<BoxCollider2D>());
        else
            boss = GameObject.Find("BossBody"); //for next time we shoot

        foreach(GameObject t in turretsInGame)
        {
            //do not shoot other turrets that existed at time of creation of this script
            if(t != null)
                Physics2D.IgnoreCollision(bulletGO.transform.GetComponent<BoxCollider2D>(), t.transform.GetComponent<BoxCollider2D>());
        }
        if (audioScript != null)
        {
            SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange, audioScript.defaultShoot, audioScript.Shoot2);
        }
    }

    //--------------------------------------------------------------------
    //Detects Trigger collision
    void OnTriggerEnter2D(Collider2D c)
    {
        bool applyDamage = false;

        //if guns are off then we do nothing here
        if (gunsOn == false)
            return;

        //TODO NEED A BETTER LOOKUP!
        #region TODO_NeedBetterLookup
        if (c.gameObject.name.StartsWith("playerBullet"))
        {
            Debug.Log("TurretMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerregularShot"))
        {
            Debug.Log("TurretMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy"); //GET DAMBAGE FROM BULLET SCRIPT AND APPLY
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playercannonShot"))
        {
            Debug.Log("TurretMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerlaserShot"))
        {
            Debug.Log("TurretMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerscatterShot"))
        {
            Debug.Log("TurretMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerthreeShot"))
        {
            Debug.Log("TurretMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("PlayerBody"))
        {
            //Do nothing
        }
        else if (c.gameObject.name.StartsWith("Meteorite"))
        {
            //Do nothing
        }
#endregion
        //Apply damage to player based on shot type of player
        if (applyDamage)  
        {
            //Need to apply damage to health via ApplyDamage script
            ApplyDamage applyDamageScript = this.gameObject.GetComponent<ApplyDamage>();

            Bullet bulletScript = c.gameObject.GetComponent<Bullet>();
            //Get the damge from the bullet script
            int damage = bulletScript.damage;

            Debug.Log("TURRET Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from TURRET " + bulletScript.damage);
            Debug.Log("TURRET HEALTH " + applyDamageScript.healthPoints);

            if (applyDamageScript.ApplyDamageToCharacter(damage))
            {
                Vector3 dropPos = transform.position;  //Init drop position to be turret position
                
                //Destroy the turret
                Destroy(this.gameObject, 0.1f);
                
                //Check if we want to drop based off of the player position
                if (!dropAtPos)
                {
                    //if we are not creating a drop at turret position then use player's position to find a spot 
                    //this is a hack specifically for the boss fight where we want the pick up to be dropped near 
                    //the player
                    dropPos = new Vector3(playerBody.transform.position.x, playerBody.transform.position.y, playerBody.transform.position.z);
                    if (playerBody.transform.position.x < 0)
                        dropPos.x += 3.0f;
                    else
                        dropPos.x -= 3.0f;
                }
                if (dropScript != null)
                {
                    //Drop a pick up (maybe)
                    dropScript.DropBonus(dropPos);
                }
                if (audioScript != null)
                {
                    SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange, audioScript.Death1, audioScript.Death2);
                }

                //Adjust the game score for each Turret we destroy
                gameWorldDataScript.AdjustGameScore(TURRET_POINTS);
            }
        }
    }
}

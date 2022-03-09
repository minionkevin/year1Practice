//
//BigEnemyMovement Script
//This script controls the movement of the "big enemy" of the game.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BigEnemyMovement : MonoBehaviour {

    #region Public Members
    public GameObject scatterBulletPrefab;                                  //Prefab of scatter bullet
    //
    //Set/Get functions for BigEnemy (used by BigEnemyController script)
    //
    public void SetScatterOnOff(bool onOff) { scatterOn = onOff; }
    public void SetReady(bool onOff) { ready = onOff; }
    public void SetDestinationPoint(float x, float y) { destPoint.x = x; destPoint.y = y; destPoint.z = 0.0f; }
    public bool GetTimeToDie() { return timeToDie; }
    #endregion

    #region Private Members
    private enum bigMovementStateType { MOVETO, APPROACH, HOVER };      //Movement states of enemy

    const float SHOOT_MAX_TIME = 0.95f;                                 //Time to shoot
    const float HOVER_MAX_TIME = 6.0f;                                  //Total Time to hover
    const float HOVER_DIR_TIME = 2.0f;                                  //Time to change hover direction

    const float MAX_SPEED = 5.0f;                                       //Max Speed of enemy
    [SerializeField]
    GameObject LShootEmitter;                                           //Left shoot emitter
    [SerializeField]
    GameObject CShootEmitter;                                           //Center shoot emitter
    [SerializeField]
    GameObject RShootEmitter;                                           //Right shoot emitter

    Vector3 destPoint = new Vector3(0.0f, 0.0f, 0.0f);                  //The point to which the big enemy is moving towards SET BY BigEnemyController
    
    float hoverTime = 0;                                                //Timers to keep track of hover, hover direction change and shoot
    float shootTime = 0;
    float hoverDirTime = 0;
    float speed = 5.0f;                                                 //Initial movement speed
    float hoverSpeed = 0.1f;                                            //Hover speed

    bool changeHoverDirection = false;                                  //Is it time to change direction in hover
    bool scatterOn      = false;                                        //Is scatter shot on
    bool ready          = false;                                        //Is enemy ready to shoot/move
    bool timeToDie      = true;                                         //Is it time to die

    bigMovementStateType currMoveState;                                //Current state of enemy
    private GOAudio audioScript;                                       //Audio script
    #endregion

    //-----------------------------------------------------------------------------
    //Start() Use this for initialization
    void Start () {

        scatterOn   = false;
        ready       = false;  //give time to player to fly in
        hoverSpeed  = 0.1f;
        hoverTime   = 0.0f;
        speed       = hoverSpeed;
        hoverDirTime    = 0;
        shootTime       = 0;
        currMoveState   = bigMovementStateType.MOVETO;
        timeToDie       = false;
        changeHoverDirection = false;
        audioScript = GetComponent<GOAudio>();

    }

    //----------------------------------------------------------------------------
    // Update is called once per frame, Updates states and movement within each state
    void Update () {
         //
         //NOTE: destPoint (destination point is set by controller)
         //
         //get the direction to the destination point
         Vector3 direction = destPoint - this.transform.position;
         direction.Normalize();

         //Move to destination point directly
         if (currMoveState == bigMovementStateType.MOVETO)
         {
             float distance = 0;

             changeHoverDirection = false;
             
             //if we are far from position speed up and
             //ramp up speed to the max
             speed = Mathf.Min(MAX_SPEED, speed += 1.5f* Time.deltaTime);
             
             //Always move to destintation point
             transform.Translate(direction * speed * Time.deltaTime);

             //Check the distance to the destination and switch to APPROACH state (to slow down)
             distance = Vector3.Distance(destPoint, this.transform.position);
             if( distance < 2.0f)
             {
                 currMoveState = bigMovementStateType.APPROACH;
                 hoverTime = 0.0f;
                 Debug.Log("APPROACH " + "Speed " + speed + "Dist " + distance);
             }
             // Debug.Log("MOVETO " + "Speed " + speed + "Dist " + distance);
         }
         else if (currMoveState == bigMovementStateType.APPROACH)
         {
             float distance = 0;
             
             //If we are getting close to target position start to slow down and
             //ramp down speed
             speed = Mathf.Max(hoverSpeed, speed -= 1.25f * Time.deltaTime);
             
             //Always move to destintation point
             transform.Translate(direction * speed * Time.deltaTime);

             //Check the distance to the destination and switch to HOVER state 
             distance = Vector3.Distance(destPoint, this.transform.position);
             if (distance < 0.05f)
             {
                 currMoveState = bigMovementStateType.HOVER;
                 hoverTime = 0.0f;
                 hoverDirTime = 0.0f;
                 Debug.Log("HOVER " + "Speed " + speed + "Dist " + distance);
             }
             //if distance is greater than 2 go back to MOVETO state
             if (distance >= 2.0)
             {
                 currMoveState = bigMovementStateType.MOVETO;
                 hoverTime = 0.0f;
                 hoverDirTime = 0.0f;
                 Debug.Log("MOVETO " + "Speed " + speed + "Dist " + distance);
             }
             changeHoverDirection = false;
             //Debug.Log("APPROACH " + "Speed " + speed + "Dist " + distance);
         }
         else if (currMoveState == bigMovementStateType.HOVER)
         {
             float distance = 0;
             //ramp down speed to hover speed
             speed = Mathf.Max(hoverSpeed, speed -= 1.5f * Time.deltaTime);
         
             //update timers
             hoverTime += Time.deltaTime;   
             hoverDirTime += Time.deltaTime;
 
             //keep track of overall hover time
             if(hoverTime >= HOVER_MAX_TIME)
             {
                 //hovered long enough, move back to MOVETO state
                 hoverTime = 0.0f;
                 currMoveState = bigMovementStateType.MOVETO;
             }
             //if we have stopped in the x get moving again
             if (direction.x == 0.0f)
             {
                 direction.x = 0.01f;
             }
             //change hover direction if needed
             if (changeHoverDirection)
             {
                direction *= -1.0f;
             }

             distance = Vector3.Distance(destPoint, this.transform.position);
             transform.Translate(direction * speed * Time.deltaTime);
            // Debug.Log("Hover " + "Speed " + speed + "Dist " + distance);
             if (distance < 0.05f)
             {
                 //set to min speed to avoid jittering
                 speed = hoverSpeed;
             }
             //keep track of when to change directions in hover
             if (hoverDirTime >= HOVER_DIR_TIME)
             {
                 changeHoverDirection = !changeHoverDirection;
                 hoverDirTime = 0.0f;
             }
             //if we have moved too far from destination then switch to move to state
             if (distance >= 2.5)
             {
                 currMoveState = bigMovementStateType.MOVETO;
                 hoverTime = 0.0f;
                 hoverDirTime = 0.0f;
                 Debug.Log("MOVETO" + speed + " speed " + "Dist " + distance);
             }
         }
        //Shoot when ready (Set by controller)
        if (scatterOn && ready)
        {
            shootTime += Time.deltaTime;
            if (shootTime > SHOOT_MAX_TIME)
            {
                shootTime = 0.0f;
                StartCoroutine(ShootScatterBullet());
            }
        }
	}
    //------------------------------------------------------------------------
    //ShootScatterBullet() Shoots out a scatter shot (in fan direction) for enemy
    IEnumerator ShootScatterBullet()
    {
        for (int delay = 0; delay < 3; delay++)
        {
            List<Vector3> scatterDirections = new List<Vector3>() { new Vector3(0.0f, -1.0f, 0.0f), new Vector3(-1.0f, -1.0f, 0.0f), new Vector3(1.0f, -1.0f, 0.0f) };
            List<Vector3> scatterPositions = new List<Vector3>() { CShootEmitter.transform.position, LShootEmitter.transform.position, RShootEmitter.transform.position };
            for (int i = 0; i < 3; i++)
            {
                //Create bullet from prefab
                GameObject bulletGO = Instantiate(scatterBulletPrefab, scatterPositions[i], Quaternion.identity) as GameObject;
                //Gett the bullet script from bullet game object
                Bullet bulletScript = bulletGO.GetComponent<Bullet>();
                //Get the direction at index and set for bullet
                Vector3 bulletDirection = scatterDirections[i];
                bulletDirection.Normalize();
                bulletScript.SetDirection(bulletDirection);
                //Ignore collisions with this bullet and this gameobject (don't shoot your self)
                Physics2D.IgnoreCollision(bulletGO.transform.GetComponent<BoxCollider2D>(), this.transform.GetComponent<BoxCollider2D>());
            }

            if (audioScript != null)
            {
                SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange, audioScript.defaultShoot, audioScript.Shoot2);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    //--------------------------------------------------------------------
    //Detects Trigger collision
    void OnTriggerEnter2D(Collider2D c)
    {
        bool applyDamage = false;
        
        //Ignore collisions  if enemy is not in position....
        if (!ready && !scatterOn)
            return;

        //Check the type of game object that we collided with and apply damage 
        //only if object is listed below
        if (c.gameObject.name.StartsWith("playerBullet"))
        {
            Debug.Log("Big Enemy Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerregularShot"))
        {
            Debug.Log("Big Enemy Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy"); //GET DAMBAGE FROM BULLET SCRIPT AND APPLY
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playercannonShot"))
        {
            Debug.Log("Big Enemy Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerlaserShot"))
        {
            Debug.Log("Big Enemy Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerscatterShot"))
        {
            Debug.Log("Big Enemy Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerthreeShot"))
        {
            Debug.Log("Big Enemy Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }

        //Apply damage if proper collision took place
        if (applyDamage)  
        {
            ApplyDamage applyDamageScript = this.gameObject.GetComponent<ApplyDamage>();
            Bullet bulletScript = c.gameObject.GetComponent<Bullet>();
            //Get the damage amount from bullet script we collided with
            int damage = bulletScript.damage;
            Debug.Log("Big Enemy Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy " + bulletScript.damage);
            Debug.Log("HEALTH " + applyDamageScript.healthPoints);
            //Apply damage via ApplyDamage script
            if (applyDamageScript.ApplyDamageToCharacter(damage))
            {
                //Destroy object if it is time to die (health == 0 )
                timeToDie = true;
                Destroy(this.gameObject, 0.5f);
                if (audioScript != null)
                {
                    SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange, audioScript.Death1, audioScript.Death2);
                }

            }
        }
    }


}

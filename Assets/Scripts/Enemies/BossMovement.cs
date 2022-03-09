//
//BossMovement Script
//This script controls the movement of the "boss enemy" of the game.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossMovement : MonoBehaviour
{
    #region Public Members
    //Status flags for boss
    public const int BOSS_FLAGS_NONE                    = 0x00000000;       //Clear
    public const int BOSS_FLAGS_TURRETL_DESTROYED       = 0x00000001;       //Left Turret Destroyed
    public const int BOSS_FLAGS_TURRETR_DESTROYED       = 0x00000010;       //Right Turret Destroyed
    public const int BOSS_FLAGS_TURRETC_DESTROYED       = 0x00000100;       //Center Turret Destroyed
    public const int BOSS_FLAGS_ALL_DESTROYED = BOSS_FLAGS_TURRETL_DESTROYED | BOSS_FLAGS_TURRETC_DESTROYED | BOSS_FLAGS_TURRETR_DESTROYED;  //All Turrets on boss are destroyed

    public GameObject scatterBulletPrefab;                      //Scatter shot prefab
    public GameObject fanLeft;                                 //Fan objects on boss
    public GameObject fanRight;
    public GameObject LShootEmitter;                           //Left emmitter for scatter shot
    public GameObject CShootEmitter;                           //Center emmitter for scatter shot
    public GameObject RShootEmitter;                           //Right emmitter for scatter shot

    public GameObject turretLeftGO;                            //Left Turret
    public GameObject turretRightGO;                           //Right Turret
    public GameObject turretCenterGO;


    //Set/Get Functions for boss
    public void SetScatterOnOff(bool onOff){scatterOn = onOff;}
    public void SetReady(bool onOff) { ready = onOff; }
    public bool GetTimeToDie() { return timeToDie; }
    public void SetDestinationPoint(float x, float y) { destPoint.x = x; destPoint.y = y; destPoint.z = 0.0f; }
    #endregion

    #region Private Members
    //Movement states
    private enum bossMovementStateType { MOVETO, APPROACH, HOVER };

    const float HOVER_MAX_TIME = 6.0f;                  //Total time to hover
    const float HOVER_DIR_TIME = 2.0f;                  //Time to change
    const float MAX_SPEED = 2.0f;                       //Max speed of boss

       //Large center Turret

    ApplyDamage turretLeftApplyDamageScript;            //Each Turret has its own Apply damage script
    ApplyDamage turretRightApplyDamageScript;
    ApplyDamage turretCenterApplyDamageScript;

    int bossFlags;                                      //Keep track of boss flags (i.e. turrets destroyed)
    float fanSpeedDir = -600.0f;                        //Speed of fan rotation
    float shootTime = 0;                                //Keep track of time to shoot
    float hoverTime = 0;                                //Keep track of time in hover
    float speed = 2.0f;                                 //Speed of boss
    float hoverSpeed = 0.1f;                            //Hover speed of boss
    float hoverDirTime = 0;                             //Keep track of time to change hover directioin
    Vector3 destPoint = new Vector3(0.0f, 0.0f, 0.0f);  //Destination point set by controller script
    bossMovementStateType currMoveState;                //Keep track of current move state
    bool changeHoverDirection = false;                  //Keep track of when to change direction in hover
    bool scatterOn = false;                             //Keep track of whether to have scatter shot on or off
    bool ready;                                         //Keeps track of enemy ready (ie. has reached fly in position)
    bool timeToDie = true;                              //Keeps track of whether it is time to die
    private GOAudio audioScript;                        //Audio script
    #endregion
    //---------------------------------------------------------------------------------------
    //Start()
    //Use this for initialization
    void Start () {
   
        ready = false;
        timeToDie = false;
        changeHoverDirection = false;
        hoverTime = 0.0f;
        speed = hoverSpeed;

        hoverDirTime = 0;
        currMoveState = bossMovementStateType.MOVETO;

        if(turretLeftGO)
            turretLeftApplyDamageScript = turretLeftGO.GetComponent<ApplyDamage>();
        if (turretRightGO)
            turretRightApplyDamageScript = turretRightGO.GetComponent<ApplyDamage>();
        if (turretCenterGO)
            turretCenterApplyDamageScript = turretCenterGO.GetComponent<ApplyDamage>();

        bossFlags = BOSS_FLAGS_NONE;

        audioScript = GetComponent<GOAudio>();
    }

    //------------------------------------------------------------------------------------
    // Update is called once per frame, animates fans, updates movement states
    void Update () {
        UpdateFans();
        UpdatePosition();
        if (scatterOn && ready)
        {
            shootTime += Time.deltaTime;
            if (shootTime > 0.75)
            {
                //if it is time to shoot begin scatter shots
                shootTime = 0.0f;
                StartCoroutine(ShootScatterBullet());
            }
        }
	}

    //------------------------------------------------------------------------------------
    //UpdatePosition() Updates the destination position for the boss movement script (body movement)
    void UpdatePosition()
    {
        //if not ready do nothing
        if (!ready)
            return;
        CheckAllTurretsDestoyed();
        //get the direction to the current destination point
        Vector3 direction = destPoint - this.transform.position;
        direction.Normalize();
        if (currMoveState == bossMovementStateType.MOVETO)
        {
            float distance = 0;
            changeHoverDirection = false;
            //if we are far from position speed up
            //ramp up speed to the max
            speed = Mathf.Min(MAX_SPEED, speed += 1.5f * Time.deltaTime);
            
            //Always move to destintation point
            transform.Translate(direction * speed * Time.deltaTime);

            distance = Vector3.Distance(destPoint, this.transform.position);
            if (distance < 2.0f)
            {
                //if we are getting close to destination then switch to approach state
                currMoveState = bossMovementStateType.APPROACH;
                hoverTime = 0.0f;
                Debug.Log("APPROACH " + "Speed " + speed + "Dist " + distance);
            }
            // Debug.Log("MOVETO " + "Speed " + speed + "Dist " + distance);
        }
        else if (currMoveState == bossMovementStateType.APPROACH)
        {
            float distance = 0;
            //If we are getting close to target position start to slow down
            //ramp down speed
            speed = Mathf.Max(hoverSpeed, speed -= 1.25f * Time.deltaTime);
            
            //Always move to destintation point
            transform.Translate(direction * speed * Time.deltaTime);

            distance = Vector3.Distance(destPoint, this.transform.position);
            if (distance < 0.05f)
            {
                //if we are close to the destination position then hover
                currMoveState = bossMovementStateType.HOVER;
                hoverTime = 0.0f;
                hoverDirTime = 0.0f;
                Debug.Log("HOVER " + "Speed " + speed + "Dist " + distance);
            }
            if (distance >= 2.0)
            {
                //if we move far away then go to move to state
                currMoveState = bossMovementStateType.MOVETO;
                hoverTime = 0.0f;
                hoverDirTime = 0.0f;
                //Debug.Log("MOVETO " + "Speed " + speed + "Dist " + distance);
            }
            changeHoverDirection = false;
            //Debug.Log("APPROACH " + "Speed " + speed + "Dist " + distance);
        }
        else if (currMoveState == bossMovementStateType.HOVER)
        {
            float distance = 0;
            //slow down speed to hover speed
            speed = Mathf.Max(hoverSpeed, speed -= 1.5f * Time.deltaTime);

            //update hover timers
            hoverTime += Time.deltaTime;
            hoverDirTime += Time.deltaTime;

            //keep track of overall hover time
            if (hoverTime >= HOVER_MAX_TIME)
            {
                //hovered long enough so go to "move to" state
                hoverTime = 0.0f;
                currMoveState = bossMovementStateType.MOVETO;
            }
            if (direction.x == 0.0f)
            {
                direction.x = 0.01f;
            }
            if (changeHoverDirection)
            {
                //time to change direction in hover mode
                direction *= -1.0f;
            }

            //get distance to destination
            distance = Vector3.Distance(destPoint, this.transform.position);

            //move in direction
            transform.Translate(direction * speed * Time.deltaTime);
            // Debug.Log("Hover " + "Speed " + speed + "Dist " + distance);
            if (distance < 0.05f)
            {
                //if distance is close set speed to hover speed 
                speed = hoverSpeed;
            }
            //keep track of when to change ddirections in hover
            if (hoverDirTime >= HOVER_DIR_TIME /*&& speed == hoverSpeed*/)
            {
                changeHoverDirection = !changeHoverDirection;
                hoverDirTime = 0.0f;
            }
            if (distance >= 2.5)
            {
                //if we move far away then go to move to state
                currMoveState = bossMovementStateType.MOVETO;
                hoverTime = 0.0f;
                hoverDirTime = 0.0f;
                Debug.Log("MOVETO" + speed + " speed " + "Dist " + distance);
            }
        }
    }

    //-----------------------------------------------------------------------------------------
    //UpdateFans() animates the fans on the boss
    void UpdateFans()
    {
        if(fanLeft)
            fanLeft.transform.Rotate(0, 0, fanSpeedDir * Time.deltaTime);
        if (fanRight)
            fanRight.transform.Rotate(0, 0, fanSpeedDir * Time.deltaTime);
    }

    //-----------------------------------------------------------------------------------------
    //ShootScatterBullet() Shoot one scatter on each emitter with a delay
    IEnumerator ShootScatterBullet()
    {
        for (int delay = 0; delay < 3; delay++)
        {
            List<Vector3> scatterDirections = new List<Vector3>() { new Vector3(0.0f, -1.0f, 0.0f), new Vector3(-1.0f, -1.0f, 0.0f), new Vector3(1.0f, -1.0f, 0.0f) };
            List<Vector3> scatterPositions = new List<Vector3>();

            if (CShootEmitter)
                scatterPositions.Add(CShootEmitter.transform.position);

            if (LShootEmitter)
                scatterPositions.Add(LShootEmitter.transform.position);

            if (RShootEmitter)
                scatterPositions.Add(RShootEmitter.transform.position);


            for (int i = 0; i < scatterPositions.Count; i++)
            {
                GameObject bulletGO = Instantiate(scatterBulletPrefab, scatterPositions[i], Quaternion.identity) as GameObject;
                Bullet bulletScript = bulletGO.GetComponent<Bullet>();
                Vector3 bulletDirection = scatterDirections[i];
                bulletDirection.Normalize();
                bulletScript.SetDirection(bulletDirection);
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
    //OnTriggerEnter2D() Detects Trigger collision
    void OnTriggerEnter2D(Collider2D c)
    {
        bool applyDamage = false;

        //Is Boss in position?
        if (ready == false)
            return;

        //Only apply damage to boss body if all turrets are destroyed
        if (!CheckAllTurretsDestoyed())
            return;

        //Check to see if we need to apply damage to the enemy
        if (c.gameObject.name.StartsWith("playerBullet"))
        {
            Debug.Log("Boss Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerregularShot"))
        {
            Debug.Log("Boss Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy"); //GET DAMBAGE FROM BULLET SCRIPT AND APPLY
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playercannonShot"))
        {
            Debug.Log("Boss Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerlaserShot"))
        {
            Debug.Log("Boss Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerscatterShot"))
        {
            Debug.Log("Boss Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerthreeShot"))
        {
            Debug.Log("Boss Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }

        //Apply damage to enemy based on type of shot by player
        if (applyDamage)  
        {
            ApplyDamage applyDamageScript = this.gameObject.GetComponent<ApplyDamage>();
            Bullet bulletScript = c.gameObject.GetComponent<Bullet>();
            int damage = bulletScript.damage;
            Debug.Log("BOSS Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy " + bulletScript.damage);
            Debug.Log("BOSS BODY HEALTH " + applyDamageScript.healthPoints);
            if (applyDamageScript.ApplyDamageToCharacter(damage))
            {
                timeToDie = true;
                //Destroy Fans
                if (fanLeft != null)
                    Destroy(fanLeft);
                if (fanRight != null)
                    Destroy(fanRight);

                Destroy(this.gameObject, 0.5f);
                if (audioScript != null)
                {
                    SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange, audioScript.Death1, audioScript.Death2);
                    SoundManager.instance.musicSourceGame.Stop();
                }

            }
        }
    }

    //-----------------------------------------------------------------------------------------
    //CheckAllTurretsDestoyed() Checks to see if all turrets have been destroyed
    bool CheckAllTurretsDestoyed()
    {
        bool allTurretsDead = false;
        if (!CheckBossFlag(BOSS_FLAGS_ALL_DESTROYED))
        {
            if(!CheckBossFlag(BOSS_FLAGS_TURRETL_DESTROYED)) 
            {
                if (turretLeftApplyDamageScript && turretLeftApplyDamageScript.aboutToDie)
                {
                    //Set flag if left turret has been destroyed
                    SetBossFlag(BOSS_FLAGS_TURRETL_DESTROYED);
                }
            }
            if (!CheckBossFlag(BOSS_FLAGS_TURRETR_DESTROYED))
            {
                if (turretRightApplyDamageScript && turretRightApplyDamageScript.aboutToDie)
                {
                    //Set flag if right turret has been destroyed
                    SetBossFlag(BOSS_FLAGS_TURRETR_DESTROYED);
                }
            }
            if (!CheckBossFlag(BOSS_FLAGS_TURRETC_DESTROYED))
            {
                if (turretCenterApplyDamageScript && turretCenterApplyDamageScript.aboutToDie)
                {
                    //Set flag if center turret has been destroyed
                    SetBossFlag(BOSS_FLAGS_TURRETC_DESTROYED);
                }
            }
        }
        else
        {
            allTurretsDead = true;
            //turn on the collider for the body
            BoxCollider2D bodyCollider = GetComponent<BoxCollider2D>();
            bodyCollider.enabled = true;
        }
        
        return allTurretsDead;
    }

    //--------------------------------------------------------------------------------------
    //SetBossFlag() sets the enemyStatus flag
    public void SetBossFlag(int enemyStatus)
    {
        bossFlags |= enemyStatus;
    }

    //--------------------------------------------------------------------------------------
    //ClearBossFlag() clears the enemyStatus flag
    public void ClearBossFlag(int enemyStatus)
    {
        bossFlags &= ~(enemyStatus);
    }

    //--------------------------------------------------------------------------------------
    //CheckBossFlag() checkis if the enemyStatus flag is set
    public bool CheckBossFlag(int enemyStatus)
    {
        return ((bossFlags & enemyStatus) == enemyStatus);
    }
}

//
//PlayerMovement Script
//This script controls the actual movement, rotation of thrusters for player in game
//Attached to player body game object
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {


    #region Public Members

    //Bullet type prefabs (set in editor)
    public GameObject cannonBulletPrefab;
    public GameObject regularBulletPrefab;
    public GameObject laserBulletPrefab;
    public GameObject threeBulletPrefab;
    public GameObject scatterBulletPrefab;
   
    
    private GameObject bodyGO;                                     //Body of player -- Body of player must be set and named properly so it is set as private but kepty with other parts
    //Additional/Optional body parts
    public GameObject fanGO;                                       //Fan
    public GameObject LThrusterGO;                                 //Left Thruster
    public GameObject RThrusterGO;                                 //Right Thruster
    public GameObject CThrustShootEmitter;                         //Center Thruster emitter for shots

    //Get/Set Funnctions
    public GameObject GetLeftThruster() { return LThrusterGO; }
    public GameObject GetRightThruster() { return RThrusterGO; }
    public void SetHealth(int inhealth) { health = inhealth; }
    public int GetHealth() { return health; }
    #endregion

    #region Private Members
    private int   health = 100;                                     //Health of player
    private float fanSpeed = 200.0f;                                //Fanspeed used to animate fan
    private float thrustRotSpeed = 2.0f;                            //Thruster rotation speed
    private float maxThrustAngle = 15.0f;                           //Max/Min thruster angles
    private float minThrustAngle = -15.0f;
    private float centerThrustAngle = 0.0f;                         //Center angle of thruster
    private bool invinceable = false;                               //Set if player has picked up an ivinceable pick up

    private GOAudio audioScript;
    private GameObject gameWorld;                                   //GameWorld object
    private GameWorldData gameWorldDataScript;                      //GameWorld script

    private TriggerDeath triggerDeathScript;                        //Trigger Death script to trigger death animation

    private bool rotateLeft;                                        //Determine whether to rotate right
    private bool rotateRight;                                       //Determine whether to rotate left
    private bool playerBodyDead;                                    //Determine whether player body is dead

    Quaternion leftThrustMax;                                       //Max rotation for left 
    Quaternion rightThrustMax;                                      //Max rotation for right      
    Quaternion neutralThrustMax;                                    //Max rotation for neutral postion

    #endregion
    public void SetInvincible(bool inInvince) { invinceable = inInvince; }

    //--------------------------------------------------------------------
    // Use this for initialization
    void Start()
    { 
        Initialize();
    }
    //--------------------------------------------------------------------
    //Initialize()  Initialize data members
    public void Initialize()
    {
        //init body parts
        bodyGO = this.transform.Find("Body").gameObject;

        //Init angle values
        leftThrustMax = Quaternion.AngleAxis(maxThrustAngle, new Vector3(0, 0, 1));
        rightThrustMax = Quaternion.AngleAxis(minThrustAngle, new Vector3(0, 0, 1));
        neutralThrustMax = Quaternion.AngleAxis(centerThrustAngle, new Vector3(0, 0, 1));
        rotateLeft = false;
        rotateRight = false;


        triggerDeathScript = bodyGO.GetComponent<TriggerDeath>();
        playerBodyDead = false;
        gameWorld = GameObject.Find("GameWorld");
        gameWorldDataScript = gameWorld.GetComponent<GameWorldData>();
        audioScript = GetComponent<GOAudio>();
        SetAllActive();

    }
    //--------------------------------------------------------------------
    // Update() is called once per frame, controls rotation of fan
    void Update()
    {
        if (gameWorldDataScript.isGameInPlay())
        { 
            //RotateThrustersNeutral();
            RotateFan();
        }
    }

    //--------------------------------------------------------------------
    // RotateFan() Updates rotation on fan
    private void RotateFan()
    {
        if (fanGO != null)
            fanGO.transform.Rotate(0, 0, -6.0f * fanSpeed * Time.deltaTime);
    }
    //--------------------------------------------------------------------
    //RotateThrustersLeft() Rotates thruster to the left (called by PlayerController)
    public void RotateThrustersLeft()
    {
        if (LThrusterGO != null )
        LThrusterGO.transform.rotation = Quaternion.Slerp(LThrusterGO.transform.rotation, leftThrustMax, Time.deltaTime * thrustRotSpeed);
        if (RThrusterGO != null)
            RThrusterGO.transform.rotation = Quaternion.Slerp(RThrusterGO.transform.rotation, leftThrustMax, Time.deltaTime * thrustRotSpeed);
    }

    //--------------------------------------------------------------------
    //RotateThrustersRight() Rotates thruster to the right (called by PlayerController)
    public void RotateThrustersRight()
    {
        if (LThrusterGO != null)
            LThrusterGO.transform.rotation = Quaternion.Slerp(LThrusterGO.transform.rotation, rightThrustMax, Time.deltaTime * thrustRotSpeed);
        if (RThrusterGO != null)
            RThrusterGO.transform.rotation = Quaternion.Slerp(RThrusterGO.transform.rotation, rightThrustMax, Time.deltaTime * thrustRotSpeed);
    }

    //--------------------------------------------------------------------
    //RotateThrustersNeutral() Rotates thruster to the neutral position (called by PlayerController)
    public void RotateThrustersNeutral()
    {
        if (LThrusterGO != null)
            LThrusterGO.transform.rotation = Quaternion.Slerp(LThrusterGO.transform.rotation, neutralThrustMax, Time.deltaTime * thrustRotSpeed);

        if (RThrusterGO != null)
            RThrusterGO.transform.rotation = Quaternion.Slerp(RThrusterGO.transform.rotation, neutralThrustMax, Time.deltaTime * thrustRotSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, neutralThrustMax, Time.deltaTime * thrustRotSpeed);
        rotateRight = false;
        rotateLeft = false;
    }

    //--------------------------------------------------------------------
    //CheckBounds() Clanps player position to be within bounds of game play
    private void CheckBounds()
    {
        float x = transform.position.x;
        float y = transform.position.y;

        x = Mathf.Clamp(x, GameWorldData.MIN_X, GameWorldData.MAX_X);
        y = Mathf.Clamp(y, GameWorldData.MIN_Y, GameWorldData.MAX_Y);

        transform.position = new Vector3(x, y, transform.position.z);
        //print("position " + x + y + z);
    }

    //--------------------------------------------------------------------
    //RotateThrustersToAngle() Rotates body to angle
    public void RotateThrustersToAngle(float angle)
    {
        //find the rotation of the angle about the z axis
        Quaternion angleRotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        if (LThrusterGO != null)
            LThrusterGO.transform.rotation = Quaternion.Slerp(LThrusterGO.transform.rotation, angleRotation, Time.deltaTime * thrustRotSpeed);
        if (RThrusterGO != null)
            RThrusterGO.transform.rotation = Quaternion.Slerp(RThrusterGO.transform.rotation, angleRotation, Time.deltaTime * thrustRotSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, angleRotation, Time.deltaTime * thrustRotSpeed);
    }

    //--------------------------------------------------------------------
    //RotateThrustersToQuat() Rotates thrusters towards angleRotation quaternion
    public void RotateThrustersToQuat(Quaternion angleRotation)
    {
        if (LThrusterGO != null)
            LThrusterGO.transform.rotation = Quaternion.Slerp(LThrusterGO.transform.rotation, angleRotation, Time.deltaTime * thrustRotSpeed);
        if (RThrusterGO != null)
            RThrusterGO.transform.rotation = Quaternion.Slerp(RThrusterGO.transform.rotation, angleRotation, Time.deltaTime * thrustRotSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, angleRotation, Time.deltaTime * thrustRotSpeed);
    }

    //--------------------------------------------------------------------
    //OnTriggerEnter2D() Detects Trigger collision and applies damage 
    void OnTriggerEnter2D(Collider2D c)
    {
        if (playerBodyDead) return;
        int damageAmount = 0;
        //TODO need to decide on the damageamounts
        if (this.gameObject.name == "PlayerBody" &&c.gameObject.name.StartsWith("regularShot"))
        {
            Debug.Log("PlayerMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from player");
            damageAmount = 10;
        }
        else if (this.gameObject.name == "PlayerBody" && c.gameObject.name.StartsWith("Meteorite"))
        {
            Debug.Log("PlayerMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from player");
            damageAmount = 100;
        }
        else if (this.gameObject.name == "PlayerBody" && c.gameObject.name.StartsWith( "EnemySimpleBody"))
        {
            Debug.Log("PlayerMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from player");
            damageAmount = 100;
        }
        else if (this.gameObject.name == "PlayerBody" && c.gameObject.name.Contains("Spiral") && c.gameObject.name.Contains("Body"))
        {
            Debug.Log("PlayerMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from player");
            damageAmount = 100;
        }
        else if (c.gameObject.name.StartsWith("Bullet"))
        {
            Debug.Log("PlayerMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from player");
            damageAmount = 10;
        }
        else if (c.gameObject.name.StartsWith("cannonShot"))
        {
            Debug.Log("PlayerMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from player");
            damageAmount = 5;
        }
        else if (c.gameObject.name.StartsWith("laserShot"))
        {
            Debug.Log("PlayerMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            damageAmount = 30;

        }
        else if (c.gameObject.name.StartsWith("scatterShot"))
        {
            Debug.Log("PlayerMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            damageAmount = 5;
        }
        else if (c.gameObject.name.StartsWith("threeShot"))
        {
            Debug.Log("PlayerMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from player");
            damageAmount = 3;
        }
        //Adjust health (check for player death happens in control
        if (!invinceable && damageAmount > 0 && triggerDeathScript != null)
        {
            health -= damageAmount;
        }

    }

    //-----------------------------------------------------------------------------------------
    //TriggerDeath() Start the death animation, hide the body 
    public void TriggerDeath(int lives)
    {
      
        if (triggerDeathScript != null)
        {
            playerBodyDead = true;
            triggerDeathScript.TriggerDeathAnim();
            if (audioScript != null)
            {
                if(lives <= 0)
                    SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange, audioScript.Death3);
                else
                    SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange, audioScript.Death1, audioScript.Death2);
            }
            foreach (Transform child in transform)
            {
                if (child.gameObject.name != "Body")
                {
                    child.gameObject.SetActive(false);
                }
                else if (child.gameObject.name == "Body")
                {
                    //check if the body has children (drop shadow or effect)
                    foreach(Transform gChild in child.transform)
                    {
                        gChild.gameObject.SetActive(false);
                    }
                }
            }
            StartCoroutine(HideBody());
        }
      
    }
    
    //-----------------------------------------------------------------------------------------
    //HideBody() waits until animation is done and resets the anim data
    IEnumerator HideBody()
    {
        yield return new WaitForSeconds(0.5f);
        bodyGO.SetActive(false);
        triggerDeathScript.TriggerDeathReset();
    }

    //-----------------------------------------------------------------------------------------
    //SetAllActive() Sets all parts of the player body to be active
    public void SetAllActive()
    {
       
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            if (child.gameObject.name == "Body")
            {
                //check if the body has children (drop shadow or effect)
                foreach (Transform gChild in child.transform)
                {
                    if (gChild.gameObject.name == "SpriteEffect")
                    {
                        gChild.gameObject.SetActive(false);
                    }
                    else if (gChild.gameObject.name == "ParticleEffect")
                    {
                        ParticleSystem particleSystem = gChild.gameObject.GetComponent<ParticleSystem>();
                        // Set the sorting layer of the particle system.
                        particleSystem.GetComponent<Renderer>().sortingLayerName = "foreground";
                        particleSystem.GetComponent<Renderer>().sortingOrder = 2;
                        gChild.gameObject.SetActive(true);
                        particleSystem.Stop();

                    }
                    else
                    {
                        gChild.gameObject.SetActive(true);
                    }
                }
            }
        }
      
        playerBodyDead = false;
    }

    public void SetInvincibleEffects(bool invince)
    {
       
        foreach (Transform child in transform)
        {
               
            if (child.gameObject.name == "Body")
            {
                //check if the body has children (effect)
                foreach (Transform gChild in child.transform)
                {
                    if (gChild.gameObject.name == "SpriteEffect")
                    {
                        if (invince)
                        {
                            gChild.gameObject.SetActive(true);
                        }
                        else
                        {
                            gChild.gameObject.SetActive(false);
                        }
                    }
                    else if(gChild.gameObject.name == "ParticleEffect")
                    {
                        ParticleSystem particleSystem = gChild.gameObject.GetComponent<ParticleSystem>();
                   
                        // Set the sorting layer of the particle system.
                        particleSystem.GetComponent<Renderer>().sortingLayerName = "foreground";
                        particleSystem.GetComponent<Renderer>().sortingOrder = 2;
                        if (invince)
                        {
                            particleSystem.Play();
                        }
                        else
                        {
                            particleSystem.Stop();
                        }
                    }
                       
                }
            }
        }
       
    }

    //-----------------------------------------------------------------------------------------
    //ShootRegularBullet()
    public void ShootRegularBullet()
    {
        if (CThrustShootEmitter != null)
        {
            //Center
            GameObject bulletGO = Instantiate(regularBulletPrefab, CThrustShootEmitter.transform.position, Quaternion.identity) as GameObject;
            Bullet bulletScript = bulletGO.GetComponent<Bullet>();
            Vector3 bulletDirection = new Vector3(0.0f, 1.0f, 0.0f);
            bulletGO.name = "player" + bulletGO.name;
            bulletDirection.Normalize();
            bulletScript.SetDirection(bulletDirection);
            Physics2D.IgnoreCollision(bulletGO.transform.GetComponent<BoxCollider2D>(), this.transform.GetComponent<BoxCollider2D>());
            if (audioScript != null)
            {
                SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange, audioScript.defaultShoot);
            }
        }
    }
    //-----------------------------------------------------------------------------------------
    //ShootLaserBullet()
    public void ShootLaserBullet()
    {
        if (CThrustShootEmitter != null)
        { 
            //Center
            GameObject bulletGO = Instantiate(laserBulletPrefab, CThrustShootEmitter.transform.position, Quaternion.identity) as GameObject;
            Bullet bulletScript = bulletGO.GetComponent<Bullet>();
            Vector3 bulletDirection = new Vector3(0.0f, 1.0f, 0.0f);
            bulletGO.name = "player" + bulletGO.name;
            bulletDirection.Normalize();
            bulletScript.SetDirection(bulletDirection);
            Physics2D.IgnoreCollision(bulletGO.transform.GetComponent<BoxCollider2D>(), this.transform.GetComponent<BoxCollider2D>());

            if (audioScript != null)
            {
                SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange,audioScript.Shoot3);
            }
        }
    }

    //-----------------------------------------------------------------------------------------
    //ShootThreeBullet()
    public void ShootThreeBullet()
    {
        if (CThrustShootEmitter != null)
        { 
            //Center
            GameObject bulletGO = Instantiate(threeBulletPrefab, CThrustShootEmitter.transform.position, Quaternion.identity) as GameObject;
            Bullet bulletScript = bulletGO.GetComponent<Bullet>();

            Vector3 bulletDirection = new Vector3(0.0f, 1.0f, 0.0f);
            bulletGO.name = "player" + bulletGO.name;
            bulletDirection.Normalize();
            bulletScript.SetDirection(bulletDirection);
            Physics2D.IgnoreCollision(bulletGO.transform.GetComponent<BoxCollider2D>(), this.transform.GetComponent<BoxCollider2D>());
            if (audioScript != null)
            {
                SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange,audioScript.Shoot4);
            }
        }
    }

    //-----------------------------------------------------------------------------------------
    //ShootScatterBullet()
    public void ShootScatterBullet()
    {
        if (CThrustShootEmitter != null)
        {
            List<Vector3> scatterDirections = new List<Vector3>() { new Vector3(0.0f, 1.0f, 0.0f), new Vector3(-1.0f, 1.0f, 0.0f), new Vector3(1.0f, 1.0f, 0.0f), new Vector3(-3.0f, 1.0f, 0.0f), new Vector3(3.0f, 1.0f, 0.0f) };

            for (int i = 0; i < 5; i++)
            {
                //Center
                GameObject bulletGO = Instantiate(scatterBulletPrefab, CThrustShootEmitter.transform.position, Quaternion.identity) as GameObject;
                Bullet bulletScript = bulletGO.GetComponent<Bullet>();
                bulletGO.name = "player" + bulletGO.name;
                Vector3 bulletDirection = scatterDirections[i];
                bulletDirection.Normalize();
                bulletScript.SetDirection(bulletDirection);
                Physics2D.IgnoreCollision(bulletGO.transform.GetComponent<BoxCollider2D>(), this.transform.GetComponent<BoxCollider2D>());
            }
            if (audioScript != null)
            {
                SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange, audioScript.Shoot5);
            }
        }
    }

    private void ShootFromCannon( GameObject CannonThrusterGO )
    {
        if (CannonThrusterGO != null)
        {
            //Emitter
            Transform thrustShootEmitter = CannonThrusterGO.transform.GetChild(0); //ASSUMES the Thruster has a child emitter to shoot from 

            GameObject bulletGO = Instantiate(cannonBulletPrefab, thrustShootEmitter.position, CannonThrusterGO.transform.rotation) as GameObject;
            Bullet bulletScript = bulletGO.GetComponent<Bullet>();
            Vector3 bulletDirection = new Vector3();
            bulletGO.name = "player" + bulletGO.name;
            bulletDirection = thrustShootEmitter.transform.position - CannonThrusterGO.transform.position;
            bulletDirection.Normalize();
            bulletScript.SetDirection(bulletDirection);
            Physics2D.IgnoreCollision(bulletGO.transform.GetComponent<BoxCollider2D>(), this.transform.GetComponent<BoxCollider2D>());

        }
    }

    //-----------------------------------------------------------------------------------------
    //ShootCannonBullet()
    public void ShootCannonBullet()
    {
      
        //shoot from left thruster
        ShootFromCannon(LThrusterGO);
        //shoot from right thruster
        ShootFromCannon(RThrusterGO);

        if (audioScript != null)
        {
            SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange, audioScript.Shoot2);
        }
    }

    //-----------------------------------------------------------------------------------------
    //ShootBulletInDirection()
    public void ShootBulletInDirection(Vector3 bulletDirection)
    {
        GameObject bulletGO = Instantiate(cannonBulletPrefab, transform.position, Quaternion.identity) as GameObject;
        bulletGO.name = "player" + bulletGO.name;
        Bullet bulletScript = bulletGO.GetComponent<Bullet>();
        bulletDirection.Normalize();
        bulletScript.SetDirection(bulletDirection);
        Physics2D.IgnoreCollision(bulletGO.transform.GetComponent<BoxCollider2D>(), this.transform.GetComponent<BoxCollider2D>());
    }


    //Play PickSounds
    public void PlayPickUpSound(PickUp.pickUpTypes pickUp)
    {
        //don't play sound if audio is not attached
        if (!audioScript)
            return; 
        switch (pickUp)
        {
            case PickUp.pickUpTypes.ONE_UP:
                SoundManager.instance.PlaySingle(audioScript.efxSource, audioScript.Sound1);
                break;
            case PickUp.pickUpTypes.INVINCE:
                SoundManager.instance.PlaySingle(audioScript.efxSource, audioScript.Sound2);
                break;
            case PickUp.pickUpTypes.CANNON:
                SoundManager.instance.PlaySingle(audioScript.efxSource, audioScript.Sound3);
                break;
            case PickUp.pickUpTypes.LASER:
                SoundManager.instance.PlaySingle(audioScript.efxSource, audioScript.Sound4);
                break;
            case PickUp.pickUpTypes.THREE:
                SoundManager.instance.PlaySingle(audioScript.efxSource, audioScript.Sound5);
                break;
            case PickUp.pickUpTypes.SCATTER:
                SoundManager.instance.PlaySingle(audioScript.efxSource, audioScript.Sound6);
                break;
        }

    }
}



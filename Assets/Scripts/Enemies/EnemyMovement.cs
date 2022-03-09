//
//EnemyMovement Script
//This script is used to control the movement for simple enemies (green) 
//and spiral simple enemies (purple).  It is attached to the "body" of enemy.
//Used in conjuction with EnemySimpleControl script (simple enemies) and
//EnemySpiralControl (spiral simple enemies)
//

using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    const int ENEMY_POINTS = 1;   //Defines the number of points achieved when destroying this enemy

    #region Public Members

    public GameObject BulletPrefab;  //Defines which type of bullet this enemy will shoot
    public bool aboutToDie = false;
    #endregion

    #region Private Members

    //Define speed and angle of rotations for thrusters as well as MAX rotation values
    private float thrustRotSpeed = 2.0f;
    private float maxThrustAngle = 15.0f;
    private float minThrustAngle = -15.0f;
    private float centerThrustAngle = 0.0f;
    private Quaternion leftThrustMax;                   //MAX  left rotation 
    private Quaternion rightThrustMax;                  //MAX  right rotation 
    private Quaternion neutralThrustMax;                //MAX  neutral rotation 
    private GOAudio audioScript;                        //Audio script
    //GameWorld data
    private GameObject gameWorld;           
    private GameWorldData gameWorldDataScript;

    private TestDrops dropScript;                      //Allows to create a pickup on destruction of this enemy

    private GameObject bodyGO;                         //GameObject representing the body of the enemy 

    #endregion
    //-----------------------------------------------------------------------------------
    // Start() Use this for initialization
    void Start()
    {
        //Get the body of this game object
        bodyGO = this.transform.Find("Body").gameObject;
        
        //init rotation values
        leftThrustMax       = Quaternion.AngleAxis(maxThrustAngle, new Vector3(0, 0, 1));
        rightThrustMax      = Quaternion.AngleAxis(minThrustAngle, new Vector3(0, 0, 1));
        neutralThrustMax    = Quaternion.AngleAxis(centerThrustAngle, new Vector3(0, 0, 1));

        //Init references to other necessary game scripts
        gameWorld = GameObject.Find("GameWorld");
        dropScript = gameWorld.GetComponent<TestDrops>();
        gameWorldDataScript = gameWorld.GetComponent<GameWorldData>();
        audioScript = GetComponent<GOAudio>();
    }

    //-----------------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        //do nothing
       
    }

    //-----------------------------------------------------------------------------------
    //RotateThrustersLeft() -- Rotates thrusters towards the maximum left rotation
    public void RotateThrustersLeft()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, leftThrustMax, Time.deltaTime * thrustRotSpeed);
    }

    //-----------------------------------------------------------------------------------
    //RotateThrustersRight() -- Rotates thrusters towards the maximum right rotation
    public void RotateThrustersRight()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, rightThrustMax, Time.deltaTime * thrustRotSpeed);
    }

    //-----------------------------------------------------------------------------------
    //RotateThrustersNeutral() --Rotates thrusters towards the maximum neutral rotation
    public void RotateThrustersNeutral()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, neutralThrustMax, Time.deltaTime * thrustRotSpeed);
    }

    //-----------------------------------------------------------------------------------
    //CheckBounds() Clamps the enemy position to stay in bounds of play area
    private void CheckBounds()
    {
        float x = transform.position.x;
        float y = transform.position.y;

        x = Mathf.Clamp(x, GameWorldData.MIN_X, GameWorldData.MAX_X);
        y = Mathf.Clamp(y, GameWorldData.MIN_Y, GameWorldData.MAX_Y);

        transform.position = new Vector3(x, y, transform.position.z);
        //print("position " + x + y + z);
    }
    //-----------------------------------------------------------------------------------
    //RotateThrustersToAngle rotate this transform towards angle
    public void RotateThrustersToAngle(float angle)
    {
        //find the rotation of the angle about the z axis
        Quaternion angleRotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        transform.rotation = Quaternion.Slerp(transform.rotation, angleRotation, Time.deltaTime * thrustRotSpeed);
    }

    //-----------------------------------------------------------------------------------
    //RotateThrustersToQuat rotate this transform towards Quaternion (angleRotation)
    public void RotateThrustersToQuat(Quaternion angleRotation)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, angleRotation, Time.deltaTime * thrustRotSpeed);
    }

    //-----------------------------------------------------------------------------------
    //Detects Trigger collision and applies appropriate damage
    void OnTriggerEnter2D(Collider2D c)
    {
        bool applyDamage = false;

        //TODO NEED A BETTER LOOKUP!
        #region TODO_BETTERLOOKUP
        
        if (c.gameObject.name.StartsWith("playerBullet"))
        {
            Debug.Log("EnemyMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerregularShot(Clone)"))
        {
            Debug.Log("EnemyMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy"); //GET DAMBAGE FROM BULLET SCRIPT AND APPLY
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playercannonShot(Clone)"))
        {
            Debug.Log("EnemyMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerlaserShot(Clone)"))
        {
            Debug.Log("EnemyMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerscatterShot(Clone)"))
        {
            Debug.Log("EnemyMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (c.gameObject.name.StartsWith("playerthreeShot(Clone)"))
        {
            Debug.Log("EnemyMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if ( c.gameObject.name.StartsWith("PlayerBody"))
        {
            Destroy(this.gameObject, 0.01f);
        }
        else if ( c.gameObject.name.StartsWith("Meteorite(Clone)"))
        {
            //Destroy(this.gameObject, 0.01f);
        }
     /*   else if (this.gameObject.name == "EnemySpiralSimpleBody" && c.gameObject.name.StartsWith("playerBullet(Clone)"))
        {
            Debug.Log("EnemyMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (this.gameObject.name == "EnemySpiralSimpleBody" && c.gameObject.name.StartsWith("playerregularShot(Clone)"))
        {
            Debug.Log("EnemyMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy"); //GET DAMBAGE FROM BULLET SCRIPT AND APPLY
            applyDamage = true;
        }
        else if (this.gameObject.name == "EnemySpiralSimpleBody" && c.gameObject.name.StartsWith("playercannonShot(Clone)"))
        {
            Debug.Log("EnemyMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (this.gameObject.name == "EnemySpiralSimpleBody" && c.gameObject.name.StartsWith("playerlaserShot(Clone)"))
        {
            Debug.Log("EnemyMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (this.gameObject.name == "EnemySpiralSimpleBody" && c.gameObject.name.StartsWith("playerscatterShot(Clone)"))
        {
            Debug.Log("EnemyMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (this.gameObject.name == "EnemySpiralSimpleBody" && c.gameObject.name.StartsWith("playerthreeShot(Clone)"))
        {
            Debug.Log("EnemyMovement Collided " + this.gameObject.name + " with " + c.gameObject.name + "Subtract hit points from enemy");
            applyDamage = true;
        }
        else if (this.gameObject.name == "EnemySpiralSimpleBody" && c.gameObject.name.StartsWith( "PlayerBody"))
        {
            Destroy(this.gameObject, 0.01f);
        }
        else if (this.gameObject.name == "EnemySpiralSimpleBody" && c.gameObject.name.StartsWith( "Meteorite(Clone)"))
        { 
        //    Destroy(this.gameObject, 0.01f);
        }*/
#endregion
        //Check to see if damage should be applied
        if (applyDamage)  
        {
            //Get current health script for enemy
            ApplyDamage applyDamageScript = this.gameObject.GetComponent<ApplyDamage>();
            //Get the bullet script attached to incoming bullet
            Bullet bulletScript = c.gameObject.GetComponent<Bullet>();
            //Get the damage amount to apply to enemy based on bullet
            int damage = bulletScript.damage;
            //Apply the damage amount to enemy and check if the enemy is going to die
            if (applyDamageScript.ApplyDamageToCharacter(damage))
            {
                aboutToDie = true;
                Destroy(this.gameObject, 0.5f);
                //If drop hasn't been made in last 10 secs then do a drop, also put a random range
                //World check for drop at death position
                if (dropScript != null)
                {
                    dropScript.DropBonus(this.transform.position);
                }
                //Update point for game because this enemy was destroyed
                gameWorldDataScript.AdjustGameScore(ENEMY_POINTS);
                if (audioScript != null)
                {
                    SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange, audioScript.Death1, audioScript.Death2);
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------
    //ShootBulletInDirection Instatiante a bullet prefab with initial rotation (rotatInit)
    //with given direction (bulletDirection). 
    public void ShootBulletInDirection(Vector3 bulletDirection, Quaternion rotateInit)
    {
        GameObject bulletGO = Instantiate(BulletPrefab, transform.position, rotateInit) as GameObject;
        Bullet bulletScript = bulletGO.GetComponent<Bullet>();
        bulletDirection.Normalize();
        bulletScript.SetDirection(bulletDirection);
        bulletScript.SetSpeed(30);
        //Ignore collisions with transform of calling game object
        Physics2D.IgnoreCollision(bulletGO.transform.GetComponent<BoxCollider2D>(), this.transform.GetComponent<BoxCollider2D>());

        if (audioScript != null)
        {
            SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange, audioScript.defaultShoot, audioScript.Shoot2);
        }

    }

}



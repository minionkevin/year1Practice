//
//EnemySimpleControl Script
//This script controls the state or mind of simple enemy it is the script attached to the "controller"
//

using UnityEngine;
using System.Collections;

public class EnemySimpleControl : MonoBehaviour {

    public GameObject enemyBodyPreFab;          //Defines the body to create
    public float fireRate = 0.5f;                     //Defines how fast to shoot
    public  float speed = 10.0f;                //Defines how fast to move
    public  float lifeTime =6.5f;                //Defines how long to live
    private float shootTime;                    //Defines when it is time to shoot
    private float updateDirectionTime;          //Defines when it is time to update direction of enemy (i.e. start following player)
    private float stopUpdatingDirection;        //Defines when to stop updating direction (i.e. following player)
    private Vector3 moveDirection;              //Defines the direction to move in
    private Vector3 upDirection;                //Defines the "UP" direction the enemy starts in 
    private bool ready;                         //Defines whether the enemy is ready to shoot (in play area)
    private float angle;                        //Defines the angle to rotate towards

    private EnemyMovement enemyMove;            //Stores the EnemyMovement script attached to body of game object
    private GameObject enemyBody;               //Stores the EnemyBody
    private GameObject playerBody;              //Stores the Player game object

   
    //--------------------------------------------------------------------------------
    //Start() Use this for initialization
    void Start()
    {
        //Initialize data members
        shootTime               = 0.0f;
        updateDirectionTime     = 0.0f;
        stopUpdatingDirection   = 2.5f;

        playerBody = GameObject.Find("PlayerBody");

  

        //Create the enemy body and rename for easier reference
        enemyBody = Instantiate(enemyBodyPreFab, transform.position, transform.rotation) as GameObject;
      
        enemyBody.name = "EnemySimpleBody";

        //Get the movement script off of the created enemy
        enemyMove = enemyBody.GetComponent<EnemyMovement>();

        //Set the up direction (+ve y)
        upDirection = new Vector3(0,10,0);

        //Set NOT ready
        ready = false;
        
        //Destroy the object once lifeTime is up
        Destroy(gameObject, lifeTime);
        Destroy(enemyBody,lifeTime);
    }

    //--------------------------------------------------------------------------------
    //OnGameOver() listen for OnGameOver message 
    void OnGameOver(int input)
    {
        //Destroy yourself it game is over
        Destroy(gameObject);
        Destroy(enemyBody);
    }

    //--------------------------------------------------------------------------------
    //UpdateDirection()  Updates the direction towards the player only for a certain
    //amount of time
    void UpdateDirection()
    {
        updateDirectionTime += Time.deltaTime;
        if (updateDirectionTime <= stopUpdatingDirection)
        {
            if (enemyBody != null && playerBody != null)
            {
                angle = 0.0f;
                //Get the direction in the X, Y towards player
                moveDirection = playerBody.transform.position - enemyBody.transform.position;
                moveDirection.z = 0.0f;

                //Get angle towards player based on the move direction and starting up direction
                if (moveDirection != Vector3.zero)
                {
                    angle = GameWorldData.AngleWindLeft(upDirection, moveDirection);
                }
                //Normalize the direction to remove "length"
                moveDirection.Normalize();
            }
        }
    }

    //--------------------------------------------------------------------------------
    //Update()  is called once per frame, controls direction via enemymovment script
    void Update()
    {
        Vector3 currentDirection; //Stores current direction (calculated)
        if (enemyBody == null)
            return;
        if (gameObject == null)
            return;
        if (playerBody == null)
            playerBody = GameObject.Find("PlayerBody");

        if (enemyMove.aboutToDie)
            ready = false;
        //Update the moveDirection to the player(maybe)
        UpdateDirection();
        //Rotate the enemy based on the moveDirection calculated above
        UpdateControl();

        //Debug Draws...
        //Debug.DrawRay(enemyBody.transform.position, moveDirection * 10, Color.red);
        //Debug.DrawRay(enemyBody.transform.position, upDirection, Color.blue);

        //Get the current direction of the player from its starting up direction
        currentDirection = enemyBody.transform.rotation * upDirection;
        //Debug.DrawRay(enemyBody.transform.position, currentDirection * 100, Color.yellow);

        //Get the angle between the current direction and the direction towards the player
        float angleBetween = Vector3.Angle(currentDirection, moveDirection); //NOTE: returns the smallest angle (so value is always 0 ..180 )
       
        //If we are rotated towards the player then we are ready to shoot
        if (angleBetween <= 1)
        {
            ready = true;
        }
    }

    //----------------------------------------------------------------------------------
    //UpdateControl() Controls rotation of enemy
    private void UpdateControl()
    {
        //Get the direction enemy is moving in (x and y only)
        Vector3 direction = new Vector3(moveDirection.x, moveDirection.y, 0);

        //Rotate the body based on angle calculated in UpdateDirection
        enemyMove.RotateThrustersToAngle(angle);

        #region TryWORLDspacetoLOCAL
        //FROM WORLD SPACE TO LOCAL SPACE
        //direction is in world space but we need to transform it to local space so that we can take into account the 
        //rotation of the object  (rotate to desired direction using angle, then move along the direction once we have found the proper
        //direction in local space
        //direction = enemyBody.transform.InverseTransformDirection(direction);
        //direction *= speed * Time.deltaTime;
        #endregion
        //OR TRY DIRECTION IN LOCAL SPACE (UP) 
        //Rotate direction based on angle, then move in that direction....
        Quaternion angleRotation = Quaternion.AngleAxis(angle, upDirection);
        direction = angleRotation * upDirection;
        direction.Normalize();

        //Move in proper direction, applying speed
        direction *= speed * Time.deltaTime;
       
       //Debug.DrawRay(enemyBody.transform.position, direction*100, Color.green);
        if (ready)
        {
            //Move body in direction
            enemyBody.transform.Translate(new Vector3(direction.x, direction.y, 0));

            shootTime += Time.deltaTime;
            //Check if time to shoot
            if (shootTime >= fireRate)
            {
                Vector3 bulletDirection; 

                if (playerBody == null)
                    playerBody = GameObject.Find("PlayerBody");

                //shoot direction towards player 
                bulletDirection = playerBody.transform.position - enemyBody.transform.position;
                enemyMove.ShootBulletInDirection(direction, enemyBody.transform.rotation);
                shootTime = 0;
                //Debug.Log("Firing at " + Time.deltaTime);
            }
       }
    }

    //----------------------------------------------------------------------------------
    //OnTriggerEnter2D() Detects Trigger collision
    void OnTriggerEnter2D(Collider2D c)
    {
       //Do nothing, damage is handled in EnemyMovement script
    }
}

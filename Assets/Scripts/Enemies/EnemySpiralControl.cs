//
//EnemySpiralControl Script
//This script controls the state or mind of spiral simple enemy it is the script attached to the "controller"
//
using UnityEngine;
using System.Collections;

public class EnemySpiralControl : MonoBehaviour {

    //Define state type for enemy (flying formation states)
    public enum enemyStateType { FLY_IN_LEFT, FLY_IN_RIGHT, SPIRAL_LEFT, SPIRAL_RIGHT, SPIRAL_OUT, ATTACK };
    public float fireRate = 0.5f;                     //Defines how fast to shoot
    public GameObject enemyBodyPreFab;      //Defines the enemy prefab to instantiate
    public float FLY_IN_TIME    = 1.0f;     //Time it takes to fly in
    public float FLY_OUT_TIME   = 1.0f;     //Time it takes to fly out
    public float speed = 8.5f;                    //Directional speed
    public float lifeTime = 30.0f;                 //Time to live

    private float rotSpeed;                 //Rotation speed
    private float shootTime;                //Time to shoot 
   
    private float updateStateTime;          //Tracks when to changed fly formation states
    private Vector3 moveDirection;          //Move direction towards player
    private Vector3 upDirection;            //Direction of initial/start "up"
    private float angle;                    //Angle to rotate
    private float startAngle;               //Angle to start at

    //Track other objects in game
    private GameObject  playerBody;         //player body (game object)
    private GameObject  enemyBody;          //Body instantiated by this script
    EnemyMovement       enemyMove;          //Movement script
    enemyStateType      enemyState = enemyStateType.FLY_IN_LEFT; //Track enemy state

    public void SetSpiralEnemyState(enemyStateType stateIn) { enemyState = stateIn; }

    //---------------------------------------------------------------------------------
    //Start() Use this for initialization
    void Start()
    {
        rotSpeed        = 150.0f;
        shootTime       = 0.0f;
        updateStateTime = 0.0f;

        playerBody = GameObject.Find("PlayerBody");

        //Instantiate the enemy and rename
        enemyBody = Instantiate(enemyBodyPreFab, transform.position, transform.rotation) as GameObject;
        enemyBody.name = "EnemySpiralSimpleBody";

        enemyMove = enemyBody.GetComponent<EnemyMovement>();
        upDirection = new Vector3(0, 10, 0);

        InitializeDirection();

        Destroy(gameObject, lifeTime);
        Destroy(enemyBody, lifeTime);
    }

    //---------------------------------------------------------------------------------
    //OnGameOver()
    void OnGameOver(int input)
    {
        Destroy(gameObject);
        Destroy(enemyBody);
    }

    //---------------------------------------------------------------------------------
    //InitializeDirection()  Inits the angle based on flying in from left or right
    //and sets the starting angle
    public void InitializeDirection()
    {
        angle = 0.0f;
        if (enemyState == enemyStateType.FLY_IN_LEFT)
        {
            angle = 270.0f;
        }
        else if (enemyState == enemyStateType.FLY_IN_RIGHT)
        {
            angle = 90.0f;
        }
        //keep track of initial angle
        startAngle = angle;
    }

    //---------------------------------------------------------------------------------
    //SetDirectionToPlayer Get the direction AND angle to the player
    public void SetDirectionToPlayer()
    {
        moveDirection = playerBody.transform.position - enemyBody.transform.position;
        moveDirection.z = 0.0f;
        if (moveDirection != Vector3.zero)
        {
            angle = GameWorldData.AngleWindLeft(upDirection, moveDirection);
        }
        moveDirection.Normalize();
    }

    //---------------------------------------------------------------------------------
    //UpdateDirectionAngleRight()
    //Spirals Right Up, come in from left (270 start angle)
    bool UpdateDirectionAngleRight()  
    {
        bool done = false;
        //adjust angle bit by bit (clock wise)
        angle -= 2;
        if (angle <= 0)
        {
            angle = 360;

        }
        //Check to see if we have done a full 360 from start angle
        if (angle == startAngle)
        {
            done = true;
        }
        return done;
    }
    //---------------------------------------------------------------------------------
    //UpdateDirectionAngleLeft()
    //Spirals Left Up , come in from right  (90 start angle)
    bool UpdateDirectionAngleLeft()  
    {
        bool done = false;

        //adjust angle bit by bit (counter clockwise)
        angle += 2;  
        if (angle >= 360)
        {
            angle = 0;

        }

        //Check to see if we have done a full 360 from start angle
        if (angle == startAngle)
        {
            done = true;
        }
        return done;
    }

    //---------------------------------------------------------------------------------
    // Update() is called once per frame, updates control (movement direction) and state of enemy
    void Update()
    {
        if (enemyBody == null)
            return;
        if (gameObject == null)
            return;
        if (playerBody == null)
            playerBody = GameObject.Find("PlayerBody");

        UpdateControl();
        UpdateState();
       //Debug.DrawRay(enemyBody.transform.position, moveDirection * 10, Color.red);
       //Debug.DrawRay(enemyBody.transform.position, upDirection, Color.blue);
       //Vector3 currentDirection = enemyBody.transform.rotation * upDirection;
       //Debug.DrawRay(enemyBody.transform.position, currentDirection * 100, Color.yellow);
    }


    //--------------------------------------------------------------------
    //UpdateControl() 
    private void UpdateControl()
    {
        Vector3 direction;

        //always rotate to the current set angle
        enemyMove.RotateThrustersToAngle(angle);


        //LOCAL SPACE (UP), rotate upDirection based on angle, then move in that direction....
        Quaternion angleRotation = Quaternion.AngleAxis(angle, upDirection);
        direction = angleRotation * upDirection;
        direction.Normalize();

        //MOVE IN PROPER DIRECTION
        direction *= speed * Time.deltaTime;
        enemyBody.transform.Translate(new Vector3(direction.x, direction.y, 0));
        //Debug.DrawRay(enemyBody.transform.position, direction * 100, Color.green);

    }

    //--------------------------------------------------------------------
    //UpdateState() Updates the state of the enemy
    void UpdateState()
    { 
        updateStateTime += Time.deltaTime;

        //Initially the enemy files in from the left or right to perform
        //spiral patter
        if (enemyState == enemyStateType.FLY_IN_LEFT)
        {
            //Flying in from left
            if (updateStateTime >= FLY_IN_TIME)
            {
                updateStateTime = 0.0f;
                shootTime = 0.0f;
                //If coming in from the left, spiral formation is clockwise (right)
                enemyState = enemyStateType.SPIRAL_RIGHT;
            }
        }
        else if (enemyState == enemyStateType.FLY_IN_RIGHT)
        {
            //Flying in from right
            if (updateStateTime >= FLY_IN_TIME)
            {
                updateStateTime = 0.0f;
                shootTime = 0.0f;
                //If coming in from the right, spiral formation is counter clockwise (left)
                enemyState = enemyStateType.SPIRAL_LEFT;
            }
        }
        else if (enemyState == enemyStateType.SPIRAL_RIGHT)
        {
            //update right spiral direction, if we are done the spiral, move out of scene
            if (UpdateDirectionAngleRight())
            {
                updateStateTime = 0.0f;
                shootTime = 0.0f;
                enemyState = enemyStateType.SPIRAL_OUT;
            }
        }
        else if (enemyState == enemyStateType.SPIRAL_LEFT)
        {
            //update left spiral direction
            if (UpdateDirectionAngleLeft())
            {
                updateStateTime = 0.0f;
                shootTime = 0.0f;
                enemyState = enemyStateType.SPIRAL_OUT;
            }
        }
        else if (enemyState == enemyStateType.SPIRAL_OUT)
        {
            //Once out of spiral set direction towards player
            if (updateStateTime >= FLY_OUT_TIME)
            {
                SetDirectionToPlayer();
                updateStateTime = 0.0f;
                shootTime = 0.0f;
                enemyState = enemyStateType.ATTACK;
            }
        }
        else if (enemyState == enemyStateType.ATTACK)
        {
            
            //Currently not shooting just flying into player
            shootTime += Time.deltaTime;
            if (shootTime >= fireRate)
            {
                if (playerBody == null)
                    playerBody = GameObject.Find("PlayerBody");

                //  Vector3 bulletDirection = enemyBody.transform.position - playerBody.transform.position;
                //  rocketMove.ShootBulletInDirection(bulletDirection);
                shootTime = 0;
                //Debug.Log("Firing at " + Time.deltaTime);
                // SetDirectionToPlayer();
            }
        }
    }

    //--------------------------------------------------------------------
    //Detects Trigger collision
    void OnTriggerEnter2D(Collider2D c)
    {
        //Do nothing -- damage applied by EnemyMovement script
    }
}

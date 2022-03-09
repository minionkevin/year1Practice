//
//Bullet Script
//This script is used to store bullet information and to move the bullet.
//Much of the data is set or applied via other scripts
//

using UnityEngine;
using System.Collections;


public class Bullet : MonoBehaviour
{
    //Explosion Effect
    //TODO: public GameObject Explosion;                    //Explosion for bullet -- CURRENTLY NOT USED       

    public float    speed;                          //Speed of bullet
    public float    lifeTime;                       //Life Time of bullet
    public int      damage;                         //Damage applied by bullet (used in other scripts on collisions)
    public Vector3  direction;                      //Direction bullet should travel in (set by scripts creating the bullet)

    public Vector3 GetDirection() { return direction; }
    public void SetDirection(Vector3 inDir) { direction.x = inDir.x; direction.y = inDir.y; direction.z = inDir.z; }
    public void SetSpeed(float inSpeed) { speed = inSpeed; }
    //-----------------------------------------------------------------------------
    //Start()
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    //-----------------------------------------------------------------------------
    //Update()
    void Update()
    {
        //move the bullet int the given direction
        transform.Translate(direction * speed * Time.deltaTime);
    }

    //-----------------------------------------------------------------------------
    //OnTriggerEnter2D()
    void OnTriggerEnter2D(Collider2D c)
    {
        //destroy bullet
        Destroy(gameObject);
    }

}
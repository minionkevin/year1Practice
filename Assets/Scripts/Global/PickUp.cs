//
//PickUp Script
//This script works with the player control script to handle what to do
//when the player collides with a "pick up".
//

using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

    public enum pickUpTypes { ONE_UP,INVINCE, THREE, LASER, CANNON, SCATTER }; //Possible pick up types
    public float lifeTime;                                                     //Time to die
    public pickUpTypes pickUpType;                                             //Pick up type of this object
 
    private GameObject playerControl;                                          //Player game object
    private PlayerController playerControlScript;                              //Player control script

    //-----------------------------------------------------------------------------------------
    // Start () Use this for initialization
    void Start () {
        //Get player game object/script
        playerControl = GameObject.Find("PlayerControl");
        playerControlScript = playerControl.GetComponent<PlayerController>();
        Destroy(gameObject, lifeTime); //Destroy pick up after lifeTime is reached
	}

    //-----------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update () {
	
        //Do nothing
	}

    //-----------------------------------------------------------------------------------------
    //HandlePickUp() Handles the pick up based on type, via the player control
    void HandlePickUp()
    {
        switch (pickUpType)
        {
            case pickUpTypes. ONE_UP:
                playerControlScript.StartOneUp();
                break;
            case pickUpTypes.INVINCE:
                playerControlScript.StartInvince();
                break;
            case pickUpTypes.THREE:
                playerControlScript.StartThree();
                break;
            case pickUpTypes.LASER:
                playerControlScript.StartLaser();
                break;
            case pickUpTypes.CANNON:
                playerControlScript.StartCannon();
                break;
            case pickUpTypes.SCATTER:
                playerControlScript.StartScatter();
                break;

        }
    }

    //-----------------------------------------------------------------------------------------
    //OnTriggerEnter2D()
    void OnTriggerEnter2D(Collider2D c)
    {
        //Check if we hit the player
        if (c.gameObject.name == "PlayerBody")
        {
            //Handle the pick up and destroy game object
            HandlePickUp();
            Destroy(gameObject);
        }
    }

}

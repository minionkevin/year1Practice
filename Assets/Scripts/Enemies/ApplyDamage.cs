//
//ApplyDamage Script
//This script is attached to objects for which you would like to 
//keep track of health and to apply damage to the the health.
//healthPoints MUST be set appropriately via editor once script is attached
//
using UnityEngine;
using System.Collections;

public class ApplyDamage : MonoBehaviour {

    public int healthPoints = 1;   //Total health points to start object with

    [HideInInspector]
    public bool aboutToDie;         //Tells when object is about to die

    [SerializeField]
     private TriggerDeath triggerDeathScript;

    //----------------------------------------------------------------------------------
    //ApplyDamageToCharacter() Allows you to apply the specified amount of damage to the 
    //object. Once damage is applied, returns whether the object is about to die (i.e. health
    //points = 0;
    public bool ApplyDamageToCharacter(int damage)
    {
        bool timeToDie = false;

        healthPoints -= damage;
        if (healthPoints <= 0)
        {
            healthPoints = 0;
            timeToDie = true;
            aboutToDie = true; //used to let other scripts check to see if this game object is going to die... 
            if(triggerDeathScript != null)
            {
                triggerDeathScript.TriggerDeathAnim();
            }
        }
        return timeToDie;
    }
    //--------------------------------------------------------------------------------------
	// Start() Use this for initialization
	void Start () {
        aboutToDie = false;
    }

    //--------------------------------------------------------------------------------------
    // Update() is called once per frame
    void Update()
    {
        //keeps track of whether the object is about to die...
        if (healthPoints <= 0)
        {
            aboutToDie = true;
        }
    }
}

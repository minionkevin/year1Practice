//
//TriggerDeath Script
//This script handles what to do when the player looses one life:
//Triggers Death animation.
//
using UnityEngine;
using System.Collections;

public class TriggerDeath : MonoBehaviour {

    public bool triggerDeath = false;  //Defines if death anim should be triggered
    private Animator anim;             //Holds Animator component for player animations
	//-----------------------------------------------------------------------------------
    // Use this for initialization
	void Start () {

        anim = GetComponent<Animator>();
	}

    //-----------------------------------------------------------------------------------
    // Update is called once per frame
    void Update () {

        if(anim != null && triggerDeath )
        {
            anim.SetBool("StartDeath", true);
        }
	
	}

    //-----------------------------------------------------------------------------------
    //TriggerDeathAnim()
    public void TriggerDeathAnim()
    {
        if (anim != null)
        {
            anim.SetBool("StartDeath", true);
        }
    }

    //-----------------------------------------------------------------------------------
    //TriggerDeathReset()
    public void TriggerDeathReset()
    {
        if (anim != null)
        {
            anim.SetBool("StartDeath", false);
        }
    }
}

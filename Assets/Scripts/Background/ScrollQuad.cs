using UnityEngine;
using System.Collections;
//CURRENTLY NOT USING THIS SCRIPT
public class ScrollQuad : MonoBehaviour {


    public float scrollSpeed;
    public float tileSizeY;


    private Vector3 startPosition;
    private GameObject gameWorld;
    private GameWorldData gameWorldDataScript;
    // Use this for initialization
    void Start () {
	
        //set the start position to be the transform position of this object
        startPosition = transform.position;
        gameWorld = GameObject.Find("GameWorld");
        gameWorldDataScript = gameWorld.GetComponent<GameWorldData>();
    }
	
	// Update is called once per frame
	void Update () {
        bool bPlaying = gameWorldDataScript.isGameInPlay();
        if (bPlaying)
        {
            //Mathf.Repeat is like the % operator. Recalll the modulus (%) operator gives you an int value within a certain range (i.e. 0 to 10)
            //Mathf.Repeat gives you a floating numberic value with in a given range (i.e. 0.0 - 10.0 )
            //the first value is the input value, the second value is the MAX value (if you input a value 20.0 and the max is 10.0 then it will give you a value of 10.0, input 15.0 gives you 5.0 )
            float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeY); //constrain the values from 0 to tileSizeY
           // float newPosition = Mathf.Repeat(Time.deltaTime * scrollSpeed, tileSizeY); //constrain the values from 0 to tileSizeY
            // change the transform position to the new position 
            transform.position = startPosition + Vector3.down * newPosition;
        }

	}
}

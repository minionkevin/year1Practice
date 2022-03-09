//
//TurretScroll Script
//This script controls the scrolling of the placed Turrets in the environment
//
using UnityEngine;
using System.Collections;

public class TurretScroll : MonoBehaviour {

    public float   scrollSpeed;                 //Defines how fast to scroll
    public float spaceBewteenTiles = 2.19f;
    public float maxTilesOnScreen = 6.0f;
    private Vector3 scrollDirection;             //Defines the direction to scroll in
    private float maxScrollValue;               //Defines when to loop the scroll     
    private GameObject gameWorld;               //GameWorld object
    private GameWorldData gameWorldDataScript;  //GameWorld script
    private Vector3 startPosition;              //Start position of scroll
    private float timeSinceStart;               //Time that has passed since scroll started
    private float timeAtStart;                  //Time when scroll started
    void Start ()
    {
        float adjustStartPosition;
        scrollDirection = new Vector3(0, -1, 0);
        //Init values
        timeSinceStart = 0.0f;
        gameWorld = GameObject.Find("GameWorld");
        gameWorldDataScript = gameWorld.GetComponent<GameWorldData>();
        startPosition = transform.position;
        timeAtStart = Time.time;

        //Define the maxScrollValue based on dimension of tiles
        //distance between tiles is ~2.19 with a max of 6 tiles including off screen space for buffering
        //Tiles for turret start and end offscreen
        maxScrollValue = spaceBewteenTiles * maxTilesOnScreen; 
        adjustStartPosition = Mathf.Repeat(timeAtStart * scrollSpeed, spaceBewteenTiles); //constrain the values from 0 to tileSizeY
        transform.position = startPosition;
    }
    //-------------------------------------------------------------------------------
    // Update() is called once per frame, update the position based on scroll position and time
    void Update()
    {
        timeSinceStart += Time.deltaTime;
        #region CHECK_FOR_DEATH_IMPLEMENT
        bool bPlaying = true;
        #endregion
        if (bPlaying)
        {

            //Mathf.Repeat is like the % operator. Recalll the modulus (%) operator gives you an int value within a certain range (i.e. 0 to 10)
            //Mathf.Repeat gives you a floating numberic value with in a given range (i.e. 0.0 - 10.0 )
            //The first value is the input value, the second value is the MAX value (if you input a value 20.0 and the max is 10.0 then it will give you a value of 10.0, input 15.0 gives you 5.0 )
            
            //Get the new position based on the time since we started and the scrollSpeed
            float newPosition = Mathf.Repeat(timeSinceStart * scrollSpeed, maxScrollValue); //constrain the values from 0 to maxScrollValue (See calc above)

            float y = Mathf.Repeat(Time.time * scrollSpeed, maxTilesOnScreen);  //update the y between 0 and 1
            //Change the transform position to the new position 
            transform.position = new Vector3(startPosition.x, startPosition.y + ( - 1* y * spaceBewteenTiles) );

            //Debug:
            //if (transform.position.y - startPosition.y >= 2.19 && transform.position.y - startPosition.y <= 2.2)
            //if (newPosition < 0.01f) 
            //     Debug.Log("**************Time to start " + timeSinceStart + "position " + newPosition + "Time to scroll " + (timeAtStart - Time.time));


        }
    }
}

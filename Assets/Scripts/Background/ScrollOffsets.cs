//
//ScrollOffset Script
//Scrolls the background image
//
using UnityEngine;
using System.Collections;

public class ScrollOffsets : MonoBehaviour {

    public float scrollSpeed;       //Defines the speed of the scroll (how fast does it scroll)
    private Renderer rend;          //Contains the Renderer component 
    private Vector2 savedOffset;    //Holds initial offset (only scrolling in y)

    private GameObject gameWorld;               //Holds the game world "manager" object
    private GameWorldData gameWorldDataScript; //Holds the game world manager script

    //----------------------------------------------------------------------------
    // Start() Use this for initialization
    void Start () {
        rend = GetComponent<Renderer>();
        savedOffset = rend.material.GetTextureOffset("_MainTex");
        gameWorld = GameObject.Find("GameWorld");
        gameWorldDataScript = gameWorld.GetComponent<GameWorldData>();
    }

    //----------------------------------------------------------------------------
    // Update is called once per frame, update the scroll in y direction
    void Update () {
        bool bPlaying = gameWorldDataScript.isGameInPlay();
        if (bPlaying)

        {
            float y = Mathf.Repeat(Time.time * scrollSpeed, 1);  //update the y between 0 and 1
            Vector2 offset = new Vector2(savedOffset.x, y); 
            rend.material.SetTextureOffset("_MainTex", offset);
        }
	}

    //----------------------------------------------------------------------------
    //OnDisable
    void OnDisable()
    {
        //reset offset to initial offset when game object is disabled
        rend.material.SetTextureOffset("_MainTex", savedOffset);
    }
}

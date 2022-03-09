//
//TestDrops Script
//Initial script used to test pick ups in the game
//Currently used to time drops when enemies are destroyed
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestDrops : MonoBehaviour {

    public bool testingDrops = false;      //Signifies when it is okay to drop an item, USED FOR TESTING

    public List<GameObject> dropPrefabs;  //Pick up items to drop
    
    //Timers
    float timeToDrop = 5.0f;
    float currentTime;
    int index;                            //Defines index into dropPrefabs array (which type to drop)

    private bool okToDrop;
    //-----------------------------------------------------------------------------------
    // Start () Use this for initialization
    void Start () {
         Random.seed = System.Environment.TickCount;
         timeToDrop = 5.0f;
         currentTime = 0.0f;
         index = 0;
         okToDrop = true;
    }
    //-----------------------------------------------------------------------------------
    // Update is called once per frame
    void Update() {

        if (GameWorldData.GetGameState() > GameWorldData.GameStateType.STAGE_INIT &&
            GameWorldData.GetGameState() > GameWorldData.GameStateType.STAGE_END)
        { if (testingDrops) //this means we just recently dropped one so lets wait
            {
                currentTime += Time.deltaTime;
                //Check if time to drop
                if (currentTime >= timeToDrop)
                {
                    currentTime = 0;
                    //  Testing drops here
                    Vector3 position = new Vector3(-2.0f, 3.0f, 0.0f);
                    GameObject drop = Instantiate(dropPrefabs[index], position, Quaternion.identity) as GameObject;
                    index++;
                    if (index >= dropPrefabs.Count)
                        index = 0;
                }
            }
            else
            {
                currentTime += Time.deltaTime;
                //Check if time to drop
                if (currentTime >= timeToDrop)
                {
                    currentTime = 0.0f;
                    okToDrop = true;
                }
            }
        }
    }
    //-----------------------------------------------------------------------------------
    //DropBonus() function used by other game objects (enemies) to drop a pick up
    public void DropBonus( Vector3 dropPos )
    {
        if (okToDrop)
        {
            currentTime = 0.0f;
            //if good time to drop bonus then do so
            int index = Random.Range(0, dropPrefabs.Count);
            GameObject drop = Instantiate(dropPrefabs[index], dropPos, Quaternion.identity) as GameObject;
            okToDrop = false;
        }
    }
}

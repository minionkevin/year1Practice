//
//MeteoriteMove Script
//This script controls the movement of the meteorite
//
using UnityEngine;
using System.Collections;

public class MeteoriteMove : MonoBehaviour {


    public Vector2 GetDirection() { return direction; }
    public void SetDirection(Vector2 inDir) { direction.x = inDir.x; direction.y = inDir.y;  }

    public float speed;                 //Speed of meteorite
    public float lifeTime;              //Life time of meteorite
    public Vector2 direction;           //Movement direction (should be normalized)

    private GOAudio audioScript;
    //--------------------------------------------------------------------------------------
    // Start () Use this for initialization
    void Start () {
        Destroy(gameObject, lifeTime);
        audioScript = GetComponent<GOAudio>();

    }

    //--------------------------------------------------------------------------------------
    // OnGameOver() Listen for game over message
    void OnGameOver(int input)
    {
        Destroy(gameObject);
    }

    //--------------------------------------------------------------------------------------
    // Update is called once per frame, translate the position of the meterite
    void Update () {

        direction =  Vector3.Normalize(new Vector3(direction.x, direction.y, 0));
        transform.Translate(direction * speed * Time.deltaTime);
	}

    //--------------------------------------------------------------------------------------
    //OnTriggerEnter2D
    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.name.StartsWith("player"))
        {
            if (audioScript != null)
            {
                SoundManager.instance.RandomizeSfx(audioScript.efxSource, audioScript.FXlowPitchRange, audioScript.FXhighPitchRange, audioScript.Death1, audioScript.Death2);
            }
            Debug.Log("MeteoriteMove Collided " + this.gameObject.name + " with " + c.gameObject.name + "Kill Meteorite");
            //Destroy the meteorite
            Destroy(this.gameObject, 0.1f);
            
            //TODO: Add to game score
        }
    }
}

using UnityEngine;
using System.Collections;
//
//Reusing from:  https://unity3d.com/learn/tutorials/projects/2d-roguelike-tutorial/audio-and-sound-manager?playlist=17150
//
public class SoundManager : MonoBehaviour {

    //public data members

   // public AudioSource efxSource;                       //Holds the audio source for effects
    public AudioSource musicSourceGame;                 //Holds the audio source for music in Game
    public AudioSource musicSourceStart;                //Holds the audio source for music in Start Menu
    public AudioSource musicSourceEndGame;              //Holds the audio source for music in EndGame

    public AudioSource UISource;                        //Holds the audio source for player
   
    public static SoundManager instance = null;     //Allows other scripts to call functions from SoundManager.         

  

    // Use this for initialization
    void Awake () {
        //Check if there is already an instance of the sound manager
        if (instance == null)
            instance = this;
        else if (instance != this)  //if this isn't the instace then destroy
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    //Used to play single sound clips.
    public void PlaySingle(AudioSource efxSource, AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        efxSource.clip = clip;

        //Play the clip.
        efxSource.Play();
    }

    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    public void RandomizeSfx(AudioSource efxSource, float lowPitchRange, float highPitchRange, params AudioClip[] clips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        efxSource.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        efxSource.clip = clips[randomIndex];

        //Play the clip.
        efxSource.Play();
    }


    //Used to play single sound clips.
    public void PlayUISingle (AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        UISource.clip = clip;

        //Play the clip.
        UISource.Play();
    }


    // Update is called once per frame
    void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class GOAudio : MonoBehaviour {
    
    public AudioSource efxSource;                       //Audio Source for GameObject
    public float FXlowPitchRange = .95f;                //The lowest a sound effect will be randomly pitched.
    public float FXhighPitchRange = 1.05f;               //The highest a sound effect will be randomly pitched.
                                     //Shoot Clips
    public AudioClip defaultShoot;  //regularShot (bulletDefault)
    public AudioClip Shoot2;        //cannon        (player only)
    public AudioClip Shoot3;        //laser         (player only)
    public AudioClip Shoot4;        //three shot    (player only)
    public AudioClip Shoot5;        //scatter       (player only)
    //Death Clips
    public AudioClip Death1;
    public AudioClip Death2;
    public AudioClip Death3;

    //Misc Sound Clips
    public AudioClip Sound1;
    public AudioClip Sound2;
    public AudioClip Sound3;
    public AudioClip Sound4;
    public AudioClip Sound5;
    public AudioClip Sound6;


}

using UnityEngine;
using System.Collections;

public class TurretSpawn : MonoBehaviour
{
    public GameObject enemyPrefab;              //Prefab of turret enemy to spawn
    private GameObject[] turretSpawnPoints;     //An array of all the turret spawn points on mid level
    private GameObject spawnedEnemy;            //holds a ref to the last spawned turret
    private GameObject midLevelGO;              //holds a ref to the mid level background
    private GameObject gameWorld;               //GameWorld Object
    private GameWorldData gameWorldDataScript;  //GameWorld Script
    // Use this for initialization
    void Start()
    {
        //Find the mid level background
        midLevelGO = GameObject.FindGameObjectWithTag("MidLevelLayer");

        //Find all spawn points for turret
        turretSpawnPoints = GameObject.FindGameObjectsWithTag("TurretSpawnPoint");
    }

    // Update is called once per frame
    void Update()
    {


    }

    //------------------------------------------------------------------------
    //SpawnTurret() Spawn a Turret at random spawn point.
    public void SpawnTurret(Vector3 position)
    {

        //find a random spawn point index into the spawn point array
        //create the enemy at the spawn point
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity) as GameObject;

        if (midLevelGO != null)
        {
           enemy.transform.parent = midLevelGO.transform;
        }
        //set the enemy we just spawned in case we need to refer to it later
        spawnedEnemy = enemy;

        //Turn Turret on once it has had a chance to scroll down into play area
        StartCoroutine(TurnTurretOn());
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        //Only spawn turrets in non boss states
        if ((GameWorldData.GetGameState() > GameWorldData.GameStateType.STAGE_INIT && GameWorldData.GetGameState() < GameWorldData.GameStateType.STAGE_END) && GameWorldData.GetGameState() != GameWorldData.GameStateType.STAGE_BIG_ENEMY &&
            GameWorldData.GetGameState() != GameWorldData.GameStateType.STAGE_BOSS)
        {
            if (c.gameObject.tag.StartsWith("TurretSpawnPoint"))
            {
                SpawnTurret(c.gameObject.transform.position);
            }
        }
    }

    public IEnumerator TurnTurretOn()
    {
        TurretMovement turretScript;
        //May need to adjust timing 
        yield return new WaitForSeconds(3.0f);
        if (spawnedEnemy != null)
        {
            turretScript = spawnedEnemy.GetComponent<TurretMovement>();
            if (turretScript != null)
            {
                //Turn on the guns
                turretScript.SetGunsOnOff(true);
            }
        }
    }
}
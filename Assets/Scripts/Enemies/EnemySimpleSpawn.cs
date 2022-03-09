//
//EnemySimpleSpawn Script
//This script is attached (multiple times) via enemySimpleSpawnArray in the GameWorldData script.
//It allows the control of spawning enemy types at specific way points in the game.
//
using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemySimpleSpawn : MonoBehaviour
{
    public GameWorldData.GameStateType stageToSpawn = GameWorldData.GameStateType.STAGE1;
    public GameObject enemyPrefab;              //Prefab of enemy to spawn
    public GameObject[] spawnSPoints;           //Spawn point array used to set different spawn positions for spawned enemy
    public float randomXRange = 2.0f;           //Spawns in x direction random range between -randomXRange to randomXRange
    public float randomYRange = 2.0f;           //Spawns in y direction random range between -randomXRange to randomXRange
    public float timeSpawnInterval = 1.0f;      //Interval time between spawning 

    public float waitToStartSpawn = 0.0f;
    public int   maxNumberToSpawn = -1;         //Number of enemies to spawn (total), -1 will spawn continously

    private bool spiral = false;
    private GameObject spawnedEnemy;           //The actual enemy that is spawned/instatied from prefab
    private float timeToSpawn;
    private float waitTime;
    private int currentNumSpawned;
    private bool initEnemy;
    //---------------------------------------------------------------------------------------
    //Start() Use this for initialization
    void Start()
    {
        currentNumSpawned = 0;
        spawnedEnemy = null;
        timeToSpawn = 0.0f;
        waitTime = 0.0f;
        initEnemy = false;
        if(enemyPrefab != null && (enemyPrefab.name.Contains("Spiral") || enemyPrefab.name.Contains("spiral")))
        {
            spiral = true;
        }
    }

    //-----------------------------------------------------------------------------------------
    //Update is called once per frame by GameWorldData script
    public void Update()
    {
        if (GameWorldData.GetGameState() == stageToSpawn)
        {
            //if the game state is one of the simple enemies then spawn at different intervals, otherwise spawn enemy directly in proper stage
            if (GameWorldData.GetGameState() <= GameWorldData.GameStateType.STAGE5)
            {
                waitTime += Time.deltaTime;
                if (waitTime < waitToStartSpawn)
                {
                    return;
                }
                timeToSpawn += Time.deltaTime;
                if (timeToSpawn > timeSpawnInterval && (maxNumberToSpawn == -1 || currentNumSpawned < maxNumberToSpawn))
                {
                    //Spawn enemy at proper spot
                    if (!spiral)
                    {
                        SpawnEnemy();   //simple enemy
                    }
                    else
                    {
                        if (spawnSPoints.Length > 0 && spawnSPoints[0].transform.position.x < 0)  //check if coming from left or right
                            SpawnSpiralEnemyFromIndex(0, 0); //simple enemy spiral from left
                        else
                            SpawnSpiralEnemyFromIndex(0, 1); //simple enemy spiral from right
                    }
                    timeToSpawn = 0.0f;
                    currentNumSpawned++;
                }

            }
            else if (GameWorldData.GetGameState() == GameWorldData.GameStateType.STAGE_BIG_ENEMY)
            {
                //Spawn Directly pnly once
                if (!initEnemy)
                {
                    spawnedEnemy = SpawnDirectly();
                    initEnemy = true;
                }
            }
            else if (GameWorldData.GetGameState() == GameWorldData.GameStateType.STAGE_BOSS)
            {
                //Spawn Directly pnly once
                if (!initEnemy)
                {
                    spawnedEnemy = SpawnDirectly();
                    initEnemy = true;
                }
            }
        }
    }

    //------------------------------------------------------------------------
    //SpawnEnemy()  Spawn prefab enemy at a random spawn point (in array)
    public GameObject SpawnEnemy()
    {
        //find a random spawn point index into the spawn point array
        int randomIndex = Random.Range(0, spawnSPoints.Length);

        //find random x and y values around the spawn point
        float randomX = Random.Range(-randomXRange, randomXRange);
        float randomY = Random.Range(-randomXRange, randomYRange);

        //create the enemy at the spawn point
        GameObject enemy = GameObject.Instantiate(enemyPrefab, spawnSPoints[randomIndex].transform.position, spawnSPoints[randomIndex].transform.rotation) as GameObject;

        randomX += spawnSPoints[randomIndex].transform.position.x;
        randomY += spawnSPoints[randomIndex].transform.position.y;

        //set random position for enemy
        enemy.transform.position = new Vector3(randomX, randomY, spawnSPoints[randomIndex].transform.position.z);
        
        //set the enemy we just spawned in case we need to refer to it later
        spawnedEnemy = enemy;

        //return enemy to calling function in case it needs to reference it later
        return enemy;
    }

    //------------------------------------------------------------------------
    //SpawnDirectly() Spawns prefab enemy at first spawn point in array
    public GameObject SpawnDirectly()
    {
        //create the enemy at the spawn point
        spawnedEnemy = GameObject.Instantiate(enemyPrefab, spawnSPoints[0].transform.position, spawnSPoints[0].transform.rotation) as GameObject;
     
        return spawnedEnemy;
    }

    //------------------------------------------------------------------------
    //SpawnSpiralEnemyFromIndex()
    //Spawn a prefab at spawn point specified by index into spawn array
    //Set which side to spawn on:
    //pos 0  == left, 1 == right (Sprial enemies need to know if they are starting from left or right)
    public GameObject SpawnSpiralEnemyFromIndex(int index, int pos)  
    {
        GameObject enemy = null;

        if (index < spawnSPoints.Length)
        {
            //create the enemy at the spawn point
            enemy = GameObject.Instantiate(enemyPrefab, spawnSPoints[index].transform.position, spawnSPoints[index].transform.rotation) as GameObject;

            if (pos == 0)
                enemy.GetComponent<EnemySpiralControl>().SetSpiralEnemyState(EnemySpiralControl.enemyStateType.FLY_IN_LEFT);
            else
                enemy.GetComponent<EnemySpiralControl>().SetSpiralEnemyState(EnemySpiralControl.enemyStateType.FLY_IN_RIGHT);

            enemy.GetComponent<EnemySpiralControl>().InitializeDirection();
        }

        //set the enemy we just spawned in case we need to refer to it later
        spawnedEnemy = enemy;

        //return enemy to calling function in case it needs to reference it later
        return enemy;
    }

    //------------------------------------------------------------------------
    //SpawnTurret() Spawn a Turret at random spawn point.
    public void SpawnTurret()
    { 
        //find a random spawn point index into the spawn point array
        int randomIndex = Random.Range(0, spawnSPoints.Length);
        //create the enemy at the spawn point
        GameObject enemy = GameObject.Instantiate(enemyPrefab, spawnSPoints[randomIndex].transform.position, spawnSPoints[randomIndex].transform.rotation) as GameObject;
        //set the enemy we just spawned in case we need to refer to it later
        spawnedEnemy = enemy;

        //Turn Turret on once it has had a chance to scroll down into play area
        StartCoroutine(TurnTurretOn());
    }

    //------------------------------------------------------------------------
    //TurnTurretOn()  Waits for 3 secs before turning guns on
    public IEnumerator TurnTurretOn()
    {
        //May need to adjust timing 
        yield return new WaitForSeconds(3.0f);

        //NOTE: assumes spawned enemy is a Turret at this point
        TurretMovement turretScript;
       
        turretScript = spawnedEnemy.GetComponent<TurretMovement>();
        if (turretScript != null)
        {
            //Turn on the guns
            turretScript.SetGunsOnOff(true);
        }
    }

}
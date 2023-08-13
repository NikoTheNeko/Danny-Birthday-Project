using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SushiSpawner : MonoBehaviour{

    #region Public Variables
    public float RespawnTime = 5f;

    public GameObject ObjectToSpawn;

    public Transform SpawnLocation;

    #endregion

    #region Private Variables

    #endregion

    // Start is called before the first frame update
    void Start(){
        StartCoroutine(SpawnPlateAtTimer());
    }

    // Update is called once per frame
    void Update(){
        
    }

    IEnumerator SpawnPlateAtTimer(){
        GameObject.Instantiate(ObjectToSpawn, SpawnLocation);
        yield return new WaitForSeconds(RespawnTime);
        StartCoroutine(SpawnPlateAtTimer());
    }
}

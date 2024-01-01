using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Tile1;
    public GameObject Tile2;
    public Transform parent;

    int NumberOfTiles = 2;

    public float delay = 1.0f;
    float timer;
    public float spawnZ = 15f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > delay)
        {
            TileCreate();
            timer -= delay;
        }

        
    }

    private void TileCreate(){
        int TileSelect = Random.Range(1, NumberOfTiles + 1);
        if (TileSelect == 1) {
            Instantiate(Tile1, new Vector3(0f, 0f, spawnZ), Quaternion.identity, parent);
        }
        if (TileSelect == 2) {
            Instantiate(Tile2, new Vector3(0f, 0f, spawnZ), Quaternion.identity, parent);
        }
    }
}

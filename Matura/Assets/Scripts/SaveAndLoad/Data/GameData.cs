using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData
{
    public int Health;
    public Vector3 playerPosition; 

    public GameData()
    {
        this.Health = 100;
        this.playerPosition = Vector3.zero; 
    }
}
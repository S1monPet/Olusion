using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public new string name = "New item"; //Name of item; 
    public string description = "New Description";
    public Sprite icon;
    public int currentQuantity = 1;
    public int maxQuantity = 16; 
}

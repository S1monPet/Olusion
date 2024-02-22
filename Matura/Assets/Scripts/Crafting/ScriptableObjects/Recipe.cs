using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Recipe", order = 1)]
public class Recipe : ScriptableObject
{
    public GameObject createdItemPrefab;
    public int quantityProduced = 1;
    public List<RequiredIngredients> requiredIngredients = new List<RequiredIngredients>(); //Ingredients
}

[System.Serializable] 
public class RequiredIngredients
{
    public string itemName;
    public int requiredQuantity; 


}

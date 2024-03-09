using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRespawning : MonoBehaviour
{
    public void CheckifItemIsRespawnable(Item currentItem)
    {
        if (!currentItem.Respawnable)
            return;

        RespawnItem(currentItem); 
    }
    public void RespawnItem(Item itemScript)
    {
        StartCoroutine(RespawnItemCoroutine(itemScript));
    }

    private IEnumerator RespawnItemCoroutine(Item itemScript)
    {
        yield return new WaitForSeconds(itemScript.RespawnTimer);

        itemScript.gameObject.SetActive(true);
    }
}

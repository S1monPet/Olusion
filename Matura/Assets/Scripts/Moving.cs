using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 10);
        StartCoroutine(SlowSpin());
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    float Amount = 0.1f;
    float delaySpeed = 0.01f;
    float distance = 500;

    // int rot = 0; Simon WTF are the names

    // int move = 100; Simon WTF are the names

    IEnumerator SlowSpin(){
    float count = 0;
    while(count < distance){
        gameObject.transform.Translate(new Vector3(0, 0, Amount*(-1)));
        if (count == 90){
            
        }
        count += Amount;
        yield return new WaitForSeconds(delaySpeed);
        
    }
    
    
    
}
}

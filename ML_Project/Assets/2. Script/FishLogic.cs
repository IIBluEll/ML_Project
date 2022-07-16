using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishLogic : MonoBehaviour
{
    public Area myArea;

    // Start is called before the first frame update
    void Start()
    {
        
    }

   public void OnEaten()
    {
        transform.position = new Vector3(Random.Range(-myArea.range, myArea.range), 1f, Random.Range(-myArea.range, myArea.range)) + myArea.transform.position;
    }
}

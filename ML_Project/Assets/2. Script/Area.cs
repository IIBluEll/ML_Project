using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    //public GameObject smallFish;
    public float range;
    public int numsmallFish;
    public int numbigFish;
    // Start is called before the first frame update
    void Start()
    {

    }

    void CreateFish(int num, GameObject type)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject f = Instantiate(type, new Vector3(Random.Range(-range, range), 1f, Random.Range(-range, range)) + transform.localPosition, Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));

           // f.GetComponent<FishLogic>().myArea = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class FishCollectorSettings : MonoBehaviour
{
    public GameObject[] agents;
    public FishCollectorArea[] listArea;

    StatsRecorder m_Recorder;

    public void Awake()
    {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
        m_Recorder = Academy.Instance.StatsRecorder;
    }
    void EnvironmentReset()
    {
        ClearObjects(GameObject.FindGameObjectsWithTag("SmallFish"));
        ClearObjects(GameObject.FindGameObjectsWithTag("BigFish"));

        agents = GameObject.FindGameObjectsWithTag("Agent");
        listArea = FindObjectsOfType<FishCollectorArea>();
        foreach (var fa in listArea)
        {
            fa.ResetFoodArea(agents);
        }

    }

    void ClearObjects(GameObject[] objects)
    {
        foreach (var food in objects)
        {
            Destroy(food);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

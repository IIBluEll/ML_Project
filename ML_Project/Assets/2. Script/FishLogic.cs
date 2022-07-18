using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishLogic : MonoBehaviour
{
    public bool respawn;
    public FishCollectorArea myArea;
    Transform Agent;

    // Start is called before the first frame update
    void Start()
    {
        Agent = myArea.gameObject.transform.GetChild(1).transform;
    }

    private void Update()
    {
        TagChanger();
        FollowAgent();
    }
    public void OnEaten()
    {
        if (respawn)
        {
            transform.position = new Vector3(Random.Range(-myArea.range, myArea.range),
                1f,
                Random.Range(-myArea.range, myArea.range)) + myArea.transform.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void TagChanger()
    {
        if(Agent.localScale.x- this.gameObject.transform.localScale.x >=0)
        {
            this.tag = "SmallFish";
        }
        else
        {
            this.tag = "BigFish";
        }
    }
    public void FollowAgent()
    {
        float dist = Vector3.Distance(Agent.position, this.gameObject.transform.position);
        if (this.CompareTag("BigFish") && dist<10)
        {
           // float t = Agent.gameObject.GetComponent<PlayerAgent2>().moveSpeed / 15;
            this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, Agent.position, 0.03f);
        }
    }
}

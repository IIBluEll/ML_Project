using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Linq;

public class PlayerAgent2 : Agent
{
    GameObject target;
    FishCollectorSettings m_FoodCollecterSettings;
    public GameObject area;
    FishCollectorArea m_MyArea;

    Rigidbody m_AgentRb;
    float m_LaserLength;
    // Speed of agent rotation.
    public float turnSpeed = 300;

    // Speed of agent movement.
    public float moveSpeed = 2;
    public float eatBigger = 0.05f;
    public bool contribute;
    public bool useVectorObs;
    EnvironmentParameters m_ResetParams;

    //
    private float dist;

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<FishCollectorArea>();
        m_FoodCollecterSettings = FindObjectOfType<FishCollectorSettings>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
           
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVectorObs)
        {
            var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
            sensor.AddObservation(localVelocity.x);
            sensor.AddObservation(localVelocity.z);
            sensor.AddObservation(GetClosetSmallFish().transform.position.x);
            sensor.AddObservation(GetClosetSmallFish().transform.position.z);
        }

    }

    GameObject GetClosetSmallFish()
    {
        var list = GameObject.FindGameObjectsWithTag("SmallFish").OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).ToList();

        target = list[0];

        dist = Vector3.Distance(this.transform.position, target.transform.position);

        return list[0].gameObject;
    }

    public void MoveAgent(ActionBuffers actionBuffers)
    {


        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var continuousActions = actionBuffers.ContinuousActions;
        var discreteActions = actionBuffers.DiscreteActions;


        var forward = Mathf.Clamp(continuousActions[0], 0f, 1f);
        var right = Mathf.Clamp(continuousActions[1], -1f, 1f);
        var rotate = Mathf.Clamp(continuousActions[2], -1f, 1f);

        dirToGo = transform.forward * forward;
        dirToGo += transform.right * right;
        rotateDir = -transform.up * -rotate;


        m_AgentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
        transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);


        if (m_AgentRb.velocity.sqrMagnitude > 25f) // slow it down
        {
            m_AgentRb.velocity *= 0.95f;
        }


    }

    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        
        MoveAgent(actionBuffers);

        if(transform.localPosition.y < 0)
        {
            EndEpisode();
        }

        if(dist < 2)
        {
            AddReward(0.1f);
        }

       
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        if (Input.GetKey(KeyCode.D))
        {
            continuousActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            continuousActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            continuousActionsOut[2] = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            continuousActionsOut[0] = -1;
        }
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    public override void OnEpisodeBegin()
    {
        
        m_AgentRb.velocity = Vector3.zero;
        
        transform.position = new Vector3(Random.Range(-m_MyArea.range, m_MyArea.range),
            2f, Random.Range(-m_MyArea.range, m_MyArea.range))
            + area.transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("SmallFish"))
        {
            collision.gameObject.GetComponent<FishLogic>().OnEaten();
            this.transform.localScale += new Vector3(eatBigger, eatBigger, eatBigger);
            AddReward(1f);
           
        }
        if (collision.gameObject.CompareTag("BigFish"))
        {
            collision.gameObject.GetComponent<FishLogic>().OnEaten();

            AddReward(-1f);
            EndEpisode();
            
        }
        if (collision.gameObject.name.Contains("Wall"))
        {

            AddReward(-0.05f);
            EndEpisode();

        }
    }

    public void SetResetParameters()
    {

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

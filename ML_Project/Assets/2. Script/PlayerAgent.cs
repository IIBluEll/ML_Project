using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerAgent : Agent
{
    Rigidbody rid;
    public Area myArea;
    public GameObject smallFish;
    public GameObject bigFish;
    public float movespeed = 1;
    public float turnSpeed = 150;
    public bool isEaten = false;

    

    // Start is called before the first frame update
    void Start()
    {
        rid = GetComponent<Rigidbody>();
        smallFish = GameObject.FindGameObjectWithTag("SmallFish");
        bigFish = GameObject.FindGameObjectWithTag("BigFish");

       
    }

    public override void OnEpisodeBegin()
    {
        if(this.transform.localPosition.y < 0)
        {
            this.rid.angularVelocity = Vector3.zero;
            this.rid.velocity = Vector3.zero;
             //this.transform.localPosition = new Vector3(0f, 1f, 0f);

            transform.position = new Vector3(Random.Range(-myArea.range, myArea.range), 1f, Random.Range(-myArea.range, myArea.range)) + myArea.transform.position;
            transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
        }
        else if(isEaten == true)
        {
            isEaten = false;
            this.rid.angularVelocity = Vector3.zero;
            this.rid.velocity = Vector3.zero;
            transform.position = new Vector3(Random.Range(-myArea.range, myArea.range), 1f, Random.Range(-myArea.range, myArea.range)) + myArea.transform.position;
            transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            //this.transform.localPosition = new Vector3(0f, 1f, 0f);
        }

        smallFish.GetComponent<FishLogic>().OnEaten();
        bigFish.GetComponent<FishLogic>().OnEaten();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var localVelocity = transform.InverseTransformDirection(rid.velocity);
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.z);
        //sensor.AddObservation(smallFish.transform.localPosition);
        //sensor.AddObservation(this.transform.localPosition);
    }

    public void MoveAgent(ActionBuffers actionBuffers)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;
        var continousAction = actionBuffers.ContinuousActions;
        var discreteAction = actionBuffers.DiscreteActions;

        var forward = Mathf.Clamp(continousAction[0], -1f, 1f);
        var right = Mathf.Clamp(continousAction[1], -1f, 1f);
        var rotate = Mathf.Clamp(continousAction[2], -1f, 1f);

        dirToGo = transform.forward * forward;
        dirToGo += transform.right * right;
        rotateDir = -transform.up * rotate;

        rid.AddForce(dirToGo * movespeed, ForceMode.VelocityChange);
        transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);

        if(rid.velocity.sqrMagnitude > 15f)
        {
            rid.velocity *= 0.95f;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveAgent(actions);

        if(transform.localPosition.y < 0)
        {
            EndEpisode();
            SetReward(-0.1f);
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("SmallFish"))
        {
            isEaten = true;
            AddReward(1f);
            EndEpisode();
            //collision.gameObject.GetComponent<FishLogic>().OnEaten();
        }
        else if(collision.gameObject.CompareTag("BigFish"))
        {
            isEaten = true;
            AddReward(-0.4f);
            EndEpisode();
            //collision.gameObject.GetComponent<FishLogic>().OnEaten();
        }
    }
}

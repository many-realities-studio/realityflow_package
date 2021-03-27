using System;
using System.Collections;
using UnityEngine;

public class GraphTrigger : MonoBehaviour
{

    public GameObject whiteBoard;
    public GameObject triggerObj;

    // triggerbox, collider, etc
    
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Object has hit a collider");
        RunGraph();
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("Object has entered a trigger");
        RunGraph();
    }

    void RunGraph(){
        whiteBoard.GetComponent<RealityFlowGraphView>().DoProcessing();
    }
}

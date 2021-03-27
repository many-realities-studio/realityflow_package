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
        RunGraph();
    }

    void OnTriggerEnter(Collider other) {
        RunGraph();
    }

    void RunGraph(){
        whiteBoard.GetComponent<RealityFlowGraphView>().DoProcessing();
    }
}

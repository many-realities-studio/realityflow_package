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
        //RunGraph();
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("Object has entered a trigger");
        if(triggerObj != null){ // now we have to test if the collider is the right GM
            if ( other.gameObject == triggerObj) {
                RunGraph();
            }
        } 
        else {
            RunGraph();
        }
    }

    void RunGraph(){
        whiteBoard.GetComponent<RealityFlowGraphView>().DoProcessing();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RealityFlow.Plugin.Contrib;
using GraphProcessor;

public class EdgeView : MonoBehaviour
{
    public SerializableEdge edge;
    public RealityFlowGraphView rfgv;

    public NodePortView input;
    public NodePortView output;

    // TODO: when we delete a node and the edge gets deleted, we need to make sure the remaining node has that edge removed from it's references as well
    // public void Delete(){
    public void Delete(NodePortView portBeingDeleted){
        if (input != null && portBeingDeleted != input) { input.DeleteEdge(this);}
        if (output != null && portBeingDeleted != output) { output.DeleteEdge(this);}
        // input?.DeleteEdge(this);
        // output?.DeleteEdge(this);
        rfgv.graph.Disconnect(edge);
        Destroy(this?.gameObject);
    }


    // recursive deletion strategy for edges: takes 

}

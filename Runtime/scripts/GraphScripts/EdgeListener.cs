using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;


/* This class is a refactor of EdgeConnectorListener from NGP.
   Unlike the other class this is designed to work with VR inputs instead of the UnityEditor */
public class EdgeListener : MonoBehaviour
{
    public RealityFlowGraphView graphView;

    // TODO: add dictionary or list to create edges
    // Save the NodePortViews instead of just the NodePort(s), so we can access the NodeViews for drawing the edges
    NodePortView inputView, outputView;
    NodePort input, output;

    // Constructor
    public EdgeListener(RealityFlowGraphView graphView)
    {
        this.graphView = graphView;
    }

    public void AddEdgeToGraph(){
        graphView.graph.Connect (input, output);
    }

    public void SelectInputPort(NodePortView input){
        this.inputView = input;
        // this.input = input.port;
        Debug.Log(input.port.portData);
        CheckForConnection();
    }

    public void SelectOutputPort(NodePortView output){
        this.outputView = output;        
        // this.output = output.port;        
        CheckForConnection();
    }

    private void CheckForConnection(){
        // TODO: We need to make sure this is a valid connection (get the port types and make sure they can be connected)
        // TODO: make sure we don't allow edges to be connected if they are trying to use a port that already has an edge
        if ( inputView != null && outputView != null){ // use the NodePortViews instead
        // if ( input != null && output != null){
            
            Debug.Log("Both ports are filled");
            // TODO: Update this, this only takes in the type of the node and not necessarily a type that can be cast to this node
            // if (this.input.owner.name == this.output.owner.name)
            if (this.inputView.port.owner.name == this.outputView.port.owner.name) // using NodePortViews instead
                // graphView.ConnectEdges(input, output);
                graphView.ConnectEdges(inputView, outputView);
            else
                Debug.Log("The types of ports are not connectable");
            inputView = null;
            outputView = null;
        }
        // Set the views to be null so we don't keep drawing lines between the previously clicked nodes
    }

}

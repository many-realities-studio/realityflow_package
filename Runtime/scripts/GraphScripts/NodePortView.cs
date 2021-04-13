using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using UnityEngine;
using UnityEngine.UI;

public class NodePortView : MonoBehaviour
{
    public Text fieldName;
    public Text currentNodeGUID;
    public NodePort port;
    public EdgeListener listener;
    public string type;

    public List<EdgeView> edges = new List< EdgeView >();

    // public NodeView parentNodeView; // Allows the NodePortView to have a ref to the NodeView, which can help us later draw edges between nodes

    public void SignalRedrawOnUpdate(bool flag){
        foreach(EdgeView e in edges){ e.ToggleUpdates(flag); }
    }

    public void SignalRedraw(){
        foreach(EdgeView e in edges){ e.RedrawEdge(); }
    }

    void RebuildUI(){
        fieldName.text = port.fieldName;
        string inputGUID, outputGUID;
        if(port.GetEdges().Count > 0) {
            inputGUID = ((BaseNode)port.GetEdges()[0].inputNode).GUID;
            outputGUID = ((BaseNode)port.GetEdges()[0].outputNode).GUID;
            if (inputGUID != port.owner.GUID)
            {
                currentNodeGUID.text = inputGUID.Substring(inputGUID.Length - 5);
            }
            if (outputGUID != port.owner.GUID)
            {
                currentNodeGUID.text = outputGUID.Substring(outputGUID.Length - 5);
            }
        }

        SignalRedraw();
    }

    public void SelectEdge(){
        Debug.Log("Selected port of type " + this.port.portData.displayType);
        switch (type)
        {
            case "input":
                listener.SelectInputPort(this);
                break;
            case "output":
                listener.SelectOutputPort(this);
                break;
        }

    }

    public void DeleteEdge(EdgeView edge){
        edges.Remove(edge); // Remove the edge from the list of tracked edges
        // edge.Delete(); // activate the deletion of the edge.
    }

    public void Delete(){
        // while (edges.Any()){
        //     edges[0].Delete();
        //     edges.RemoveAt(0);
        // }
        // if (!list.Any()){

        // } else {
        // edges.RemoveAll(e => e == null);
        foreach (EdgeView e in edges){
            // edges.Remove(e);
            e.Delete(this); 
        }
        edges.Clear();
        // }
        Destroy(this.gameObject);
    }

    public void DeleteFromWhiteBoard(){
        // while (edges.Any()){
        //     edges[0].Delete();
        //     edges.RemoveAt(0);
        // }
        // if (!list.Any()){

        // } else {
        // edges.RemoveAll(e => e == null);
        foreach (EdgeView e in edges){
            // edges.Remove(e);
            e.DeleteFromWhiteBoard(this); 
        }
        edges.Clear();
        // }
        Destroy(this.gameObject);
    }

    public void Init(NodePort np)
    {
        port = np;
        RebuildUI();
    }
}
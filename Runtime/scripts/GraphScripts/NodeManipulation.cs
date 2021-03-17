using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RealityFlow.Plugin.Contrib;
using UnityEngine.EventSystems;
using Microsoft.MixedReality.Toolkit.Input;

public class NodeManipulation : MonoBehaviour//,IMixedRealityPointerHandler
{
    //public LayerMask layerMask;
    Vector3 position;
    Quaternion rotation;
    public static NodeManipulation instance;
    GameObject rfgvGameObject; // realityflowgraphview script
    public RealityFlowGraphView rfgv;
    BaseGraph graph;
    void Awake()
    {
        instance = this;
        //position = this.transform.position;
        //rotation = this.transform.rotation;
        Physics.IgnoreLayerCollision(3, 3);
    }

    public void NodeRaycastCreation()
    {
        GameObject node = gameObject.transform.GetChild(0).gameObject;
        RaycastHit hit;
        if(Physics.Raycast(node.transform.position, 
        node.transform.TransformDirection(Vector3.forward), 
        out hit, 1<<6))
        {
            //Debug.DrawRay(node.transform.position, node.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
            Debug.Log("Did Hit");
            //rfgvGameObject = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(5).gameObject;
            rfgvGameObject = hit.collider.gameObject.transform.GetChild(2).gameObject;
            Debug.Log("rfgv object name is "+rfgvGameObject.name);
            rfgv = rfgvGameObject.GetComponent<RealityFlowGraphView>();
            graph = rfgv.graph;

            Vector3 [] cornerPos = new Vector3[4];
            hit.collider.gameObject.GetComponent<RectTransform>().GetWorldCorners(cornerPos);
            Debug.Log("Corners for Graph");
            foreach(Vector3 corner in cornerPos){
                Debug.Log(corner);
            }
            Debug.Log("Hit "+node.transform.position);
            Vector2 newPos = cornerPos[0];
            //node.transform.GetComponent<RectTransform>()
            AttachNodeToGraph();
        }
        else
        {
            Destroy(node);
            //Debug.DrawRay(node.transform.position, node.transform.TransformDirection(Vector3.forward) * 1000, Color.red);
            Debug.Log("Did not Hit");
        }
    }

    public void AttachNodeToGraph()
    {
        //rfgv.AddNodeCommand(this.transform.GetChild(0).tag, this.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition);
        rfgv.AddNodeCommand(this.transform.GetChild(0).tag);
        Destroy(this.gameObject);
    }
    public void RefreshPalette()
    {
        // Once a node is dragged off of the command palette, this will add a new one to take its place immediately
        GameObject o = Instantiate(this.gameObject,this.transform.position,this.transform.rotation,this.transform.parent);
        Debug.Log("Node should have been created to refresh the palette, it's name is "+o.name);
    }
}

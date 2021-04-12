using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using UnityEngine.EventSystems;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
//using Microsoft.MixedReality.Toolkit.UI;

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
        GameObject ptr = null;
        foreach (IMixedRealityController controller in MixedRealityToolkit.InputSystem.DetectedControllers)
        {
            if (controller.Visualizer?.GameObjectProxy != null)
            {
                Debug.Log("Visualizer Game Object: " + controller.Visualizer.GameObjectProxy);
            }
            else
            {
                Debug.Log("Controller has no visualizer!");
            }


            foreach (IMixedRealityPointer pointer in controller.InputSource.Pointers)
            {
                if (pointer is MonoBehaviour)
                {
                    var monoBehavior = pointer as MonoBehaviour;
                    Debug.Log("Found pointer game object: " + (monoBehavior.gameObject));
                    if( monoBehavior.gameObject.name.Contains("ParabolicPointer"))
                    {
                        ptr = monoBehavior.gameObject;
                    }
                }
            }
            if(ptr == null)
            {
                ptr = node;
            }
        }
        RaycastHit hit;
        //if(Physics.Raycast(node.transform.position, node.transform.TransformDirection(Vector3.forward), out hit, 1<<6))
        if( Physics.Raycast(node.transform.position, ptr.transform.forward, out hit, 1<<6) && ptr != null )
        {
            //Debug.DrawRay(node.transform.position, node.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
            Debug.Log("Did Hit");
            //rfgvGameObject = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(5).gameObject;
            rfgvGameObject = hit.collider.gameObject.transform.GetChild(2).gameObject;
            Debug.Log("rfgv object name is "+rfgvGameObject.name);
            rfgv = rfgvGameObject.GetComponent<RealityFlowGraphView>();
            graph = rfgv.graph;
            // rfgv.CheckOutGraph();

            Vector3 [] cornerPos = new Vector3[4];
            hit.collider.gameObject.GetComponent<RectTransform>().GetWorldCorners(cornerPos);
            Debug.Log("Corners for Graph");
            foreach(Vector3 corner in cornerPos){
                Debug.Log(corner);
            }
            Debug.Log("Hit "+node.transform.position);
            Vector3 worldNode = node.transform.position;
            Vector2 newPos;
            newPos = (worldNode - cornerPos[1]);
            Vector2 canvasDimensions = (cornerPos[2] - cornerPos[0]);
            Vector2 distFromUpperLeftCorner = newPos / canvasDimensions;
            Debug.Log("newPos:"+newPos);
            Debug.Log("canvasDimensions:"+canvasDimensions);
            Debug.Log("upper left:"+distFromUpperLeftCorner);
            //node.transform.GetComponent<RectTransform>()
            AttachNodeToGraph(distFromUpperLeftCorner);
        }
        else
        {
            Destroy(node);
            //Debug.DrawRay(node.transform.position, node.transform.TransformDirection(Vector3.forward) * 1000, Color.red);
            Debug.Log("Did not Hit");
        }
    }

    public void AttachNodeToGraph(Vector2 dist)
    {
        //rfgv.AddNodeCommand(this.transform.GetChild(0).tag, this.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition);
        rfgv.SetNewNodeLocation(dist);
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

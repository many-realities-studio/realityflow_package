using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using UnityEngine.EventSystems;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class NodeManipulation : MonoBehaviour
{
    Vector3 position;
    Quaternion rotation;
    public static NodeManipulation instance;
    GameObject rfgvGameObject;
    public RealityFlowGraphView rfgv;
    BaseGraph graph;
    void Awake()
    {
        instance = this;
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
        if( Physics.Raycast(node.transform.position, ptr.transform.forward, out hit, 1<<6) && ptr != null )
        {
            rfgvGameObject = hit.collider.gameObject.transform.GetChild(2).gameObject;
            rfgv = rfgvGameObject.GetComponent<RealityFlowGraphView>();
            graph = rfgv.graph;

            Vector3 [] cornerPos = new Vector3[4];
            hit.collider.gameObject.GetComponent<RectTransform>().GetWorldCorners(cornerPos);
            // foreach(Vector3 corner in cornerPos){
            //     Debug.Log(corner);
            // }
            Vector3 worldNode = node.transform.position;
            Vector2 newPos;
            newPos = (worldNode - cornerPos[1]);
            Vector2 canvasDimensions = (cornerPos[2] - cornerPos[0]);
            Vector2 distFromUpperLeftCorner = newPos / canvasDimensions;
            // Debug.Log("newPos:"+newPos);
            // Debug.Log("canvasDimensions:"+canvasDimensions);
            // Debug.Log("upper left:"+distFromUpperLeftCorner);
            AttachNodeToGraph(distFromUpperLeftCorner);
        }
        else
        {
            Destroy(node);
            Debug.Log("Node did not hit whiteboard");
        }
    }

    public void AttachNodeToGraph(Vector2 dist)
    {
        rfgv.SetNewNodeLocation(dist);
        rfgv.AddNodeCommand(this.transform.GetChild(0).tag);
        Destroy(this.gameObject);
    }
    public void RefreshPalette()
    {
        // Once a node is dragged off of the command palette, this will add a new one to take its place immediately
        GameObject o = Instantiate(this.gameObject,this.transform.position,this.transform.rotation,this.transform.parent);
    }
}

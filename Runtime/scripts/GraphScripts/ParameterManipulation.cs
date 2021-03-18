using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RealityFlow.Plugin.Contrib;
using UnityEngine.EventSystems;
using Microsoft.MixedReality.Toolkit.Input;

public class ParameterManipulation : MonoBehaviour//,IMixedRealityPointerHandler
{
    public LayerMask layerMask;
    Vector3 position;
    Quaternion rotation;
    public static ParameterManipulation instance;
    public GameObject nodeObject;
    public NodeView nodeView;
    void Awake()
    {
        instance = this;
        //position = this.transform.position;
        //rotation = this.transform.rotation;
        Physics.IgnoreLayerCollision(3, 3);
    }

    public void NodeRaycastCreation()
    {
        RaycastHit hit;
        if(Physics.Raycast(this.gameObject.transform.position, 
        this.gameObject.transform.TransformDirection(Vector3.forward), 
        out hit, layerMask))
        {
            //Debug.DrawRay(node.transform.position, node.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
            Debug.Log("Did Hit");
            nodeObject = hit.collider.gameObject;
            nodeView = nodeObject.GetComponent<NodeView>();
            AttachParameterToNode(this.gameObject.transform.GetComponent<RectTransform>().anchoredPosition);
        }
        else
        {
            Destroy(this.gameObject);
            //Debug.DrawRay(node.transform.position, node.transform.TransformDirection(Vector3.forward) * 1000, Color.red);
            Debug.Log("Did not Hit");
        }
    }

    public void AttachParameterToNode(Vector3 pos)
    {
        // TODO: Attach the parameter to the node via a new function in NodeView
        Destroy(this.gameObject);
    }
    public void RefreshPalette()
    {
        // Once a parameter is dragged off of the command palette, this will add a new one to take its place immediately
        GameObject o = Instantiate(this.gameObject,this.transform.position,this.transform.rotation,this.transform.parent);
        Debug.Log("Parameter should have been created to refresh the palette, it's name is "+o.name);
    }
}
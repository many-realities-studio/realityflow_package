using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using UnityEngine.EventSystems;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
//using Microsoft.MixedReality.Toolkit.UI;

public class ParameterManipulation : MonoBehaviour//,IMixedRealityPointerHandler
{
    //public LayerMask layerMask;
    Vector3 position;
    Quaternion rotation;
    public ParameterView pv;
    public static ParameterManipulation instance;
    void Awake()
    {
        instance = this;
        //position = this.transform.position;
        //rotation = this.transform.rotation;
        Physics.IgnoreLayerCollision(3, 3);
    }

    public void NodeRaycastCreation()
    {
        GameObject spherePoint = this.gameObject;
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
                ptr = spherePoint;
            }
        }
        RaycastHit hit;
        //if(Physics.Raycast(node.transform.position, node.transform.TransformDirection(Vector3.forward), out hit, 1<<6))
        if( Physics.Raycast(spherePoint.transform.position, ptr.transform.forward, out hit, LayerMask.NameToLayer("RFObject")) && ptr != null )
        {
            //Debug.DrawRay(node.transform.position, node.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
            Debug.Log("Did Hit");
            pv.SetParameterValue(hit.collider.gameObject);
            Destroy(spherePoint);
            //rfgvGameObject = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(5).gameObject;
        }
        else
        {
            Destroy(spherePoint);
            //Debug.DrawRay(node.transform.position, node.transform.TransformDirection(Vector3.forward) * 1000, Color.red);
            Debug.Log("Did not Hit");
        }
    }

    public void RefreshPalette()
    {
        // Once a node is dragged off of the command palette, this will add a new one to take its place immediately
        GameObject o = Instantiate(this.gameObject,this.transform.position,this.transform.rotation,this.transform.parent);
        Debug.Log("Node should have been created to refresh the palette, it's name is "+o.name);
    }
}
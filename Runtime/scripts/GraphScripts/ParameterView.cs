using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Packages.realityflow_package.Runtime.scripts;
using RealityFlow.Plugin.Scripts;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;

public class ParameterView : MonoBehaviour
{
    UnityEvent m_ParamDrag;
    public GameObject sphere;
    public GameObject modificationInput;
    public GameObject modificationDropdown;
    public GameObject modificationColor;
    public RealityFlowGraphView rfgv;
    public ExposedParameter pn;
    public GameObject s;

    public Text title;
    public Text type;
    public Text guid;
    void start()
    {
        s = null;
        // InputActionUnityEvent += ModifyParameterValue;
        //m_ParamDrag.AddListener(OnDrag);
    }
    // public void ListenerTrigger()
    // {
    //     if(m_ParamDrag!=null)
    //     {
    //         // determine what was manipulated
    //         m_ParamDrag.Invoke();
    //     }
    // }
    // public void OnDrag()
    // {
    //     // TODO fix the transferring of the parameter node to the normal node canvas
    //     this.transform.parent = rfgv.contentPanel.transform;
    // }

    public void AddParameterNodeToGraph(){
        // rfgv.AddNode("ParameterNode");
        //rfgv.CheckOutGraph();
        rfgv.AddParameterNodeToGraph(pn.guid);
        //rfgv.CheckInGraph();
    }

    public void DeleteParam()
    {
        //rfgv.CheckOutGraph();
        Debug.Log("Deleting parameter");
        rfgv.RemoveParameter(this);
        this.Delete();
    }

    public void Delete(){
        //rfgv.CheckOutGraph();
        if(this.gameObject != null)
            Destroy(this.gameObject);
        rfgv.vsGraph.IsUpdated = true;
        //rfgv.CheckInGraph();
    }

    public void ResetOrientation(){
        Vector3 localPos = transform.localPosition;
        localPos.z = 0.0f;
        transform.localPosition = localPos;
        transform.localScale = Vector3.one;
        //this.GetComponent<RectTransform>().anchoredPosition3D.z = 0.0f;
    }

    public void SetParameterValue(object setValue){
        pn.serializedValue = new SerializableObject(setValue,typeof(object),null);
        s = null;

        if (pn.type == "UnityEngine.GameObject, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
        {
            GameObject valueObject = (GameObject)setValue;
            FlowObject_Monobehaviour monoBehaviour = valueObject.GetComponent<FlowObject_Monobehaviour>();
            FlowTObject flowTObj = monoBehaviour.underlyingFlowObject;

            if(rfgv.vsGraph.paramIdToObjId.ContainsKey(pn.guid))
            {
                rfgv.vsGraph.paramIdToObjId[pn.guid] = flowTObj.Id;
            }
            else
            {
                rfgv.vsGraph.paramIdToObjId.Add(pn.guid, flowTObj.Id);
            }
        }
        rfgv.vsGraph.IsUpdated = true;
        //rfgv.CheckInGraph();
    }

    public void ModifyParameterValue()
    {
        //rfgv.CheckOutGraph();
        rfgv.ModifyExposedParameterValue();
        modificationInput = rfgv.gameObject.transform.parent.transform.GetChild(8).gameObject;
        modificationDropdown = rfgv.gameObject.transform.parent.transform.GetChild(10).gameObject;
        modificationColor = rfgv.gameObject.transform.parent.transform.GetChild(9).gameObject;
        Debug.Log("pn.type is "+pn.type);
        if(pn.type == "UnityEngine.GameObject, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
        {
            if(s == null)
            {
                s = Instantiate(sphere, rfgv.gameObject.transform.position, Quaternion.identity);
            }
            // s.gameObject.transform.GetChild(0).transform.gameObject.GetComponent<ParameterManipulation>().pv = this;
            s.gameObject.GetComponent<ParameterManipulation>().pv = this;
            Debug.Log("GameObject EP modified, please select a game object to fill value:");
        }
        else if(pn.type == "System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
        {
            modificationInput.GetComponent<ParameterModificationConfirm>().pv = this;
            // bring up input field to edit string
            modificationInput.SetActive(true);
            Debug.Log("String EP Modified");
        }
        else if(pn.type == "System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
        {
            modificationInput.GetComponent<ParameterModificationConfirm>().pv = this;
            // bring up input field to edit string
            modificationInput.SetActive(true);
            Debug.Log("Int EP Modified");
        }
        else if(pn.type == "System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
        {
            modificationInput.GetComponent<ParameterModificationConfirm>().pv = this;
            // bring up input field to edit string
            modificationInput.SetActive(true);
            Debug.Log("Float EP Modified");
        }
        else if(pn.type == "System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
        {
            modificationDropdown.GetComponent<ParameterModificationConfirm>().pv = this;
            modificationDropdown.SetActive(true);
            Debug.Log("Boolean EP Modified");
        }
        else if(pn.type == "UnityEngine.Color, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
        {
            modificationColor.GetComponent<ParameterModificationConfirm>().pv = this;
            modificationColor.SetActive(true);
            Debug.Log("Color EP Modified");
        }
        else
        {
            Debug.LogError("invalid type, pn.type is "+pn.type);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;

public class ParameterView : MonoBehaviour
{
    UnityEvent m_ParamDrag;
    public GameObject sphere;
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
        rfgv.AddParameterNodeToGraph(pn.guid);
    }

    public void deleteParam()
    {
        Debug.Log("Deleting parameter");
        rfgv.RemoveParameter(pn);
        Destroy(this.gameObject);
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
    }

    public void ModifyParameterValue()
    {
        
        switch(pn.type)
		{
			default:
			case "UnityEngine.GameObject":
                if(s == null)
                {
                    s = Instantiate(sphere, rfgv.gameObject.transform.position, Quaternion.identity);
                }
                // s.gameObject.transform.GetChild(0).transform.gameObject.GetComponent<ParameterManipulation>().pv = this;
                s.gameObject.GetComponent<ParameterManipulation>().pv = this;
                Debug.Log("GameObject EP modified, please select a game object to fill value:");
                break;
			case "System.String":
                // bring up input field to edit string
                break;
			case "System.Single":
                // bring up input field to edit float
                break;
			case "System.Int32":
                // bring up input field to edit int
                break;
			case "System.Boolean":
                // bring up check mark to edit bool
                break;
			case "UnityEngine.Color":
                // bring up color wheel to edit color
                break;
		}
    }
}

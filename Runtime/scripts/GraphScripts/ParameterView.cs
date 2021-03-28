using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ParameterView : MonoBehaviour
{
    UnityEvent m_ParamDrag;
    public RealityFlowGraphView rfgv;
    public ExposedParameter pn;

    public Text title;
    public Text type;
    public Text guid;
    void start()
    {
        m_ParamDrag.AddListener(OnDrag);
    }
    public void ListenerTrigger()
    {
        if(m_ParamDrag!=null)
        {
            // determine what was manipulated
            m_ParamDrag.Invoke();
        }
    }
    public void OnDrag()
    {
        // TODO fix the transferring of the parameter node to the normal node canvas
        this.transform.parent = rfgv.contentPanel.transform;
    }

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
}

using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.UI;

public class ParameterView : MonoBehaviour
{
    public RealityFlowGraphView rfgv;
    public ExposedParameter pn;

    public Text title;
    public Text type;
    public Text guid;
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

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
}

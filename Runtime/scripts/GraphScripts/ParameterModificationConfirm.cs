using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParameterModificationConfirm : MonoBehaviour
{
    public InputField input;
    public ParameterView pv;
    public void inputConfirm()
    {
        switch(pv.pn.type)
		{
			default:
			case "System.String":
                pv.SetParameterValue(input.text);
                break;
			case "System.Single":
                pv.SetParameterValue(float.Parse(input.text));
                break;
			case "System.Int32":
                pv.SetParameterValue(int.Parse(input.text));
                break;
		}
        this.gameObject.SetActive(false);
    }
}

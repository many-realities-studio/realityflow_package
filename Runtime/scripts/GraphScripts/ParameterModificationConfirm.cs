using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParameterModificationConfirm : MonoBehaviour
{
    public InputField input;
    public Dropdown dropdown;
    public ParameterView pv;
    
    void Start() {
        if(dropdown != null)
        {
            dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChangedHandler(dropdown);
            });
        }
    }

    void Destroy() {
        dropdown.onValueChanged.RemoveAllListeners();
    }

    private void DropdownValueChangedHandler(Dropdown target) {
        string dropDownText = target.options[target.value].text;
    }

    public void inputConfirm()
    {
        switch(pv.pn.type)
		{
			default:
			case "System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089":
                pv.SetParameterValue(input.text);
                break;
			case "System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089":
                pv.SetParameterValue(float.Parse(input.text));
                break;
			case "System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089":
                pv.SetParameterValue(int.Parse(input.text));
                break;
		}
        this.gameObject.SetActive(false);
    }
    public void dropdownConfirm()
    {
        if(dropdown.options[dropdown.value].text == "True")
        {
            pv.SetParameterValue(true);
        }
        else if(dropdown.options[dropdown.value].text == "False")
        {
            pv.SetParameterValue(false);
        }

        this.gameObject.SetActive(false);
    }
    public void colorConfirm()
    {
        if(dropdown.options[dropdown.value].text == "Red")
        {
            pv.SetParameterValue(Color.red);
        }
        if(dropdown.options[dropdown.value].text == "Black")
        {
            pv.SetParameterValue(Color.black);
        }
        if(dropdown.options[dropdown.value].text == "Yellow")
        {
            pv.SetParameterValue(Color.yellow);
        }
        if(dropdown.options[dropdown.value].text == "Green")
        {
            pv.SetParameterValue(Color.green);
        }
        if(dropdown.options[dropdown.value].text == "Blue")
        {
            pv.SetParameterValue(Color.blue);
        }
        if(dropdown.options[dropdown.value].text == "White")
        {
            pv.SetParameterValue(Color.white);
        }
    }
}

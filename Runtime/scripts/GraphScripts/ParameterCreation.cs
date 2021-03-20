using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ParameterCreation : MonoBehaviour
{
    public RealityFlowGraphView rfgv;
    public Dropdown typeDropdown;
    public bool ready;
    void Start() {
        //dropDownText = "no it cannot be null";
        ready = false;
        typeDropdown.onValueChanged.AddListener(delegate {
            TypeDropdownValueChangedHandler(typeDropdown);
        });
    }
    void Destroy() {
        typeDropdown.onValueChanged.RemoveAllListeners();
    }
    
    private void TypeDropdownValueChangedHandler(Dropdown target) {
        Debug.Log("selected: "+target.value);
        string dropDownText = target.options[target.value].text;
        //Debug.Log(dropDownText);
        //BoolNode.getDropDownValue(dropdownText);
    }

    public void GetDropDownValue()
    {
        this.gameObject.SetActive(false);
        ready = true;
        //Debug.Log(selectionDropdown.options[selectionDropdown.value].text);
        rfgv.setParameterType(typeDropdown.options[typeDropdown.value].text);
        //return selectionDropdown.options[selectionDropdown.value].text;
    }
}

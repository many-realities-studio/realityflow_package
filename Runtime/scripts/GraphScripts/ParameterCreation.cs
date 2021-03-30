using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ParameterCreation : MonoBehaviour
{
    public RealityFlowGraphView rfgv;
    public Dropdown typeDropdown;
    public InputField nameField;
    void Start() {
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
        //Debug.Log(selectionDropdown.options[selectionDropdown.value].text);
        rfgv.AddParameterStep2(typeDropdown.options[typeDropdown.value].text,nameField.text);
        //return selectionDropdown.options[selectionDropdown.value].text;
    }
}

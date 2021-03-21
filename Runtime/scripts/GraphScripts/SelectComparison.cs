using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectComparison : MonoBehaviour
{
    public RealityFlowGraphView rfgv;
    public Dropdown selectionDropdown;
    public bool ready;
    void Start() {
        //dropDownText = "no it cannot be null";
        ready = false;
        selectionDropdown.onValueChanged.AddListener(delegate {
            selectionDropdownValueChangedHandler(selectionDropdown);
        });
    }
    void Destroy() {
        selectionDropdown.onValueChanged.RemoveAllListeners();
    }
    
    private void selectionDropdownValueChangedHandler(Dropdown target) {
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
        rfgv.setComparison(selectionDropdown.options[selectionDropdown.value].text);
        //return selectionDropdown.options[selectionDropdown.value].text;
    }
}

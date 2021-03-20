using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectComparison : MonoBehaviour
{
    public Dropdown selectionDropdown;
    void Start() {
        selectionDropdown.onValueChanged.AddListener(delegate {
            selectionDropdownValueChangedHandler(selectionDropdown);
        });
    }
    void Destroy() {
        selectionDropdown.onValueChanged.RemoveAllListeners();
    }
    
    private void selectionDropdownValueChangedHandler(Dropdown target) {
        Debug.Log("selected: "+target.value);
        string dropdownText = target.options[target.value].text;
        //BoolNode.getDropDownValue(dropdownText);
    }

    public string getDropDownValue()
    {
        return selectionDropdown.options[selectionDropdown.value].text;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectComparison : MonoBehaviour
{
    public RealityFlowGraphView rfgv;
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
        string dropDownText = target.options[target.value].text;
    }

    public void GetDropDownValue()
    {
        this.gameObject.SetActive(false);
        rfgv.BoolNodeStep2(selectionDropdown.options[selectionDropdown.value].text);
    }
}

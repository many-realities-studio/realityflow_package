using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ParameterCreation : MonoBehaviour
{
    public RealityFlowGraphView rfgv;
    public Dropdown typeDropdown;
    public TMP_InputField nameField;
    void Start() {
        typeDropdown.onValueChanged.AddListener(delegate {
            TypeDropdownValueChangedHandler(typeDropdown);
        });
    }
    void Destroy() {
        typeDropdown.onValueChanged.RemoveAllListeners();
    }
    
    private void TypeDropdownValueChangedHandler(Dropdown target) {
        string dropDownText = target.options[target.value].text;
    }

    public void GetDropDownValue()
    {
        this.gameObject.SetActive(false);
        rfgv.AddParameterStep2(typeDropdown.options[typeDropdown.value].text,nameField.text);
    }
}

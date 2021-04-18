using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RealityFlow.Plugin.Scripts;
public class VSGraphSelectionDropdown : MonoBehaviour
{
    public RealityFlowGraphView rfgv;
    public Dropdown selectionDropdown;

    public List<FlowVSGraph> VSGraphList;
    void Start() {
        selectionDropdown.onValueChanged.AddListener(delegate {
            selectionDropdownValueChangedHandler(selectionDropdown);
        });
    }
    void Destroy() {
        selectionDropdown.onValueChanged.RemoveAllListeners();
    }
    
    private void selectionDropdownValueChangedHandler(Dropdown target) {
        string dropDownText = target.options[target.value].text;
    }

    public void LoadGraphs()
    {
        List<string> options = new List<string> ();
        VSGraphList.Clear();
        foreach (FlowVSGraph graph in FlowVSGraph.idToVSGraphMapping.Values)
        {
            options.Add(graph.Name);
            VSGraphList.Add(graph);
        }
        selectionDropdown.ClearOptions ();
        selectionDropdown.AddOptions(options);
    }

    public void GetDropDownValue()
    {
        // this function should simply confirm the user's choice
        // the dropdown value is: selectionDropdown.options[selectionDropdown.value].text
        if (selectionDropdown.options.Count > 0)
        {
            int index = selectionDropdown.value;
            Debug.Log(index);
            rfgv.InitializeGraph(VSGraphList[index]);
            VSGraphList.Clear();
            this.gameObject.SetActive(false);
        }
        else
        {
            VSGraphList.Clear();
            this.gameObject.SetActive(false);
        }
    }
}

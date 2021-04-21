using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is only needed on the Parameter View Prefab
public class SetParameterValue : MonoBehaviour
{
    [Header("Parameter Value Selection")]
    public GameObject parameterObjectSelectionView;
    public Dropdown paramaterValueSelection; 
    public ParameterView pv;

    void Start()
    {
        parameterObjectSelectionView = GameObject.Find("ParameterValueDropdownPanel");
        if(parameterObjectSelectionView != null)
            paramaterValueSelection = parameterObjectSelectionView.transform.GetChild(0);
        else
            Debug.Log("Could not find ParameterValueDropdownPanel");

        pv = this.gameObject.GetComponent<ParameterView>();
    }

    // This function loads all of the gameobjects that can be used for the Parameter
    // selection
    public void LoadObjectsforParameterSelection()
    {
        parameterObjectSelectionView.SetActive(true);
        List<string> options = new List<string> ();
        objectList.Clear();
        foreach (FlowTObject flowTObject in FlowTObject.idToGameObjectMapping.Values)
        {
            options.Add(flowTObject.Name);
            objectList.Add(flowTObject);
        }
        paramaterValueSelection.ClearOptions ();
        paramaterValueSelection.AddOptions(options);
    }

    // Upon confirming the object we set it in the Parameter View pv by calling SetParameterValue.
    public void ParameterSelection()
    {
        int index = paramaterValueSelection.value;
        pv.SetParameterValue(objectList[index]);
        objectList.Clear();
        parameterObjectSelectionView.SetActive(false);
    }
}

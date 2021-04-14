using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchfabModuleManager : MonoBehaviour
{
    //public GameObject sketchfabImporterObject;
    public GameObject loginObject;
    public GameObject searchObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ToggelSketchfabModule()
    {
        loginObject.SetActive(!loginObject.activeInHierarchy);
        searchObject.SetActive(!searchObject.activeInHierarchy);
    }
}

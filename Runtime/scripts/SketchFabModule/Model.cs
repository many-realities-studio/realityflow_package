using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public GameObject Canvas;
    GameObject modelDownloadPanel;
    public SketchfabModel sketchFabModel;
    public SketchfabBrowserWindow browserWindow;
    public SketchfabBrowserManager _browserManager;
    public GameObject modelPanel;

    // Start is called before the first frame update
    void Start()
    {
        browserWindow = (SketchfabBrowserWindow)FindObjectOfType(typeof(SketchfabBrowserWindow));
        Canvas =  GameObject.FindWithTag("Canvas");
    }
    
    public void displayModelPage()
    {
        browserWindow._browserManager.fetchModelInfo(sketchFabModel.uid);
        modelDownloadPanel = Instantiate(modelPanel, Canvas.transform);
        modelDownloadPanel.transform.parent = Canvas.transform;
        modelDownloadPanel.name = sketchFabModel.name + "Panel";
        modelDownloadPanel.GetComponent<SketchfabModelWindow>().displayModelPage(sketchFabModel, browserWindow);
    }
}
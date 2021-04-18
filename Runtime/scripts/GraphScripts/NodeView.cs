using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using System;
using UnityEngine;
using UnityEngine.UI;
using RealityFlow.Plugin.Contrib;
using Newtonsoft.Json;
using Packages.realityflow_package.Runtime.scripts;

public class NodeView : MonoBehaviour
{
    public Text title;
    public Text GUID;
    public string nodeGUID;
    public GameObject inputPanel;
    public GameObject outputPanel;
    public BaseNode node;
    public RealityFlowGraphView rfgv;
    public Vector3 localPos;

    public Rigidbody nodeViewRigidbody;

    private bool constraintsFrozen;

    public bool CanBeModified { get => _canBeModified; set => _canBeModified = value; }

    [SerializeField]
    private bool _canBeModified;


    public List<NodePortView> inputPortViews = new List<NodePortView>();
    public List<NodePortView> outputPortViews = new List<NodePortView>();
    public static NodeView instance;
    
    // TODO: Make sure this is the best way to do this. I think this is really hacky -John
    [JsonConstructor]
    public NodeView(Vector3 LocalPos, string NodeGUID)
    {
        localPos = LocalPos;
        nodeGUID = NodeGUID;
        if(RealityFlowGraphView.nodeViewtoRFGVDict.ContainsKey(NodeGUID))
        {
            RealityFlowGraphView.nodeViewtoRFGVDict[NodeGUID].nodeViewDict[NodeGUID].UpdateNodeViewLocally(this);
            
        }
        else
        {
            Debug.LogWarning("Nodeview with specified GUID does not exist in the scene. Failed to update NodeView.");
            return;
        }
    }
    private void UpdateNodeViewLocally(NodeView newValues)
    {
        if(RealityFlowGraphView.nodeViewtoRFGVDict[nodeGUID].nodeViewDict[nodeGUID].CanBeModified == false)
        {
            bool tempCanBeModified = this.CanBeModified;
            this.transform.localPosition = newValues.localPos;
            this.CanBeModified = tempCanBeModified;
        }
    }
    private void UpdateNodeViewGlobally(NodeView newValues)
    {
        if (transform.hasChanged == true)
        {
            bool tempCanBeModified = this.CanBeModified;
            this.transform.localPosition = newValues.localPos;
            this.CanBeModified = tempCanBeModified;

            if (CanBeModified == true)
            {
                Operations.UpdateNodeView(this, ConfigurationSingleton.SingleInstance.CurrentUser, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => {/* Debug.Log(e.message);*/ });
            }
            transform.hasChanged = false;
        }
    }
    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        transform.hasChanged = false;
        localPos = transform.localPosition;
        nodeViewRigidbody = this.gameObject.GetComponent<Rigidbody>();

        nodeViewRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        constraintsFrozen = true;
    }
    public void RedrawEdges(bool flag){
        foreach(NodePortView npv in inputPortViews){
            if (npv.edges.Count != 0 ) { npv.SignalRedrawOnUpdate(flag); }
        }
        foreach(NodePortView npv in outputPortViews){
            if (npv.edges.Count != 0 ) { npv.SignalRedrawOnUpdate(flag); }
        }
    }

    void Update(){
        transform.rotation = Quaternion.identity;
        localPos = transform.localPosition;
        UpdateNodeViewGlobally(this);

        if (this.CanBeModified && constraintsFrozen)
        {
            constraintsFrozen = false;
            nodeViewRigidbody.constraints = RigidbodyConstraints.None;
        }
        
        if (!this.CanBeModified && !constraintsFrozen)
        {
            constraintsFrozen = true;
            nodeViewRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }


    public void DeleteSelf(){
        rfgv.DeleteSelection(this);
        this.Delete();
    }

    public void Delete(){
        foreach(NodePortView inputPort in inputPortViews){
            inputPort.Delete();
        }
        foreach(NodePortView outputPort in outputPortViews){
            outputPort.Delete();
        }
        rfgv.graph.RemoveNode(this.node);
        if(this.gameObject != null)
            Destroy(this.gameObject);
    }

    public void DeleteFromWhiteBoard()
    {
        foreach(NodePortView inputPort in inputPortViews){
            inputPort.DeleteFromWhiteBoard();
        }
        foreach(NodePortView outputPort in outputPortViews){
            outputPort.DeleteFromWhiteBoard();
        }
        if(this.gameObject != null)
            Destroy(this.gameObject);
    }

    public void Select(){
       rfgv.AddToSelectionNV(this);
       this.GetComponent<CanvasRenderer>().SetColor(Color.green);
    }

    public void ResetOrientation(){
        Vector3 localPos = transform.localPosition;
        localPos.z = 0.0f;
        transform.localPosition = localPos;
        transform.localScale = Vector3.one;
        RedrawEdges(false);
    }

    public void UpdateNodeValues()
    {
        Vector3 [] cornerPos = new Vector3[4];
        GameObject.Find("VRWhiteBoard").GetComponent<RectTransform>().GetWorldCorners(cornerPos);
        Vector3 worldNode = this.transform.position;
        Vector2 newPos;
        newPos = (worldNode - cornerPos[1]);
        Vector2 canvasDimensions = (cornerPos[2] - cornerPos[0]);
        Vector2 distFromUpperLeftCorner = newPos / canvasDimensions;
        node.position = new Rect(distFromUpperLeftCorner, new Vector2(100,100));

        if (CanBeModified)
        {
            rfgv.vsGraph.ManipulationEndGlobalUpdate(rfgv.vsGraph);
            this.CheckIn();
        }
        
    }

    public void CheckIn()
    {
        if (CanBeModified == true)
        {
            Operations.CheckinNodeView(nodeGUID, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) =>
            {
                // On successful checkin
                if (e.message.WasSuccessful == true)
                {
                    _canBeModified = false;
                }
            });
        }
    }

    public void CheckOut()
    {
        if (CanBeModified == false)
        {
            Operations.CheckoutNodeView(this, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) =>
                {
                    // On successful checkout
                    if (e.message.WasSuccessful == true)
                    {
                        _canBeModified = true;
                    }
                });
        }
    }
}

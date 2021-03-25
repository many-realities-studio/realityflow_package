using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.UI;
using RealityFlow.Plugin.Contrib;
public class NodeView : MonoBehaviour
{
    public Text title;
    public Text GUID;
    public GameObject inputPanel;
    public GameObject outputPanel;
    public BaseNode node;
    public RealityFlowGraphView rfgv;

    public List<NodePortView> inputPortViews = new List<NodePortView>();
    public List<NodePortView> outputPortViews = new List<NodePortView>();

    // TODO: Make sure this is the best way to do this. I think this is really hacky -John
    // public BaseGraph graph;

    // public GameObject rfgvGameObject; // realityflowgraphview script

    public static NodeView instance;

    // static List <BaseNode> deletionList;

    // Start is called before the first frame update
    /*public NodeUI(string title, BaseNode node, string GUID){
        this.title.text = title;
        this.node = node;
        this.GUID.text = GUID;
    }*/
    void Awake()
    {
        instance = this;
    }
    void Start()
    {   
        // deletionList = new List<BaseNode>();
    }

    void Update(){
        transform.rotation = Quaternion.identity;
        // transform.localPosition = new Vector3(transform.position.x, transform.position.y , 0.0f);
        // transform.localPosition.z = 0.0f;

    }

    // public void Setup(RealityFlowGraphView rfgvi)
    // {
    //     rfgv=rfgvi;
    //     graph = rfgvi.graph;
    // }



    public void Delete(){
        foreach(NodePortView inputPort in inputPortViews){
            inputPort.Delete();
        }
        foreach(NodePortView outputPort in outputPortViews){
            outputPort.Delete();
        }
        rfgv.graph.RemoveNode(node);
        // _____?.Invoke(node);
        //graph.SetDirty();
        Destroy(this.gameObject);
    }

    public void Select(){
    //    rfgv.AddToSelection(node);
       rfgv.AddToSelectionNV(this);
       this.GetComponent<CanvasRenderer>().SetColor(Color.green);
    }

    public void ResetOrientation(){
        Vector3 localPos = transform.localPosition;
        localPos.z = 0.0f;
        transform.localPosition = localPos;
        transform.localScale = Vector3.one;
        //this.GetComponent<RectTransform>().anchoredPosition3D.z = 0.0f;
    }

    public void UpdateNodeValues()
    {
        // TODO: FIX
        Debug.Log("Before update, node position is: "+node.position);
        // 
        node.position = new Rect(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(100,100)); //this.transform.position;
        Debug.Log("After update, node position is: "+node.position);
    }
    /*
    public void Delete() {
        deletionList.Add(node);
    }
    public void DeleteAll(){
        Debug.Log("Deletion list contains "+deletionList.Count);
        foreach(BaseNode n in deletionList)
        {
            RuntimeGraph.commandPalette.AddCommandToStack(new Command("Remove Node", n.GUID));
            graph.RemoveNode(n);
            //graph.SetDirty();
            Destroy(gameObject);
        }
    }
    */
}
